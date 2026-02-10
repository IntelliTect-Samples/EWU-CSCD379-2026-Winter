namespace SalonManagementService.Api.Models;

public class Service
{
    public Guid ServiceId { get; set; }
    public required string Name { get; set; }
    public ICollection<StylistService> StylistServices { get; set; } = [];
}
