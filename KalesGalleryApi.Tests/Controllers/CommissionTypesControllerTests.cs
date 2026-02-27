using KalesGalleryApi.Controllers;
using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KalesGalleryApi.Tests.Controllers;

public class CommissionTypesControllerTests
{
    private readonly Mock<ICommissionTypeService> _serviceMock;
    private readonly CommissionTypesController _controller;

    public CommissionTypesControllerTests()
    {
        _serviceMock = new Mock<ICommissionTypeService>();
        _controller = new CommissionTypesController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithCommissionTypes()
    {
        var types = new List<CommissionType>
        {
            new() { Id = 1, Medium = "Digital", Price = 10m },
            new() { Id = 2, Medium = "Traditional", Price = 5m }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(types);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<CommissionType>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        var type = new CommissionType { Id = 1, Medium = "Digital", Price = 10m };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(type);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<CommissionType>(okResult.Value);
        Assert.Equal("Digital", returned.Medium);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((CommissionType?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var type = new CommissionType { Id = 0, Medium = "Beadwork", Price = 15m };
        var created = new CommissionType { Id = 3, Medium = "Beadwork", Price = 15m };
        _serviceMock.Setup(s => s.CreateAsync(type)).ReturnsAsync(created);

        var result = await _controller.Create(type);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(CommissionTypesController.GetById), createdResult.ActionName);
        var returned = Assert.IsType<CommissionType>(createdResult.Value);
        Assert.Equal(3, returned.Id);
    }

    [Fact]
    public async Task Update_WhenExists_ReturnsOk()
    {
        var type = new CommissionType { Medium = "Updated", Price = 20m };
        var updated = new CommissionType { Id = 1, Medium = "Updated", Price = 20m };
        _serviceMock.Setup(s => s.UpdateAsync(1, type)).ReturnsAsync(updated);

        var result = await _controller.Update(1, type);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<CommissionType>(okResult.Value);
        Assert.Equal("Updated", returned.Medium);
    }

    [Fact]
    public async Task Update_WhenNotExists_ReturnsNotFound()
    {
        var type = new CommissionType { Medium = "Updated", Price = 20m };
        _serviceMock.Setup(s => s.UpdateAsync(99, type)).ReturnsAsync((CommissionType?)null);

        var result = await _controller.Update(99, type);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_WhenExists_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotExists_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        var result = await _controller.Delete(99);

        Assert.IsType<NotFoundResult>(result);
    }
}
