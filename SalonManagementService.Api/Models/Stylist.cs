using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SalonManagementService.Api.Models;

public class Stylist
{
    public Guid StylistId { get; set; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string ChairName { get; set; }
    [Precision(18, 2)]
    public decimal WorkStartTime24H { get; set; }
    [Precision(18, 2)]
    public decimal WorkEndTime24H { get; set; }
    public byte[]? Image { get; set; } = null;
    public bool IsActive { get; set; } = true;
    public ICollection<StylistService> StylistServices { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
}
