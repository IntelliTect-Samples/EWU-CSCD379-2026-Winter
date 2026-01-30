using Microsoft.EntityFrameworkCore;
using Wordle.Api.Models;

namespace Wordle.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Add DbSets for your entities here
         public DbSet<Word> Words { get; set; }
    }
}
