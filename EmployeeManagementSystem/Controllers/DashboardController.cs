using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Entity;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IRepository<Employee> empRepo;
        private readonly IRepository<Department> depRepo;
        private readonly IRepository<Leave> leaveRepo;
        private readonly IRepository<Attendance> attendanceRepo;

        public DashboardController(
            IRepository<Employee> empRepo,
            IRepository<Department> depRepo,
            IRepository<Leave> leaveRepo,
            IRepository<Attendance> attendanceRepo
        )
        {
            this.empRepo = empRepo;
            this.depRepo = depRepo;
            this.leaveRepo = leaveRepo;
            this.attendanceRepo = attendanceRepo;
        }

        // ===================== DASHBOARD SUMMARY =====================
        [HttpGet]
        public async Task<IActionResult> TotalSalary()
        {
            var empList = await empRepo.GetAll();
            var depList = await depRepo.GetAll();

            return Ok(new
            {
                totalSalary = empList.Sum(x => x.Salary ?? 0),
                employeeCount = empList.Count,
                departmentCount = depList.Count
            });
        }

        // ===================== DEPARTMENT DATA =====================
        [HttpGet("department-data")]
        public async Task<IActionResult> GetDepartmentData()
        {
            var depList = await depRepo.GetAll();
            var empList = await empRepo.GetAll();

            var result = empList
                .GroupBy(x => x.DepartmentId)
                .Select(g => new DepartmentDataDto
                {
                    Name = depList.FirstOrDefault(d => d.Id == g.Key)?.Name ?? "Unknown",
                    EmployeeCount = g.Count()
                })
                .ToList();

            return Ok(result);
        }

        // ===================== EMPLOYEE ON LEAVE TODAY =====================
        [HttpGet("employee-leave-today")]
        public async Task<IActionResult> GetEmployeeOnLeaveToday()
        {
            // ✅ FIX: DateOnly (no timezone bug)
            var today = DateOnly.FromDateTime(DateTime.Now);

            var leavesToday = await leaveRepo.GetAll(l =>
                l.LeaveDate == today &&
                l.Status == (int)LeaveStatus.Accepted
            );

            if (!leavesToday.Any())
                return Ok(new List<LeaveDto>());

            var employeeIds = leavesToday.Select(l => l.EmployeeId).Distinct().ToList();
            var employees = await empRepo.GetAll(e => employeeIds.Contains(e.Id));

            var result = leavesToday.Select(l => new LeaveDto
            {
                EmployeeId = l.EmployeeId,
                EmployeeName = employees.FirstOrDefault(e => e.Id == l.EmployeeId)?.Name ?? "Unknown",
                Type = l.Type,
                Reason = l.Reason,
                Status = l.Status,
                LeaveDate = l.LeaveDate
            }).ToList();

            return Ok(result);
        }

        // ===================== ABSENT SUMMARY =====================
        [HttpGet("absent-summary")]
        public async Task<IActionResult> GetEmployeeAbsentSummary(int year, int month)
        {
            var employees = await empRepo.GetAll();
            var result = new List<EmployeeAbsentDto>();

            var today = DateOnly.FromDateTime(DateTime.Now);
            var monthStart = new DateOnly(year, month, 1);
            var monthEnd = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            // ❌ Entire future month
            if (monthStart > today)
                return Ok(result);

            // ✅ Cap at today
            var endDate = monthEnd > today ? today : monthEnd;

            foreach (var emp in employees)
            {
                // 1️⃣ Working days (exclude Sundays)
                var allDates = Enumerable
                    .Range(0, endDate.DayNumber - monthStart.DayNumber + 1)
                    .Select(d => monthStart.AddDays(d))
                    .Where(d => d.DayOfWeek != DayOfWeek.Sunday)
                    .ToList();

                // 2️⃣ Present attendance
                var presentDates = (await attendanceRepo.GetAll(a =>
                    a.EmployeeId == emp.Id &&
                    a.Type == (int)AttendanceType.Present &&
                    DateOnly.FromDateTime(a.Date) >= monthStart &&
                    DateOnly.FromDateTime(a.Date) <= endDate
                ))
                .Select(a => DateOnly.FromDateTime(a.Date))
                .ToHashSet();

                // 3️⃣ Approved leaves
                var approvedLeaveDates = (await leaveRepo.GetAll(l =>
                    l.EmployeeId == emp.Id &&
                    l.Status == (int)LeaveStatus.Accepted &&
                    l.LeaveDate >= monthStart &&
                    l.LeaveDate <= endDate
                ))
                .Select(l => l.LeaveDate)
                .ToHashSet();

                // 4️⃣ Absent calculation
                var absentDays = allDates.Count(d =>
                    !presentDates.Contains(d) &&
                    !approvedLeaveDates.Contains(d)
                );

                result.Add(new EmployeeAbsentDto
                {
                    EmployeeName = emp.Name,
                    AbsentDays = absentDays
                });
            }

            return Ok(result);
        }
    }
}
