using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Entity;
using System.Security.Claims;

namespace EmployeeManagementSystem.Service
{
    public class UserHelper
    {
        private readonly IRepository<User> userRepo;
        private readonly IRepository<Employee> empRepo;

        public UserHelper(IRepository<User> userRepo, IRepository<Employee> empRepo)
        {
            this.userRepo = userRepo;
            this.empRepo = empRepo;
        }

        public async Task<int> GetEmployeeId(ClaimsPrincipal userClaim)
        {
            var email =
                userClaim.FindFirstValue(ClaimTypes.Email) ??
                userClaim.FindFirstValue("email") ??
                userClaim.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
                throw new UnauthorizedAccessException("User email not found");

            var user = (await userRepo.GetAll(u => u.Email == email))
                       .FirstOrDefault();

            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            if (user.Role == "Admin")
                return 0;

            var employee = (await empRepo.GetAll(e => e.UserId == user.Id))
                           .FirstOrDefault();

            if (employee == null)
                throw new Exception("Employee record not found for this user");

            return employee.Id;
        }

        public async Task<bool> IsAdmin(ClaimsPrincipal userClaim)
        {
            var email =
                userClaim.FindFirstValue(ClaimTypes.Email) ??
                userClaim.Identity?.Name;

            var user = (await userRepo.GetAll(u => u.Email == email))
                       .FirstOrDefault();

            return user?.Role == "Admin";
        }
    }
}
