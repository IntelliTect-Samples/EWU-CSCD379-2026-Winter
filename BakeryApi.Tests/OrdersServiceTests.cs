using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BakeryApi.Controllers;
using BakeryApi.Models;
using BakeryApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BakeryApi.Tests;

public class OrdersControllerTests
{
    [Fact]
    public async Task GetOrders_WhenNoOrders_ReturnsEmptyList()
    {
        // Arrange
        var serviceMock = new Mock<IOrderService>();
        serviceMock.Setup(s => s.GetAllOrdersAsync())
            .ReturnsAsync(new List<Order>());

        var controller = new OrdersController(serviceMock.Object);

        // Act
        var result = await controller.GetOrders() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<Order>>(result.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task CreateOrder_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var serviceMock = new Mock<IOrderService>();
        var controller = new OrdersController(serviceMock.Object);
        controller.ModelState.AddModelError("", "invalid");

        // Act
        var result = await controller.CreateOrder(new Order());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateOrder_Valid_AddsAndReturnsOk()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "A",
            CustomerEmail = "a@b.com",
            CustomerPhone = "123-456-7890",
            Status = "Pending",
            TotalAmount = 10m
        };

        var serviceMock = new Mock<IOrderService>();
        serviceMock.Setup(s => s.CreateOrderAsync(order))
            .ReturnsAsync(order);

        var controller = new OrdersController(serviceMock.Object);

        // Act
        var result = await controller.CreateOrder(order) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returned = Assert.IsType<Order>(result.Value);
        Assert.Equal("A", returned.CustomerName);
    }

    [Fact]
    public async Task UpdateStatus_NonExisting_ReturnsNotFound()
    {
        // Arrange
        var serviceMock = new Mock<IOrderService>();
        serviceMock.Setup(s => s.UpdateStatusAsync(999, "Completed"))
            .ReturnsAsync((Order?)null);

        var controller = new OrdersController(serviceMock.Object);

        // Act
        var result = await controller.UpdateStatus(
            999,
            new OrdersController.StatusUpdateDto { Status = "Completed" }
        );

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}