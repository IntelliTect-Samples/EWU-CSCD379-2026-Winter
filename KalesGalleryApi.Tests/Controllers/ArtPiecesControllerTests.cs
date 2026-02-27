using KalesGalleryApi.Controllers;
using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KalesGalleryApi.Tests.Controllers;

public class ArtPiecesControllerTests
{
    private readonly Mock<IArtPieceService> _artPieceServiceMock;
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
    private readonly ArtPiecesController _controller;

    public ArtPiecesControllerTests()
    {
        _artPieceServiceMock = new Mock<IArtPieceService>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _controller = new ArtPiecesController(_artPieceServiceMock.Object, _blobStorageServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithArtPieces()
    {
        var artPieces = new List<ArtPiece>
        {
            new() { Id = 1, Name = "Sunset", Price = 100m },
            new() { Id = 2, Name = "Sunrise", Price = 200m }
        };
        _artPieceServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(artPieces);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<ArtPiece>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        var artPiece = new ArtPiece { Id = 1, Name = "Sunset", Price = 100m };
        _artPieceServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(artPiece);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<ArtPiece>(okResult.Value);
        Assert.Equal("Sunset", returned.Name);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNotFound()
    {
        _artPieceServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ArtPiece?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_WhenExists_DeletesBlobAndReturnsNoContent()
    {
        var artPiece = new ArtPiece
        {
            Id = 1,
            Name = "Sunset",
            ImageUrl = "https://storage.blob.core.windows.net/art/image.jpg"
        };
        _artPieceServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(artPiece);
        _blobStorageServiceMock.Setup(s => s.DeleteAsync("image.jpg")).ReturnsAsync(true);
        _artPieceServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        _blobStorageServiceMock.Verify(s => s.DeleteAsync("image.jpg"), Times.Once);
        _artPieceServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenNotExists_ReturnsNotFound()
    {
        _artPieceServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ArtPiece?)null);

        var result = await _controller.Delete(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNoImageUrl_SkipsBlobDelete()
    {
        var artPiece = new ArtPiece { Id = 1, Name = "Sunset", ImageUrl = "" };
        _artPieceServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(artPiece);
        _artPieceServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        _blobStorageServiceMock.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Never);
    }
}
