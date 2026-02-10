using DooblesApi.Controllers;
using DooblesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DooblesApi.Tests.Controllers;

public class DoobleControllerTests : TestBase
{
    private readonly DoobleController _controller;
    private readonly IDoobleService _service;

    public DoobleControllerTests()
    {
        _service = new DoobleService(_context);
     _controller = new DoobleController(_service);
    }

    [Fact]
    public async Task GetDoobledName_ReturnsOk_WhenNamesExist()
    {
        // Arrange - seed data is already in the database from migration

        // Act
        var result = await _controller.GetDoobledName();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
        Assert.IsType<string>(okResult.Value);
    }

    [Fact]
    public async Task GetDoobledName_ReturnsNotFound_WhenNoNamesExist()
    {
        // Arrange - clear all names
        _context.DoobledNames.RemoveRange(_context.DoobledNames);
        await _context.SaveChangesAsync();

      // Act
var result = await _controller.GetDoobledName();

 // Assert
      Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllNames_ReturnsAllNames()
 {
        // Act
    var result = await _controller.GetAllNames();

     // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var names = Assert.IsType<List<string>>(okResult.Value);
        Assert.NotEmpty(names);
    }

    [Fact]
    public async Task GetDoobledName_ReturnsRandomName_FromDatabase()
    {
        // Act - call multiple times to verify randomness
        var results = new HashSet<string>();
        for (int i = 0; i < 10; i++)
        {
       var result = await _controller.GetDoobledName();
            var okResult = result.Result as OkObjectResult;
    if (okResult?.Value is string name)
          {
          results.Add(name);
   }
    }

  // Assert - should have gotten at least 2 different names to verify randomness
        Assert.True(results.Count >= 2, "Should get at least 2 different names in 10 tries");
    }
}