using BakeryApi.Models.Auth;

namespace BakeryApi.Services
{
    public interface IAuthService
    {
        Task<(string? Token, IEnumerable<string>? Roles)> LoginAsync(LoginDto dto);
    }
}