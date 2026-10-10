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
    public class EmployeeController : ControllerBase
    {
        private readonly IRepository<Employee> employeeRepository;
        private readonly IRepository<User> userRepo;

        public EmployeeController(
            IRepository<Employee> employeeRepository,
            IRepository<User> userRepo)
        {
            this.employeeRepository = employeeRepository;
            this.userRepo = userRepo;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEmployeeList([FromQuery] SearchOptions searchOption)
        {
            var pagedData = new PagedData<Employee>();

            if (string.IsNullOrEmpty(searchOption.Search))
            {
                pagedData.Data = await employeeRepository.GetAll();
            }
            else
            {
                pagedData.Data = await employeeRepository.GetAll(x =>
                    x.Name.Contains(searchOption.Search) ||
                    x.Phone.Contains(searchOption.Search) ||
                    x.Email.Contains(searchOption.Search)
                );
            }

            pagedData.TotalData = pagedData.Data.Count;

            if (searchOption.PageIndex.HasValue)
            {
                pagedData.Data = pagedData.Data
                    .Skip(searchOption.PageIndex.Value * searchOption.PageSize!.Value)
                    .Take(searchOption.PageSize.Value)
                    .ToList();
            }

            return Ok(pagedData);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetEmployeeList([FromRoute] int id)
        {
            return Ok(await employeeRepository.FindByIdAsync(id));
        }

        // ✅ FIXED ADD EMPLOYEE
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee model)
        {
            // 1️⃣ Check if user already exists
            var existingUser = (await userRepo.GetAll(x => x.Email == model.Email))
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault();

            User user;

            if (existingUser == null)
            {
                // 2️⃣ Create user only if not exists
                user = new User
                {
                    Email = model.Email,
                    Role = "Employee",
                    Password = new PasswordHelper().HashPassword("12345"),
                    ProfileImage = ""
                };

                await userRepo.AddAsync(user);
                await userRepo.SaveChangesAsync();
            }
            else
            {
                user = existingUser;
            }

            // 3️⃣ Link employee to correct user
            model.UserId = user.Id;

            await employeeRepository.AddAsync(model);
            await employeeRepository.SaveChangesAsync();

            return Ok();
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] Employee model)
        {
            var employee = await employeeRepository.FindByIdAsync(id);

            employee.Name = model.Name;
            employee.Phone = model.Phone;
            employee.DepartmentId = model.DepartmentId;
            employee.LastWorkingDate = model.LastWorkingDate;
            employee.JobTitle = model.JobTitle;
            employee.Salary = model.Salary;

            employeeRepository.Update(employee);
            await employeeRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            await employeeRepository.DeleteAsync(id);
            await employeeRepository.SaveChangesAsync();
            return Ok();
        }
    }
}
