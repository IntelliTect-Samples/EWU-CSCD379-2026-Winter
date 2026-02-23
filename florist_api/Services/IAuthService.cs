using florist_api.DTOs;
using florist_api.Models;
using Microsoft.AspNetCore.Identity;

namespace florist_api.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest model);
        Task<IdentityResult> RegisterEmployeeAsync(EmployeeCreateRequest model);
    }
}