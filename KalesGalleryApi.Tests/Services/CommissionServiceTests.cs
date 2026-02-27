using KalesGalleryApi.Data;
using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Tests.Services;

public class CommissionServiceTests
{
    private static GalleryDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<GalleryDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new GalleryDbContext(options);
    }

    private static async Task SeedType(GalleryDbContext ctx)
    {
        if (!await ctx.CommissionTypes.AnyAsync())
        {
            ctx.CommissionTypes.Add(new CommissionType { Id = 100, Medium = "Digital", Price = 10m });
            await ctx.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCommissions()
    {
        var dbName = Guid.NewGuid().ToString();
        using (var ctx = CreateContext(dbName))
        {
            await SeedType(ctx);
            ctx.Commissions.AddRange(
                new Commission { Name = "A", UserId = "u1", TypeId = 100, Price = 10m },
                new Commission { Name = "B", UserId = "u2", TypeId = 100, Price = 10m });
            await ctx.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new CommissionService(ctx);
            var result = await service.GetAllAsync();
            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsCommission()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            await SeedType(ctx);
            var commission = new Commission { Name = "Portrait", UserId = "u1", TypeId = 100, Price = 10m };
            ctx.Commissions.Add(commission);
            await ctx.SaveChangesAsync();
            id = commission.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new CommissionService(ctx);
            var result = await service.GetByIdAsync(id);
            Assert.NotNull(result);
            Assert.Equal("Portrait", result.Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new CommissionService(ctx);

        Assert.Null(await service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsOnlyUserCommissions()
    {
        var dbName = Guid.NewGuid().ToString();
        using (var ctx = CreateContext(dbName))
        {
            await SeedType(ctx);
            ctx.Commissions.AddRange(
                new Commission { Name = "A", UserId = "u1", TypeId = 100, Price = 10m },
                new Commission { Name = "B", UserId = "u2", TypeId = 100, Price = 10m },
                new Commission { Name = "C", UserId = "u1", TypeId = 100, Price = 10m });
            await ctx.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new CommissionService(ctx);
            var result = await service.GetByUserIdAsync("u1");
            Assert.Equal(2, result.Count());
            Assert.All(result, c => Assert.Equal("u1", c.UserId));
        }
    }

    [Fact]
    public async Task CreateAsync_AddsCommission()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        await SeedType(ctx);
        var service = new CommissionService(ctx);

        var commission = new Commission { Name = "New", UserId = "u1", TypeId = 100, Price = 10m };
        var result = await service.CreateAsync(commission);

        Assert.True(result.Id > 0);
        Assert.Equal(1, await ctx.Commissions.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_UpdatesFields()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            await SeedType(ctx);
            var commission = new Commission { Name = "Old", UserId = "u1", TypeId = 100, Price = 10m };
            ctx.Commissions.Add(commission);
            await ctx.SaveChangesAsync();
            id = commission.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new CommissionService(ctx);
            var updated = new Commission { Name = "Updated", Description = "New desc", TypeId = 100, Price = 20m, IsCompleted = true };
            var result = await service.UpdateAsync(id, updated);

            Assert.NotNull(result);
            Assert.Equal("Updated", result!.Name);
            Assert.True(result.IsCompleted);
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ReturnsTrue()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            await SeedType(ctx);
            var commission = new Commission { Name = "Del", UserId = "u1", TypeId = 100, Price = 10m };
            ctx.Commissions.Add(commission);
            await ctx.SaveChangesAsync();
            id = commission.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new CommissionService(ctx);
            Assert.True(await service.DeleteAsync(id));
            Assert.Equal(0, await ctx.Commissions.CountAsync());
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ReturnsFalse()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new CommissionService(ctx);

        Assert.False(await service.DeleteAsync(999));
    }
}
