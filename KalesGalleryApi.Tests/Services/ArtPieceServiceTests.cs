using KalesGalleryApi.Data;
using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Tests.Services;

public class ArtPieceServiceTests
{
    private static GalleryDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<GalleryDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new GalleryDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllArtPieces()
    {
        var dbName = Guid.NewGuid().ToString();
        using (var ctx = CreateContext(dbName))
        {
            ctx.ArtPieces.AddRange(
                new ArtPiece { Name = "A", Price = 10m },
                new ArtPiece { Name = "B", Price = 20m });
            await ctx.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new ArtPieceService(ctx);
            var result = await service.GetAllAsync();
            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsPiece()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            var piece = new ArtPiece { Name = "Sunset", Price = 100m };
            ctx.ArtPieces.Add(piece);
            await ctx.SaveChangesAsync();
            id = piece.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new ArtPieceService(ctx);
            var result = await service.GetByIdAsync(id);
            Assert.NotNull(result);
            Assert.Equal("Sunset", result.Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new ArtPieceService(ctx);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsAndReturnsPiece()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new ArtPieceService(ctx);

        var piece = new ArtPiece { Name = "New Piece", Description = "Desc", Price = 50m, IsAvailable = true };
        var result = await service.CreateAsync(piece);

        Assert.True(result.Id > 0);
        Assert.Equal("New Piece", result.Name);
        Assert.Equal(1, await ctx.ArtPieces.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_UpdatesFields()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            var piece = new ArtPiece { Name = "Old", Description = "Old", Price = 10m, IsAvailable = true };
            ctx.ArtPieces.Add(piece);
            await ctx.SaveChangesAsync();
            id = piece.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new ArtPieceService(ctx);
            var updated = new ArtPiece { Name = "New", Description = "New", Price = 99m, IsAvailable = false, ImageUrl = "url" };
            var result = await service.UpdateAsync(id, updated);

            Assert.NotNull(result);
            Assert.Equal("New", result!.Name);
            Assert.Equal(99m, result.Price);
            Assert.False(result.IsAvailable);
        }
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ReturnsNull()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new ArtPieceService(ctx);

        var result = await service.UpdateAsync(999, new ArtPiece { Name = "X" });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_RemovesAndReturnsTrue()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = CreateContext(dbName))
        {
            var piece = new ArtPiece { Name = "ToDelete", Price = 10m };
            ctx.ArtPieces.Add(piece);
            await ctx.SaveChangesAsync();
            id = piece.Id;
        }

        using (var ctx = CreateContext(dbName))
        {
            var service = new ArtPieceService(ctx);
            var result = await service.DeleteAsync(id);

            Assert.True(result);
            Assert.Equal(0, await ctx.ArtPieces.CountAsync());
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ReturnsFalse()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = CreateContext(dbName);
        var service = new ArtPieceService(ctx);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }
}
