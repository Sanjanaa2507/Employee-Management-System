using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [ApiController]
    [Route("api/geofence")]
    [Authorize] // ✅ keep auth
    public class GeoFenceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GeoFenceController(AppDbContext context)
        {
            _context = context;
        }

        // ================= CURRENT GEOFENCE =================
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var fence = await _context.GeoFences
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (fence == null)
                return Ok(null);

            return Ok(fence);
        }

        // ================= LIVE EMPLOYEES =================
        [HttpGet("live")]
        public async Task<IActionResult> GetLiveEmployees()
        {
            var data = await _context.EmployeeLocations
                .Include(x => x.Employee)
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.CheckedAt).First())
                .Select(x => new
                {
                    name = x.Employee.Name,
                    latitude = x.Latitude,
                    longitude = x.Longitude,
                    isInsideBoundary = x.IsInsideBoundary
                })
                .ToListAsync();

            return Ok(data); // ✅ JSON GUARANTEED
        }
    }
}
