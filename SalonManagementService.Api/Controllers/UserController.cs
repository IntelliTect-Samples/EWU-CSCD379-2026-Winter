using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonManagementService.Api.Dtos;
using SalonManagementService.Api.Models;

namespace SalonManagementService.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = Roles.Admin)]
public class UserController(UserManager<User> userManager) : ControllerBase
{
    [HttpGet("List")]
    public async Task<ActionResult<IEnumerable<UserDto>>> List()
    {
        var users = await userManager.Users.ToListAsync();

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            });
        }

        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> Get(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        var roles = await userManager.GetRolesAsync(user);

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        // Prevent deleting yourself
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser?.Id == user.Id)
        {
            return BadRequest("You cannot delete your own account.");
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        return NoContent();
    }
}
