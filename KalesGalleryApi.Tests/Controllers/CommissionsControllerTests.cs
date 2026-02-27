using KalesGalleryApi.Controllers;
using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace KalesGalleryApi.Tests.Controllers;

public class CommissionsControllerTests
{
    private readonly Mock<ICommissionService> _commissionServiceMock;
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly Mock<ICommissionTypeService> _commissionTypeServiceMock;
    private readonly CommissionsController _controller;

    public CommissionsControllerTests()
    {
        _commissionServiceMock = new Mock<ICommissionService>();
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _commissionTypeServiceMock = new Mock<ICommissionTypeService>();
        _controller = new CommissionsController(
            _commissionServiceMock.Object,
            _invoiceServiceMock.Object,
            _commissionTypeServiceMock.Object);
    }

    private void SetUser(string userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithCommissions()
    {
        var commissions = new List<Commission>
        {
            new() { Id = 1, Name = "Portrait", UserId = "user1" },
            new() { Id = 2, Name = "Landscape", UserId = "user2" }
        };
        _commissionServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(commissions);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<Commission>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        var commission = new Commission { Id = 1, Name = "Portrait" };
        _commissionServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(commission);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("Portrait", Assert.IsType<Commission>(okResult.Value).Name);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNotFound()
    {
        _commissionServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Commission?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetMyCommissions_ReturnsUserCommissions()
    {
        SetUser("user1");
        var commissions = new List<Commission>
        {
            new() { Id = 1, Name = "Portrait", UserId = "user1" }
        };
        _commissionServiceMock.Setup(s => s.GetByUserIdAsync("user1")).ReturnsAsync(commissions);

        var result = await _controller.GetMyCommissions();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<Commission>>(okResult.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task Create_WithValidType_CreatesCommissionAndInvoice()
    {
        SetUser("user1");
        var commissionType = new CommissionType { Id = 1, Medium = "Digital", Price = 10m };
        _commissionTypeServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(commissionType);

        var commission = new Commission { Name = "Portrait", Description = "Test", TypeId = 1 };
        var created = new Commission { Id = 1, Name = "Portrait", Description = "Test", TypeId = 1, UserId = "user1", Price = 10m };
        _commissionServiceMock.Setup(s => s.CreateAsync(It.IsAny<Commission>())).ReturnsAsync(created);
        _invoiceServiceMock.Setup(s => s.CreateAsync(It.IsAny<Invoice>())).ReturnsAsync(new Invoice { Id = 1 });

        var result = await _controller.Create(commission);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.IsType<Commission>(createdResult.Value);
        _invoiceServiceMock.Verify(s => s.CreateAsync(It.Is<Invoice>(i =>
            i.UserId == "user1" && i.TotalPrice == 10m)), Times.Once);
    }

    [Fact]
    public async Task Create_WithInvalidType_ReturnsBadRequest()
    {
        SetUser("user1");
        _commissionTypeServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((CommissionType?)null);
        var commission = new Commission { Name = "Portrait", TypeId = 99 };

        var result = await _controller.Create(commission);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WhenExists_ReturnsOk()
    {
        var commission = new Commission { Name = "Updated" };
        var updated = new Commission { Id = 1, Name = "Updated" };
        _commissionServiceMock.Setup(s => s.UpdateAsync(1, commission)).ReturnsAsync(updated);

        var result = await _controller.Update(1, commission);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("Updated", Assert.IsType<Commission>(okResult.Value).Name);
    }

    [Fact]
    public async Task Update_WhenNotExists_ReturnsNotFound()
    {
        var commission = new Commission { Name = "Updated" };
        _commissionServiceMock.Setup(s => s.UpdateAsync(99, commission)).ReturnsAsync((Commission?)null);

        var result = await _controller.Update(99, commission);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_WhenExists_ReturnsNoContent()
    {
        _commissionServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotExists_ReturnsNotFound()
    {
        _commissionServiceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        var result = await _controller.Delete(99);

        Assert.IsType<NotFoundResult>(result);
    }
}
