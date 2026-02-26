using Xunit;
using Microsoft.AspNetCore.Mvc;
using BakeryApi.Controllers;
using BakeryApi.Data;
using BakeryApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace BakeryApi.Tests;

public class OrdersControllerTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void GetOrders_WhenNoOrders_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var controller = new OrdersController(context);

        // Act
        var result = controller.GetOrders() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<Order>>(result.Value);
        Assert.Empty(list);
    }

    [Fact]
    public void CreateOrder_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        using var context = CreateContext();
        var controller = new OrdersController(context);
        controller.ModelState.AddModelError("", "invalid");

        // Act
        var result = controller.CreateOrder(new Order());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateOrder_Valid_AddsAndReturnsOk()
    {
        // Arrange
        using var context = CreateContext();
        var controller = new OrdersController(context);
        var order = new Order { CustomerName = "A", CustomerEmail = "a@b.com", CustomerPhone = "123-456-7890", Status = "Pending", TotalAmount = 10m };

        // Act
        var result = controller.CreateOrder(order) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returned = Assert.IsType<Order>(result.Value);
        Assert.Equal("A", returned.CustomerName);
        Assert.Equal(1, context.Orders.Count());
    }

    [Fact]
    public void UpdateStatus_NonExisting_ReturnsNotFound()
    {
        // Arrange
        using var context = CreateContext();
        var controller = new OrdersController(context);

        // Act
        var result = controller.UpdateStatus(999, new OrdersController.StatusUpdateDto { Status = "Completed" });

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
