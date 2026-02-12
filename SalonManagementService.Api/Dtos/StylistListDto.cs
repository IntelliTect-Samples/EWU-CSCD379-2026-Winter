namespace SalonManagementService.Api.Dtos;

public class StylistListDto
{
    public Guid StylistId { get; set; }
    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
}
