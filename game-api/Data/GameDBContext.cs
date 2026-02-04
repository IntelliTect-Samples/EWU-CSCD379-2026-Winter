using Microsoft.EntityFrameworkCore;
using game_api.Model;

namespace game_api.Data {
    public class GameDBContext : DbContext {
        public GameDBContext(DbContextOptions<GameDBContext> options) : base(options) { }

        public DbSet<Score> Scores { get; set; }
    }
}
