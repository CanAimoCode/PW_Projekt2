using Microsoft.EntityFrameworkCore;
using GameCatalogApp.Models;

namespace GameCatalogApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<VideoGame> VideoGames { get; set; } = null!;

        public DbSet<Studio> Studios { get; set; } = null!;
    }
}
