using Microsoft.AspNetCore.Mvc;
using game_api.Model;
using game_api.Services;

namespace game_api.Controller {
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreController : ControllerBase {
        private readonly IScoreService _scoreService;

        public ScoreController(IScoreService scoreService) {
            _scoreService = scoreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Score>>> GetScores() {
            var scores = await _scoreService.GetAllScoresAsync();
            return Ok(scores);
        }

        [HttpPost]
        public async Task<ActionResult<Score>> PostScore(Score score) {
            var createdScore = await _scoreService.AddScoreAsync(score);
            return Ok(createdScore);
        }
    }
}