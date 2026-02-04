using game_api.Data;
using game_api.Model;
using Microsoft.EntityFrameworkCore;

namespace game_api.Services {
    public class ScoreService : IScoreService {
        private readonly GameDBContext _context;

        public ScoreService(GameDBContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Score>> GetAllScoresAsync() {
            return await _context.Scores
                .OrderByDescending(s => s.DateAchieved)
                .ToListAsync();
        }

        public async Task<Score> AddScoreAsync(Score score) {
            score.DateAchieved = DateTime.UtcNow;
            _context.Scores.Add(score);
            await _context.SaveChangesAsync();
            return score;
        }
    }
}