using BakeryApi.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace BakeryApi.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<(string? Token, IEnumerable<string>? Roles)> LoginAsync(LoginDto dto);
    }
}
