using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using florist_api.Models;

namespace florist_api.Data
{
    // Inheriting from IdentityDbContext adds all the User/Role tables automatically
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Your specific table for the flower shop
        public DbSet<Bouquet> Bouquets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed initial Roles into Azure SQL
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().HasData(
                new Microsoft.AspNetCore.Identity.IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new Microsoft.AspNetCore.Identity.IdentityRole { Name = "Employee", NormalizedName = "EMPLOYEE" },
                new Microsoft.AspNetCore.Identity.IdentityRole { Name = "Customer", NormalizedName = "CUSTOMER" }
            );
        }
    }
}