using System.Net;
using System.Net.Http.Json;
using DooblesApi.Models;
using DooblesApi.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace DooblesApi.Tests.IntegrationTests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registrations
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DooblesDbContext>));

                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                services.AddDbContext<DooblesDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Build the service provider & seed data
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DooblesDbContext>();

                db.Database.EnsureCreated();

                // 🌱 Minimal seed data
                db.Doobles.Add(new Dooble
                {
                    Name = "TestDooble"
                });

                db.SaveChanges();
            });
        }).CreateClient();
    }

    [Fact]
    public async Task RootEndpoint_ReturnsDooblesApi()
    {
        var response = await _client.GetStringAsync("/");
        Assert.Equal("DooblesApi", response);
    }

    [Fact]
    public async Task GetDoobledName_ReturnsOk()
    {
        var response = await _client.GetAsync("/dooble/dooblename");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllNames_ReturnsOk()
    {
        var response = await _client.GetAsync("/dooble/all");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var names = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(names);
        Assert.NotEmpty(names);
    }

    [Fact]
    public async Task PostReview_CreatesReview()
    {
        var review = new Review
        {
            Stars = 5,
            ReviewText = "Integration test review",
            Reviewer = "IntegrationTester"
        };

        var response = await _client.PostAsJsonAsync("/review", review);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}

