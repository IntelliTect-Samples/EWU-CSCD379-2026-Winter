using KalesGalleryApi.Data;
using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Tests.Services;

public class InvoiceServiceTests
{
    private static GalleryDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<GalleryDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new GalleryDbContext(options);
    }

    /// <summary>
    /// Seeds the required related entities (Users, CommissionType, Commissions) so Invoice FK constraints are satisfied.
    /// </summary>
    private static async Task SeedRelatedData(GalleryDbContext ctx, string[] userIds, params int[] commissionIds)
    {
        foreach (var uid in userIds)
        {
            if (!await ctx.Users.AnyAsync(u => u.Id == uid))
            {
                ctx.Users.Add(new ApplicationUser { Id = uid, UserName = $"{uid}@test.com", Email = $"{uid}@test.com" });
            }
        }
        if (!await ctx.CommissionTypes.AnyAsync(ct => ct.Id == 100))
        {
            ctx.CommissionTypes.Add(new CommissionType { Id = 100, Medium = "Digital", Price = 10m });
        }
        foreach (var cid in commissionIds)
        {
            if (!await ctx.Commissions.AnyAsync(c => c.Id == cid))
            {
                ctx.Commissions.Add(new Commission { Id = cid, Name = $"C{cid}", UserId = userIds[0], TypeId = 100, Price = 10m });
            }
        }
        await ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllInvoices()
    {
        var dbName = Guid.NewGuid().ToString();
        using (var ctx = CreateContext(dbName))
        {
            await SeedRelatedData(ctx, ["u1", "u2"], 1, 2);
            ctx.Invoices.AddRange(
                new Invoice { UserId = "u1", CommissionId = 1, TotalPrice = 10m },
                new Invoice { UserId = "u2", CommissionId = 2, TotalPrice = 20m });
            await ctx.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new InvoiceService(ctx);
            var result = await service.GetAllAsync();
            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsInvoice()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            await SeedRelatedData(ctx, ["u1"], 1);
            var invoice = new Invoice { UserId = "u1", CommissionId = 1, TotalPrice = 10m };
            ctx.Invoices.Add(invoice);
            await ctx.SaveChangesAsync();
            id = invoice.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new InvoiceService(ctx);
            var result = await service.GetByIdAsync(id);
            Assert.NotNull(result);
            Assert.Equal(10m, result.TotalPrice);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new InvoiceService(ctx);

        Assert.Null(await service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsOnlyUserInvoices()
    {
        var dbName = Guid.NewGuid().ToString();
        using (var ctx = CreateContext(dbName))
        {
            await SeedRelatedData(ctx, ["u1", "u2"], 1, 2, 3);
            ctx.Invoices.AddRange(
                new Invoice { UserId = "u1", CommissionId = 1, TotalPrice = 10m },
                new Invoice { UserId = "u2", CommissionId = 2, TotalPrice = 20m },
                new Invoice { UserId = "u1", CommissionId = 3, TotalPrice = 30m });
            await ctx.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new InvoiceService(ctx);
            var result = await service.GetByUserIdAsync("u1");
            Assert.Equal(2, result.Count());
            Assert.All(result, i => Assert.Equal("u1", i.UserId));
        }
    }

    [Fact]
    public async Task GetByCommissionIdAsync_WhenExists_ReturnsInvoice()
    {
        var dbName = Guid.NewGuid().ToString();
        using (var ctx = CreateContext(dbName))
        {
            await SeedRelatedData(ctx, ["u1"], 42);
            ctx.Invoices.Add(new Invoice { UserId = "u1", CommissionId = 42, TotalPrice = 10m });
            await ctx.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new InvoiceService(ctx);
            var result = await service.GetByCommissionIdAsync(42);
            Assert.NotNull(result);
            Assert.Equal(42, result.CommissionId);
        }
    }

    [Fact]
    public async Task GetByCommissionIdAsync_WhenNotExists_ReturnsNull()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new InvoiceService(ctx);

        Assert.Null(await service.GetByCommissionIdAsync(999));
    }

    [Fact]
    public async Task CreateAsync_AddsInvoice()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new InvoiceService(ctx);

        await SeedRelatedData(ctx, ["u1"], 1);
        var invoice = new Invoice { UserId = "u1", CommissionId = 1, TotalPrice = 50m };
        var result = await service.CreateAsync(invoice);

        Assert.True(result.Id > 0);
        Assert.Equal(1, await ctx.Invoices.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_UpdatesFields()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            await SeedRelatedData(ctx, ["u1", "u2"], 1, 2);
            var invoice = new Invoice { UserId = "u1", CommissionId = 1, TotalPrice = 10m };
            ctx.Invoices.Add(invoice);
            await ctx.SaveChangesAsync();
            id = invoice.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new InvoiceService(ctx);
            var updated = new Invoice { UserId = "u2", CommissionId = 2, TotalPrice = 99m };
            var result = await service.UpdateAsync(id, updated);

            Assert.NotNull(result);
            Assert.Equal("u2", result!.UserId);
            Assert.Equal(99m, result.TotalPrice);
        }
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ReturnsNull()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new InvoiceService(ctx);

        Assert.Null(await service.UpdateAsync(999, new Invoice()));
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ReturnsTrue()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            await SeedRelatedData(ctx, ["u1"], 1);
            var invoice = new Invoice { UserId = "u1", CommissionId = 1, TotalPrice = 10m };
            ctx.Invoices.Add(invoice);
            await ctx.SaveChangesAsync();
            id = invoice.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new InvoiceService(ctx);
            Assert.True(await service.DeleteAsync(id));
            Assert.Equal(0, await ctx.Invoices.CountAsync());
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ReturnsFalse()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new InvoiceService(ctx);

        Assert.False(await service.DeleteAsync(999));
    }
}
