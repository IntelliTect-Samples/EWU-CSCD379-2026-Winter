using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BakeryApi.Controllers;
using BakeryApi.Models;
using BakeryApi.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BakeryApi.Tests;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetProducts_WhenNoProducts_ReturnsEmptyList()
    {
        // Arrange
        var serviceMock = new Mock<IProductService>();
        serviceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<Product>());

        var controller = new ProductsController(serviceMock.Object);

        // Act
        var result = await controller.GetProducts() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<Product>>(result.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task AddProduct_ValidProduct_ReturnsOk()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test",
            Description = "d",
            Price = 1.23m
        };

        var serviceMock = new Mock<IProductService>();
        serviceMock.Setup(s => s.AddAsync(product))
            .ReturnsAsync(product);

        var controller = new ProductsController(serviceMock.Object);

        // Act
        var result = await controller.AddProduct(product) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returned = Assert.IsType<Product>(result.Value);
        Assert.Equal("Test", returned.Name);
    }

    [Fact]
    public async Task DeleteProduct_NonExisting_ReturnsNotFound()
    {
        // Arrange
        var serviceMock = new Mock<IProductService>();
        serviceMock.Setup(s => s.DeleteAsync(123))
            .ReturnsAsync(false);

        var controller = new ProductsController(serviceMock.Object);

        // Act
        var result = await controller.DeleteProduct(123);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}