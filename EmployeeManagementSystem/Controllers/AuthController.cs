using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Entity;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> userRepo;
        private readonly IConfiguration configuration;
        private readonly IRepository<Employee> empRepo;

        public AuthController(
            IRepository<User> userRepo,
            IConfiguration configuration,
            IRepository<Employee> empRepo)
        {
            this.userRepo = userRepo;
            this.configuration = configuration;
            this.empRepo = empRepo;
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto model)
        {
            var user = (await userRepo.GetAll(x => x.Email == model.Email))
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();

            if (user == null)
                return BadRequest(new { message = "user not found" });

            var helper = new PasswordHelper();

            if (!helper.VerifyPassword(user.Password, model.Password))
                return BadRequest(new { message = "email or password incorrect" });

            var token = GenerateToken(user.Email, user.Role);

            return Ok(new AuthTokenDto
            {
                Id = user.Id,
                Email = user.Email,
                Token = token,
                Role = user.Role
            });
        }

        // ================= JWT TOKEN =================
        private string GenerateToken(string email, string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JwtKey"]!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                // ✅ THIS FIX UNBLOCKS EVERYTHING
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ================= PROFILE =================
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = (await userRepo.GetAll(x => x.Email == email))
                        .OrderByDescending(x => x.Id)
                        .First();

            var employee = (await empRepo.GetAll(x => x.UserId == user.Id))
                            .FirstOrDefault();

            return Ok(new ProfileDto
            {
                Name = employee?.Name,
                Phone = employee?.Phone,
                Salary = employee?.Salary,
                Email = user.Email,
                ProfileImage = user.ProfileImage
            });
        }
    }
}
