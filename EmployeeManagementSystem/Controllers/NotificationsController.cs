using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserHelper _userHelper;

        public NotificationController(AppDbContext context, UserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        // 🔔 GET notifications (ADMIN or EMPLOYEE)
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var isAdmin = await _userHelper.IsAdmin(User);
            var employeeId = await _userHelper.GetEmployeeId(User);

            var notifications = isAdmin
                ? await _context.Notifications
                    .Where(n => n.EmployeeId == 0)
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new
                    {
                        n.Id,
                        n.Message,
                        n.IsRead,
                        n.CreatedAt
                    })
                    .ToListAsync()
                : await _context.Notifications
                    .Where(n => n.EmployeeId == employeeId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new
                    {
                        n.Id,
                        n.Message,
                        n.IsRead,
                        n.CreatedAt
                    })
                    .ToListAsync();

            return Ok(notifications);
        }

        // 🔴 UNREAD COUNT (bell badge)
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var isAdmin = await _userHelper.IsAdmin(User);
            var employeeId = await _userHelper.GetEmployeeId(User);

            var count = isAdmin
                ? await _context.Notifications.CountAsync(n => n.EmployeeId == 0 && !n.IsRead)
                : await _context.Notifications.CountAsync(n => n.EmployeeId == employeeId && !n.IsRead);

            return Ok(count);
        }

        // ✅ MARK SINGLE AS READ (ON CLICK)
        [HttpPut("mark-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var isAdmin = await _userHelper.IsAdmin(User);
            var employeeId = await _userHelper.GetEmployeeId(User);

            var notification = await _context.Notifications.FirstOrDefaultAsync(n =>
                n.Id == id &&
                (isAdmin ? n.EmployeeId == 0 : n.EmployeeId == employeeId)
            );

            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // ✅ MARK ALL AS READ
        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var isAdmin = await _userHelper.IsAdmin(User);
            var employeeId = await _userHelper.GetEmployeeId(User);

            var notifications = isAdmin
                ? await _context.Notifications
                    .Where(n => n.EmployeeId == 0 && !n.IsRead)
                    .ToListAsync()
                : await _context.Notifications
                    .Where(n => n.EmployeeId == employeeId && !n.IsRead)
                    .ToListAsync();

            foreach (var n in notifications)
                n.IsRead = true;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // 🧹 CLEAR ALL
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAll()
        {
            var isAdmin = await _userHelper.IsAdmin(User);
            var employeeId = await _userHelper.GetEmployeeId(User);

            var notifications = isAdmin
                ? await _context.Notifications.Where(n => n.EmployeeId == 0).ToListAsync()
                : await _context.Notifications.Where(n => n.EmployeeId == employeeId).ToListAsync();

            _context.Notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
