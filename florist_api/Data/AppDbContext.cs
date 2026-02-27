using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using florist_api.Models;

namespace florist_api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<Bouquet> Bouquets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Employee", NormalizedName = "EMPLOYEE" },
                new IdentityRole { Id = "3", Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            var hasher = new PasswordHasher<ApplicationUser>();

            // 2. Users
            var adminUser = new ApplicationUser
            {
                Id = "101",
                UserName = "head_gardener",
                NormalizedUserName = "HEAD_GARDENER",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "PetalPass123!");

            var employeeUser = new ApplicationUser
            {
                Id = "102",
                UserName = "lily_staff",
                NormalizedUserName = "LILY_STAFF",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            employeeUser.PasswordHash = hasher.HashPassword(employeeUser, "FlowerPower456!");

            var customerUser = new ApplicationUser
            {
                Id = "103",
                UserName = "rose_buyer",
                NormalizedUserName = "ROSE_BUYER",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            customerUser.PasswordHash = hasher.HashPassword(customerUser, "GardenGuest789!");

            // 3. Seed Users
            builder.Entity<ApplicationUser>().HasData(adminUser, employeeUser, customerUser);

            // 4. Link Users to Roles
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "101", RoleId = "1" },
                new IdentityUserRole<string> { UserId = "102", RoleId = "2" },
                new IdentityUserRole<string> { UserId = "103", RoleId = "3" }
            );
        }
    }
}