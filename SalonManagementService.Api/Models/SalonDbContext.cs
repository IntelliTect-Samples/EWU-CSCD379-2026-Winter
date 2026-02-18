using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SalonManagementService.Api.Models;

public class SalonDbContext(DbContextOptions<SalonDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Stylist> Stylists => Set<Stylist>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<StylistService> StylistServices => Set<StylistService>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentStylistService> AppointmentStylistServices => Set<AppointmentStylistService>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
