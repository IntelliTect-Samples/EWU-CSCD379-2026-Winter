using SalonManagementService.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace SalonManagementService.Api;

public static class SalonSeeder
{
    public static async Task SeedData(SalonDbContext context, RoleManager<IdentityRole> roleManager)
    {
        // Seed roles first
        await SeedRoles(roleManager);

        if (context.Stylists.Any() || context.Services.Any() || context.Appointments.Any())
        {
            return;
        }

        var random = new Random();

        var services = new List<Service>
        {
            new Service { ServiceId = Guid.NewGuid(), Name = "Haircut" },
            new Service { ServiceId = Guid.NewGuid(), Name = "Hair Coloring" },
            new Service { ServiceId = Guid.NewGuid(), Name = "Blowout" },
            new Service { ServiceId = Guid.NewGuid(), Name = "Hair Extensions" },
            new Service { ServiceId = Guid.NewGuid(), Name = "Deep Conditioning" }
        };

        var stylists = new List<Stylist>
        {
            new Stylist
            {
                StylistId = Guid.NewGuid(),
                Name = "Emma Johnson",
                PhoneNumber = "(555) 123-4567",
                ChairName = "Chair A",
                WorkStartTime24H = 9.0m,
                WorkEndTime24H = 17.0m
            },
            new Stylist
            {
                StylistId = Guid.NewGuid(),
                Name = "Liam Smith",
                PhoneNumber = "(555) 234-5678",
                ChairName = "Chair B",
                WorkStartTime24H = 10.0m,
                WorkEndTime24H = 18.0m
            },
            new Stylist
            {
                StylistId = Guid.NewGuid(),
                Name = "Olivia Williams",
                PhoneNumber = "(555) 345-6789",
                ChairName = "Chair C",
                WorkStartTime24H = 8.0m,
                WorkEndTime24H = 16.0m
            },
            new Stylist
            {
                StylistId = Guid.NewGuid(),
                Name = "Noah Brown",
                PhoneNumber = "(555) 456-7890",
                ChairName = "Chair D",
                WorkStartTime24H = 11.0m,
                WorkEndTime24H = 19.0m
            },
            new Stylist
            {
                StylistId = Guid.NewGuid(),
                Name = "Ava Martinez",
                PhoneNumber = "(555) 567-8901",
                ChairName = "Chair E",
                WorkStartTime24H = 9.0m,
                WorkEndTime24H = 17.0m
            }
        };

        context.Services.AddRange(services);
        context.Stylists.AddRange(stylists);
        context.SaveChanges();

        var stylistServices = new List<StylistService>();
        var serviceRates = new Dictionary<string, decimal>
        {
            { "Haircut", 45.0m },
            { "Hair Coloring", 120.0m },
            { "Blowout", 55.0m },
            { "Hair Extensions", 250.0m },
            { "Deep Conditioning", 35.0m }
        };

        var serviceDurations = new Dictionary<string, int>
        {
            { "Haircut", 30 },
            { "Hair Coloring", 120 },
            { "Blowout", 45 },
            { "Hair Extensions", 180 },
            { "Deep Conditioning", 30 }
        };

        foreach (var stylist in stylists)
        {
            foreach (var service in services)
            {
                stylistServices.Add(new StylistService
                {
                    StylistServiceId = Guid.NewGuid(),
                    StylistId = stylist.StylistId,
                    ServiceId = service.ServiceId,
                    DurationInMinutes = serviceDurations[service.Name],
                    Rate = serviceRates[service.Name] + random.Next(-10, 11)
                });
            }
        }

        context.StylistServices.AddRange(stylistServices);
        context.SaveChanges();

        var customerNames = new[]
        {
            "Alice Cooper", "Bob Dylan", "Charlie Parker", "Diana Ross", "Elvis Presley",
            "Frank Sinatra", "Grace Kelly", "Henry Ford", "Iris Apfel", "Jack London",
            "Kate Winslet", "Leo DiCaprio", "Mary Poppins", "Nancy Drew", "Oscar Wilde",
            "Penny Lane", "Quinn Fabray", "Rachel Green", "Sam Winchester", "Tina Fey",
            "Uma Thurman", "Victor Hugo", "Wendy Darling", "Xavier Dolan", "Yoko Ono"
        };

        var appointments = new List<Appointment>();
        var appointmentStylistServices = new List<AppointmentStylistService>();

        for (int i = 0; i < 25; i++)
        {
            var stylist = stylists[random.Next(stylists.Count)];
            var appointmentDate = DateTime.Now.AddDays(random.Next(-30, 30)).Date
                .AddHours(random.Next(9, 17));

            var stylistServicesForStylist = stylistServices
                .Where(ss => ss.StylistId == stylist.StylistId)
                .ToList();

            var selectedStylistServices = stylistServicesForStylist
                .OrderBy(_ => random.Next())
                .Take(random.Next(1, 4))
                .ToList();

            var totalDuration = selectedStylistServices.Sum(ss => ss.DurationInMinutes);
            var totalPrice = selectedStylistServices.Sum(ss => ss.Rate);

            var appointment = new Appointment
            {
                AppointmentId = Guid.NewGuid(),
                StylistId = stylist.StylistId,
                DateTime = appointmentDate,
                DurationInMinutes = totalDuration,
                TotalPrice = totalPrice,
                DatePaid = random.Next(2) == 0 ? DateTime.MinValue : appointmentDate.AddDays(random.Next(-5, 0)),
                CustomerName = customerNames[i],
                CustomerPhone = $"(555) {random.Next(100, 999)}-{random.Next(1000, 9999)}"
            };

            appointments.Add(appointment);

            foreach (var stylistService in selectedStylistServices)
            {
                appointmentStylistServices.Add(new AppointmentStylistService
                {
                    AppointmentStylistServiceId = Guid.NewGuid(),
                    AppointmentId = appointment.AppointmentId,
                    StylistServiceId = stylistService.StylistServiceId
                });
            }
        }

        context.Appointments.AddRange(appointments);
        context.SaveChanges();

        context.AppointmentStylistServices.AddRange(appointmentStylistServices);
        context.SaveChanges();
    }

    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { Roles.Admin, Roles.Stylist, Roles.Customer };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
