using Xunit;
using game_api.Model;

namespace game_api_tests;

public class ModelTests
{
    [Theory]
    [InlineData("Easy")]
    [InlineData("Medium")]
    [InlineData("Hard")]
    public void ScoreModel_ShouldStoreSelectedDifficulty(string chosenDifficulty)
    {
        // Arrange
        var score = new Score { Difficulty = chosenDifficulty };

        // Act & Assert
        Assert.Equal(chosenDifficulty, score.Difficulty);
    }

    [Fact]
    public void Score_WithEmptyName_IsInvalidLogic()
    {
        // Arrange
        var score = new Score { PlayerName = "" };
        
        // Assert
        Assert.True(string.IsNullOrWhiteSpace(score.PlayerName));
    }

    [Fact]
    public void Score_WithValidName_StoresCorrectly()
    {
        // Arrange
        var score = new Score { PlayerName = "Billy" };

        // Assert
        Assert.Equal("Billy", score.PlayerName);
    }
}