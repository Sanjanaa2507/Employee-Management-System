using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Entity;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // protect controller
    public class AttendanceController : ControllerBase
    {
        private readonly IRepository<Attendance> attendanceRepo;
        private readonly UserHelper userHelper;

        public AttendanceController(
            IRepository<Attendance> attendanceRepo,
            UserHelper userHelper)
        {
            this.attendanceRepo = attendanceRepo;
            this.userHelper = userHelper;
        }

        // ================= MARK PRESENT =================
        [HttpPost("mark-present")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> MarkAttendance()
        {
            var employeeId = await userHelper.GetEmployeeId(User);

            if (employeeId <= 0)
            {
                return Unauthorized(new { message = "Invalid employee" });
            }

            var today = DateTime.Now.Date; // ✅ FIX (LOCAL DATE)

            var attendanceList = await attendanceRepo.GetAll(x =>
                x.EmployeeId == employeeId &&
                x.Date.Date == today
            );

            if (attendanceList.Any())
            {
                return BadRequest(new { message = "Already marked present today" });
            }

            var attendance = new Attendance
            {
                Date = DateTime.Now.Date,
                EmployeeId = employeeId,
                Type = (int)AttendanceType.Present,
                Status = "Present"
            };

            await attendanceRepo.AddAsync(attendance);
            await attendanceRepo.SaveChangesAsync();

            return Ok(new { message = "Attendance marked successfully" });
        }

        // ================= HISTORY =================
        [HttpGet]
        public async Task<IActionResult> GetAttendanceHistory([FromQuery] SearchOptions options)
        {
            if (!await userHelper.IsAdmin(User))
            {
                options.EmployeeId = await userHelper.GetEmployeeId(User);
            }

            if (!options.EmployeeId.HasValue)
            {
                return Unauthorized();
            }

            var list = await attendanceRepo.GetAll(x =>
                x.EmployeeId == options.EmployeeId.Value
            );

            var pagedData = new PagedData<Attendance>
            {
                TotalData = list.Count
            };

            if (options.PageIndex.HasValue)
            {
                list = list
                    .Skip(options.PageIndex.Value * options.PageSize.Value)
                    .Take(options.PageSize.Value)
                    .ToList();
            }

            pagedData.Data = list;
            return Ok(pagedData);
        }
    }
}
