using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Service
{
    public class DailyAbsenceChecker
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public DailyAbsenceChecker(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task RunAsync()
        {
            // ✅ Always evaluate YESTERDAY
            var checkDate = DateTime.Now.Date.AddDays(-1);

            // 1️⃣ Active employees as of checkDate
            var employees = await _context.Employees
                .Where(e => e.LastWorkingDate == null || e.LastWorkingDate >= checkDate)
                .ToListAsync();

            foreach (var emp in employees)
            {
                if (string.IsNullOrWhiteSpace(emp.Email))
                    continue;

                // 2️⃣ Was employee present yesterday?
                bool isPresent = await _context.Attendances.AnyAsync(a =>
                    a.EmployeeId == emp.Id &&
                    a.Date == checkDate &&
                    a.Type == (int)AttendanceType.Present
                );

                // 3️⃣ Was employee on approved leave yesterday?
                bool onLeave = await _context.Leaves.AnyAsync(l =>
                    l.EmployeeId == emp.Id &&
                    l.LeaveDate == DateOnly.FromDateTime(checkDate) &&
                    l.Status == (int)LeaveStatus.Accepted
                );

                // 4️⃣ Absent → send email
                if (!isPresent && !onLeave)
                {
                    await _emailService.SendAbsentEmail(
                        emp.Email,
                        emp.Name ?? "Employee",
                        checkDate // ✅ IMPORTANT: yesterday's date
                    );
                }
            }
        }
    }
}
