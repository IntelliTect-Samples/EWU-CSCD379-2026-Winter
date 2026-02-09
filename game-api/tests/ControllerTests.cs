using Xunit;
using Microsoft.AspNetCore.Mvc;
using game_api.Model;

namespace game_api_tests;

public class ScoreControllerTests {
    [Fact]
    public void ScoreController_Exists_And_CanBeCreated() {
    
        var controllerType = typeof(ScoreController);
        
        Assert.NotNull(controllerType);
        Assert.Equal("ScoreController", controllerType.Name);
    }

    [Fact]
    public void GetScores_Endpoint_HasHttpGetAttribute() {
        var method = typeof(ScoreController).GetMethod("GetScores");
        var attribute = method?.GetCustomAttributes(typeof(HttpGetAttribute), false);

        Assert.NotNull(attribute);
    }

    [Fact]
    public void PostScore_Endpoint_HasHttpPostAttribute() {
        var method = typeof(ScoreController).GetMethod("PostScore");
        var attribute = method?.GetCustomAttributes(typeof(HttpPostAttribute), false);

        Assert.NotNull(attribute);
    }
}