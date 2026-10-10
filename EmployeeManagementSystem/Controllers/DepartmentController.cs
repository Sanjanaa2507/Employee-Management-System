using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Entity;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IRepository<Department> departmentRepository;

        public DepartmentController(IRepository<Department> departmentRepository)
        {
            this.departmentRepository = departmentRepository;
        }

        [HttpPost]
       [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDepartment([FromBody] Department model)
        {
            await departmentRepository.AddAsync(model);
            await departmentRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
       [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDepartment([FromRoute] int id, [FromBody] Department model)
        {
            var department = await departmentRepository.FindByIdAsync(id);
            department.Name = model.Name;
            departmentRepository.Update(department);
            await departmentRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
       [Authorize]
        public async Task<IActionResult> GetAllDepartment([FromQuery] SearchOptions options)
        {
            var list = await departmentRepository.GetAll();
            var pagedDate = new PagedData<Department>();
            pagedDate.TotalData = list.Count;
            if (options.PageIndex.HasValue)
            {
                pagedDate.Data = list.Skip(options!.PageIndex!.Value * options!.PageSize!.Value)
                                        .Take(options.PageSize.Value).ToList();
            }
            else
            {
                pagedDate.Data = list;
            }
            return Ok(pagedDate);        
        }

        [HttpDelete("{id}")]
       [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDepartment([FromRoute] int id)
        {
            await departmentRepository.DeleteAsync(id);
            await departmentRepository.SaveChangesAsync();
            return Ok();
        }
    }
}
