using KalesGalleryApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Data;

public class GalleryDbContext : IdentityDbContext<ApplicationUser>
{
    public GalleryDbContext(DbContextOptions<GalleryDbContext> options) : base(options)
    {
    }

    public DbSet<ArtPiece> ArtPieces { get; set; }
    public DbSet<Commission> Commissions { get; set; }
    public DbSet<CommissionType> CommissionTypes { get; set; }
    public DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure decimal precision for TotalPrice
        modelBuilder.Entity<Invoice>()
            .Property(i => i.TotalPrice)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<ArtPiece>()
            .Property(a => a.Price)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Commission>()
            .Property(c => c.Price)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<CommissionType>()
            .Property(ct => ct.Price)
            .HasColumnType("decimal(10,2)");

        // Configure relationships
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.User)
            .WithMany(u => u.Invoices)
            .HasForeignKey(i => i.UserId);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Commission)
            .WithOne(c => c.Invoice)
            .HasForeignKey<Invoice>(i => i.CommissionId);

        modelBuilder.Entity<Commission>()
            .HasOne(c => c.Type)
            .WithMany(ct => ct.Commissions)
            .HasForeignKey(c => c.TypeId);

        modelBuilder.Entity<Commission>()
            .HasOne(c => c.User)
            .WithMany(u => u.Commissions)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed CommissionTypes
        modelBuilder.Entity<CommissionType>().HasData(
            new CommissionType { Id = 1, Medium = "Digital", Price = 10.00m },
            new CommissionType { Id = 2, Medium = "Traditional", Price = 5.00m },
            new CommissionType { Id = 3, Medium = "Beadwork", Price = 15.00m }
        );
    }
}
