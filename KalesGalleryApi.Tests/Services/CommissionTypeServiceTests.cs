using KalesGalleryApi.Data;
using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Tests.Services;

public class CommissionTypeServiceTests
{
    private static GalleryDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<GalleryDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new GalleryDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTypes()
    {
        var dbName = Guid.NewGuid().ToString();
        using (var ctx = CreateContext(dbName))
        {
            ctx.CommissionTypes.AddRange(
                new CommissionType { Medium = "Digital", Price = 10m },
                new CommissionType { Medium = "Traditional", Price = 5m });
            await ctx.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new CommissionTypeService(ctx);
            var result = await service.GetAllAsync();
            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsType()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            var type = new CommissionType { Medium = "Digital", Price = 10m };
            ctx.CommissionTypes.Add(type);
            await ctx.SaveChangesAsync();
            id = type.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new CommissionTypeService(ctx);
            var result = await service.GetByIdAsync(id);
            Assert.NotNull(result);
            Assert.Equal("Digital", result.Medium);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new CommissionTypeService(ctx);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsAndReturnsType()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new CommissionTypeService(ctx);

        var type = new CommissionType { Medium = "Beadwork", Price = 15m };
        var result = await service.CreateAsync(type);

        Assert.True(result.Id > 0);
        Assert.Equal("Beadwork", result.Medium);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_UpdatesFields()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            var type = new CommissionType { Medium = "Old", Price = 5m };
            ctx.CommissionTypes.Add(type);
            await ctx.SaveChangesAsync();
            id = type.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new CommissionTypeService(ctx);
            var updated = new CommissionType { Medium = "Updated", Price = 25m };
            var result = await service.UpdateAsync(id, updated);

            Assert.NotNull(result);
            Assert.Equal("Updated", result!.Medium);
            Assert.Equal(25m, result.Price);
        }
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ReturnsNull()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new CommissionTypeService(ctx);

        var result = await service.UpdateAsync(999, new CommissionType { Medium = "X" });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ReturnsTrue()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            var type = new CommissionType { Medium = "ToDelete", Price = 5m };
            ctx.CommissionTypes.Add(type);
            await ctx.SaveChangesAsync();
            id = type.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new CommissionTypeService(ctx);
            var result = await service.DeleteAsync(id);

            Assert.True(result);
            Assert.Equal(0, await ctx.CommissionTypes.CountAsync());
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ReturnsFalse()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new CommissionTypeService(ctx);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }
}
