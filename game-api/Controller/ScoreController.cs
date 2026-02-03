using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using game_api.Model;
using game_api.Data;

namespace game_api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreController : ControllerBase {
        
        private readonly GameDBContext _context;
        public ScoreController(GameDBContext context)
        {
            _context = context;
        }

        // GET: api/scores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Score>>> GetScores() {
            return await _context.Scores
                .OrderByDescending(s => s.DateAchieved)
                .ToListAsync();
        }

        // POST: api/scores
        [HttpPost]
        public async Task<ActionResult<Score>> PostScore(Score score) {
            score.DateAchieved = DateTime.UtcNow;
            _context.Scores.Add(score);
            await _context.SaveChangesAsync();
            return Ok(score);
        }
    }
}