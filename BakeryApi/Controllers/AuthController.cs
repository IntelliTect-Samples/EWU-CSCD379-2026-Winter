using Microsoft.AspNetCore.Mvc;
using BakeryApi.Models.Auth;
using BakeryApi.Services;

namespace BakeryApi.Controllers
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
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (token, roles) = await _authService.LoginAsync(dto);

            if (token == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                accessToken = token,
                roles = roles
            });
        }
    }
}