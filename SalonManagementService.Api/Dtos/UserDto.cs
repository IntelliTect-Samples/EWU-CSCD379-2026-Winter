namespace SalonManagementService.Api.Dtos;

public class UserDto
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public List<string> Roles { get; set; } = [];
}
