using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalonManagementService.Api.Models;

namespace SalonManagementService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserManager<User> userManager) : ControllerBase
{
    [HttpGet("Me")]
    [Authorize]
    public async Task<ActionResult> Me()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var roles = await userManager.GetRolesAsync(user);

        return Ok(new
        {
            email = user.Email,
            roles
        });
    }
}
