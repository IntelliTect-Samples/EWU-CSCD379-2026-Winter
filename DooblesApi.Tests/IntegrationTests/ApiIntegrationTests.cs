using System.Net;
using System.Net.Http.Json;
using DooblesApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DooblesApi.Tests.IntegrationTests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DooblesDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<DooblesDbContext>(options =>
                {
                options.UseInMemoryDatabase("TestDb");
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task RootEndpoint_ReturnsDooblesApi()
    {
        // Act
        var response = await _client.GetStringAsync("/");

        // Assert
        Assert.Equal("DooblesApi", response);
    }

    [Fact]
    public async Task GetDoobledName_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/dooble/dooblename");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllNames_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/dooble/all");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var names = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(names);
        Assert.NotEmpty(names);
    }

    [Fact]
    public async Task PostReview_CreatesReview()
    {
        // Arrange
        var review = new Review
        {
            Stars = 5,
            ReviewText = "Integration test review",
            Reviewer = "IntegrationTester"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/review", review);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
