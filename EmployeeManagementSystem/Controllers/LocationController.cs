using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Entity;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmployeeManagementSystem.Controllers
{
    [ApiController]
    [Route("api/location")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly AppDbContext context;

        public LocationController(AppDbContext context)
        {
            this.context = context;
        }

        // ===================== EMPLOYEE CHECK-IN =====================
        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] LocationDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Email claim missing in token");
            }

            var employee = await context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.User.Email == email);

            if (employee == null)
            {
                return BadRequest("Employee profile not found for this user");
            }

            var fence = await context.GeoFences
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (fence == null)
            {
                return BadRequest("GeoFence not configured");
            }

            var distance = GeoHelper.DistanceInMeters(
                dto.Latitude,
                dto.Longitude,
                fence.CenterLatitude,
                fence.CenterLongitude
            );

            bool inside = distance <= fence.RadiusInMeters;

            context.EmployeeLocations.Add(new EmployeeLocation
            {
                EmployeeId = employee.Id,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsInsideBoundary = inside,
                CheckedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            return Ok(new { inside });
        }

        // ===================== ADMIN: ALL EMPLOYEE LOCATIONS =====================
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEmployeeLocations()
        {
            // 1️⃣ Load data FIRST (DB side)
            var rawLocations = await context.EmployeeLocations
                .Include(x => x.Employee)
                .ToListAsync();   // <-- IMPORTANT

            // 2️⃣ Group & pick latest location PER employee (MEMORY side)
            var locations = rawLocations
                .GroupBy(x => x.EmployeeId)
                .Select(g => g
                    .OrderByDescending(x => x.CheckedAt)
                    .First())
                .Select(x => new
                {
                    employeeId = x.EmployeeId,
                    name = x.Employee.Name,
                    latitude = x.Latitude,
                    longitude = x.Longitude,
                    isInside = x.IsInsideBoundary,
                    checkedAt = x.CheckedAt
                })
                .ToList();

            return Ok(locations);
        }

    }
}
