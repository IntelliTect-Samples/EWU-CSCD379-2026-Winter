using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using florist_api.DTOs;

namespace florist_api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/users")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserManagementController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/admin/users/employees
        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<IdentityUser>>> GetEmployees()
        {
            // Gets all users who are in the "Employee" role
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            return Ok(employees);
        }

        // POST: api/admin/users/add-employee
        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployee([FromBody] RegisterRequest model)
        {
            var user = new IdentityUser { UserName = model.Username, Email = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Employee");
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