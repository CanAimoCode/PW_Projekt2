using Microsoft.EntityFrameworkCore;
using GameCatalogApp.Interfaces;
using GameCatalogApp.Models;

namespace GameCatalogApp.Data
{
    public class VideoGameRepositorySql : IVideoGameRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoGameRepositorySql(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoGame>> GetAllAsync()
        {
            return await _context.VideoGames
                .Include(g => g.Studio)
                .ToListAsync();
        }

        public async Task<VideoGame?> GetByIdAsync(int id)
        {
            return await _context.VideoGames
                .Include(g => g.Studio)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task AddAsync(VideoGame videoGame)
        {
            _context.VideoGames.Add(videoGame);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VideoGame videoGame)
        {
            var existingGame = await _context.VideoGames.FindAsync(videoGame.Id);

            if (existingGame == null)
                return;

            existingGame.Title = videoGame.Title;
            existingGame.Genre = videoGame.Genre;
            existingGame.ReleaseYear = videoGame.ReleaseYear;
            existingGame.Rating = videoGame.Rating;
            existingGame.Description = videoGame.Description;
            existingGame.StudioId = videoGame.StudioId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var videoGame = await _context.VideoGames.FindAsync(id);

            if (videoGame != null)
            {
                _context.VideoGames.Remove(videoGame);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.VideoGames.AnyAsync(g => g.Id == id);
        }
    }
}
