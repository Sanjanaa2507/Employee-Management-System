using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Entity;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly IRepository<Leave> leaveRepo;
        private readonly IRepository<Attendance> attendanceRepo;
        private readonly AppDbContext _context;
        private readonly UserHelper userHelper;
        private readonly EmailService _emailService;

        public LeaveController(
    IRepository<Leave> leaveRepo,
    IRepository<Attendance> attendanceRepo,
    AppDbContext context,
    UserHelper userHelper,
    EmailService emailService)
        {
            this.leaveRepo = leaveRepo;
            this.attendanceRepo = attendanceRepo;
            _context = context;
            this.userHelper = userHelper;

            _emailService = emailService;
        }

        // ================= APPLY LEAVE =================
        [HttpPost("apply")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveDto model)
        {
            if (model == null)
                return BadRequest("Invalid payload");

            if (model.Type == null || model.LeaveDate == null || string.IsNullOrEmpty(model.Reason))
                return BadRequest("All fields are required");

            var employeeId = await userHelper.GetEmployeeId(User);
            if (employeeId <= 0)
                return Unauthorized();

            var leave = new Leave
            {
                EmployeeId = employeeId,
                Reason = model.Reason,
                Type = model.Type.Value,
                LeaveDate = model.LeaveDate.Value,
                Status = (int)LeaveStatus.Pending
            };

            await leaveRepo.AddAsync(leave);
            await leaveRepo.SaveChangesAsync();

            // 🔍 GET EMPLOYEE NAME (for admin message)
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            var employeeName = employee?.User?.Email ?? $"Employee #{employeeId}";

            // 🔔 ADMIN NOTIFICATION
            _context.Notifications.Add(new Notification
            {
                EmployeeId = 0, // ADMIN
                Message = $"{employeeName} has applied for leave on {leave.LeaveDate:dd-MMM-yyyy}",
                IsRead = false,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Leave applied successfully" });
        }

        // ================= ACCEPT / REJECT =================
        [HttpPost("update-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLeaveStatus([FromBody] LeaveDto model)
        {
            if (model?.Id == null || model.Status == null)
                return BadRequest("Invalid request");

            var leave = await leaveRepo.FindByIdAsync(model.Id.Value);
            if (leave == null)
                return NotFound("Leave not found");

            leave.Status = model.Status.Value;

            var leaveDateTime = leave.LeaveDate.ToDateTime(TimeOnly.MinValue);

            if (model.Status == (int)LeaveStatus.Accepted)
            {
                bool attendanceExists = _context.Attendances.Any(a =>
                    a.EmployeeId == leave.EmployeeId &&
                    a.Date.Date == leaveDateTime.Date
                );

                if (!attendanceExists)
                {
                    _context.Attendances.Add(new Attendance
                    {
                        EmployeeId = leave.EmployeeId,
                        Date = leaveDateTime,
                        Type = (int)AttendanceType.Leave,
                        Status = "Leave"
                    });
                }

                // 🔔 EMPLOYEE NOTIFICATION
                _context.Notifications.Add(new Notification
                {
                    EmployeeId = leave.EmployeeId,
                    Message = $"Your leave for {leave.LeaveDate:dd-MMM-yyyy} has been approved.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });
            }

            if (model.Status == (int)LeaveStatus.Rejected)
            {
                _context.Notifications.Add(new Notification
                {
                    EmployeeId = leave.EmployeeId,
                    Message = $"Your leave for {leave.LeaveDate:dd-MMM-yyyy} has been rejected.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });
            }

           
await _context.SaveChangesAsync();

// Send email to employee after accepting or rejecting leave
if (model.Status == (int)LeaveStatus.Accepted ||
    model.Status == (int)LeaveStatus.Rejected)
{
    var employee = await _context.Employees
        .FirstOrDefaultAsync(e => e.Id == leave.EmployeeId);

    var employeeEmail = employee?.Email;
    var employeeName = employee?.Name ?? "Employee";

    if (!string.IsNullOrWhiteSpace(employeeEmail))
    {
        try
        {
            await _emailService.SendLeaveStatusEmail(
                employeeEmail,
                employeeName,
                leave.LeaveDate,
                model.Status == (int)LeaveStatus.Accepted
            );
        }
        catch (Exception ex)
        {
            // Email failure should not undo the leave status update
            Console.WriteLine($"Leave email failed: {ex.Message}");
        }
    }
}

return Ok();

        }

        // ================= LIST LEAVES =================
        [HttpGet]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> List([FromQuery] SearchOptions options)
        {
            List<Leave> list;

            if (await userHelper.IsAdmin(User))
            {
                list = await leaveRepo.GetAll();
            }
            else
            {
                var employeeId = await userHelper.GetEmployeeId(User);
                list = await leaveRepo.GetAll(x => x.EmployeeId == employeeId);
            }

            var pagedData = new PagedData<Leave>
            {
                TotalData = list.Count,
                Data = options.PageIndex.HasValue && options.PageSize.HasValue
                    ? list.Skip(options.PageIndex.Value * options.PageSize.Value)
                          .Take(options.PageSize.Value)
                          .ToList()
                    : list
            };

            return Ok(pagedData);
        }
    }
}
