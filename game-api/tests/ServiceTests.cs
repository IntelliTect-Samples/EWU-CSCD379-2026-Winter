using Xunit;
using Microsoft.EntityFrameworkCore;
using game_api.Data;
using game_api.Model;
using game_api.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace game_api_tests;

public class ServiceTests
{
    private GameDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new GameDBContext(options);
    }

    [Fact]
    public async Task AddScoreAsync_ShouldSetCurrentDate()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ScoreService(context);
        var newScore = new Score { PlayerName = "Sam", Time = 12.5, Difficulty = "Hard" };

        // Act
        var result = await service.AddScoreAsync(newScore);

        // Assert
        Assert.True(result.DateAchieved > DateTime.UtcNow.AddMinutes(-1));
        Assert.Equal("Sam", result.PlayerName);
    }

    [Fact]
    public async Task GetAllScores_ShouldReturnResults()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ScoreService(context);
        
        context.Scores.Add(new Score { PlayerName = "Player 1", Time = 15.0, DateAchieved = DateTime.UtcNow.AddDays(-1) });
        context.Scores.Add(new Score { PlayerName = "Player 2", Time = 10.0, DateAchieved = DateTime.UtcNow });
        await context.SaveChangesAsync();

        // Act
        var scores = await service.GetAllScoresAsync();

        // Assert
        Assert.Equal(2, scores.Count());
        Assert.Equal("Player 2", scores.First().PlayerName);
    }

    [Fact]
    public async Task AddScoreAsync_SavesAllGameData() {
        var service = new ScoreService(GetDbContext());
        var score = new Score { 
            PlayerName = "Jimmy", 
            Time = 45.2, 
            Difficulty = "Hard" 
        };
        
        var result = await service.AddScoreAsync(score);
        
        Assert.Equal("Jimmy", result.PlayerName);
        Assert.Equal(45.2, result.Time);
        Assert.Equal("Hard", result.Difficulty);
        Assert.NotEqual(default, result.DateAchieved);
    }
}