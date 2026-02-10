using Microsoft.EntityFrameworkCore;

namespace SalonManagementService.Api.Models;

public class StylistService
{
    public Guid StylistServiceId { get; set; }
    public Guid StylistId { get; set; }
    public Stylist Stylist { get; set; } = null!;
    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public int DurationInMinutes { get; set; }
    [Precision(18, 2)]
    public decimal Rate { get; set; }
    public ICollection<AppointmentStylistService> AppointmentStylistServices { get; set; } = [];
}
