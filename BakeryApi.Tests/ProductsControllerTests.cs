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

public class ProductsControllerTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void GetProducts_WhenNoProducts_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var controller = new ProductsController(context);

        // Act
        var result = controller.GetProducts() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<Product>>(result.Value);
        Assert.Empty(list);
    }

    [Fact]
    public void AddProduct_ValidProduct_ReturnsOkAndAdds()
    {
        // Arrange
        using var context = CreateContext();
        var controller = new ProductsController(context);
        var product = new Product { Name = "Test", Description = "d", Price = 1.23m };

        // Act
        var result = controller.AddProduct(product) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returned = Assert.IsType<Product>(result.Value);
        Assert.Equal("Test", returned.Name);
        Assert.Equal(1, context.Products.Count());
    }

    [Fact]
    public async Task UploadImage_NullFile_ReturnsBadRequest()
    {
        // Arrange
        using var context = CreateContext();
        var controller = new ProductsController(context);

        // Act
        var result = await controller.UploadImage(null) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void DeleteProduct_NonExisting_ReturnsNotFound()
    {
        // Arrange
        using var context = CreateContext();
        var controller = new ProductsController(context);

        // Act
        var result = controller.DeleteProduct(123);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
