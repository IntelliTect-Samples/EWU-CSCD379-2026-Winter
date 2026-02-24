using Microsoft.AspNetCore.Identity;

namespace SalonManagementService.Api.Models;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty; // Todo: fix
    public DateTime DateOfBirth { get; set; }
}