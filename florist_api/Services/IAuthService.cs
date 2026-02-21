using florist_api.DTOs;
using Microsoft.AspNetCore.Identity;

namespace florist_api.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginRequest model);
        Task<IdentityResult> RegisterEmployeeAsync(EmployeeCreateRequest model);
    }
}