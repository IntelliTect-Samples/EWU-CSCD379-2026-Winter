using Microsoft.EntityFrameworkCore;

namespace SalonManagementService.Api.Models;

public class Appointment
{
    public Guid AppointmentId { get; set; }
    public Guid StylistId { get; set; }
    public Stylist Stylist { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public int DurationInMinutes { get; set; }
    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }
    public DateTime DatePaid { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerPhone { get; set; }
    public ICollection<AppointmentStylistService> AppointmentStylistServices { get; set; } = [];
}
