using Microsoft.EntityFrameworkCore;
using florist_api.Data;
using florist_api.Models;
using florist_api.Services;
using florist_api.DTOs;
using Xunit;
using System;
using System.Threading.Tasks;

namespace florist_api.Tests
{
    public class BouquetServiceTests
    {
        private AppDbContext GetDbContext()
        {
            // Creates a fresh, unique in-memory database for every test
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task UpdateInventoryAsync_ShouldUpdateCount_WhenBouquetExists()
        {
            // ARRANGE
            using var context = GetDbContext();
            var service = new BouquetService(context);
            
            var bouquet = new Bouquet { 
                Id = 1, 
                Name = "Red Roses", 
                Season = "All", 
                Price = 10.00m, 
                InventoryCount = 5,
                ImageUrl = "test.jpg" 
            };
            context.Bouquets.Add(bouquet);
            await context.SaveChangesAsync();

            // ACT
            var result = await service.UpdateInventoryAsync(1, 20);
            var updatedBouquet = await context.Bouquets.FindAsync(1);

            // ASSERT
            Assert.True(result);
            Assert.Equal(20, updatedBouquet!.InventoryCount);
        }

        [Fact]
        public async Task CreateBouquetAsync_ShouldSetDefaultInventoryToZero()
        {
            // ARRANGE
            using var context = GetDbContext();
            var service = new BouquetService(context);
            var dto = new BouquetCreateRequest { 
                Name = "Tulip Joy", 
                Price = 15.00m, 
                Season = "Spring",
                ImageUrl = "test.jpg" 
            };

            // ACT
            var result = await service.CreateBouquetAsync(dto);

            // ASSERT
            Assert.Equal(0, result.InventoryCount);
            Assert.Equal("Tulip Joy", result.Name);
        }
    }
}