using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using florist_api.DTOs;
using florist_api.Models;
using florist_api.Services;

namespace florist_api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/users")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;

        public UserManagementController(UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }

        // GET: api/admin/users/employees
        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            return Ok(employees.Select(e => new { e.Id, e.UserName, e.HireDate }));
        }

        // POST: api/admin/users/add-employee
        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeCreateRequest model)
        {
            var result = await _authService.RegisterEmployeeAsync(model);
            
            if (result.Succeeded)
            {
                return Ok(new { message = "Employee created successfully" });
            }
            return BadRequest(result.Errors);
        }

        // DELETE: api/admin/users/employee/5
        [HttpDelete("employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded) return NoContent();

            return BadRequest(result.Errors);
        }
    }
}