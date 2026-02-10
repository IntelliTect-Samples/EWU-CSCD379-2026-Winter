namespace SalonManagementService.Api.Models;

public class AppointmentStylistService
{
    public Guid AppointmentStylistServiceId { get; set; }
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public Guid StylistServiceId { get; set; }
    public StylistService StylistService { get; set; } = null!;
}
