using System.ComponentModel.DataAnnotations;

namespace SalonManagementService.Api.Dtos;

public class StylistDto
{
    public Guid? StylistId { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    [Required]
    public required string PhoneNumber { get; set; }
    
    [Required]
    public required string ChairName { get; set; }
    
    public decimal WorkStartTime24H { get; set; }
    
    public decimal WorkEndTime24H { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public bool IsActive { get; set; } = true;
}
