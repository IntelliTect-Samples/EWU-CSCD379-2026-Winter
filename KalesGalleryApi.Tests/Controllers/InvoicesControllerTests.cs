using KalesGalleryApi.Controllers;
using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace KalesGalleryApi.Tests.Controllers;

public class InvoicesControllerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly InvoicesController _controller;

    public InvoicesControllerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _controller = new InvoicesController(_serviceMock.Object);
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
    public async Task GetAll_ReturnsOkWithInvoices()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = 1, UserId = "user1", TotalPrice = 10m },
            new() { Id = 2, UserId = "user2", TotalPrice = 20m }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(invoices);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<Invoice>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        var invoice = new Invoice { Id = 1, TotalPrice = 10m };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(invoice);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(10m, Assert.IsType<Invoice>(okResult.Value).TotalPrice);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Invoice?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetByUserId_ReturnsOk()
    {
        var invoices = new List<Invoice> { new() { Id = 1, UserId = "user1" } };
        _serviceMock.Setup(s => s.GetByUserIdAsync("user1")).ReturnsAsync(invoices);

        var result = await _controller.GetByUserId("user1");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<Invoice>>(okResult.Value));
    }

    [Fact]
    public async Task GetMyInvoices_ReturnsCurrentUserInvoices()
    {
        SetUser("user1");
        var invoices = new List<Invoice> { new() { Id = 1, UserId = "user1" } };
        _serviceMock.Setup(s => s.GetByUserIdAsync("user1")).ReturnsAsync(invoices);

        var result = await _controller.GetMyInvoices();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<Invoice>>(okResult.Value));
    }

    [Fact]
    public async Task GetByCommissionId_WhenExists_ReturnsOk()
    {
        var invoice = new Invoice { Id = 1, CommissionId = 5 };
        _serviceMock.Setup(s => s.GetByCommissionIdAsync(5)).ReturnsAsync(invoice);

        var result = await _controller.GetByCommissionId(5);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(5, Assert.IsType<Invoice>(okResult.Value).CommissionId);
    }

    [Fact]
    public async Task GetByCommissionId_WhenNotExists_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByCommissionIdAsync(99)).ReturnsAsync((Invoice?)null);

        var result = await _controller.GetByCommissionId(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var invoice = new Invoice { UserId = "user1", CommissionId = 1, TotalPrice = 10m };
        var created = new Invoice { Id = 1, UserId = "user1", CommissionId = 1, TotalPrice = 10m };
        _serviceMock.Setup(s => s.CreateAsync(invoice)).ReturnsAsync(created);

        var result = await _controller.Create(invoice);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(1, Assert.IsType<Invoice>(createdResult.Value).Id);
    }

    [Fact]
    public async Task Update_WhenExists_ReturnsOk()
    {
        var invoice = new Invoice { TotalPrice = 20m };
        var updated = new Invoice { Id = 1, TotalPrice = 20m };
        _serviceMock.Setup(s => s.UpdateAsync(1, invoice)).ReturnsAsync(updated);

        var result = await _controller.Update(1, invoice);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(20m, Assert.IsType<Invoice>(okResult.Value).TotalPrice);
    }

    [Fact]
    public async Task Update_WhenNotExists_ReturnsNotFound()
    {
        var invoice = new Invoice { TotalPrice = 20m };
        _serviceMock.Setup(s => s.UpdateAsync(99, invoice)).ReturnsAsync((Invoice?)null);

        var result = await _controller.Update(99, invoice);

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
