using Microsoft.AspNetCore.Mvc;
using florist_api.Services;
using florist_api.DTOs;

namespace florist_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var result = await _authService.LoginAsync(model);
            
            if (result != null)
            {
                return Ok(result);
            }
            
            return Unauthorized("The garden gate remains locked. Invalid username or password.");
        }
    }
}