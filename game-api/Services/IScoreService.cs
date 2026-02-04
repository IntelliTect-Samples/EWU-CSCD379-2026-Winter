using game_api.Model;

namespace game_api.Services {
    public interface IScoreService {
        Task<IEnumerable<Score>> GetAllScoresAsync();
        Task<Score> AddScoreAsync(Score score);
    }
}