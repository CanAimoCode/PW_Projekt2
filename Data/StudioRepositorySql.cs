using Microsoft.EntityFrameworkCore;
using GameCatalogApp.Interfaces;
using GameCatalogApp.Models;

namespace GameCatalogApp.Data
{
    public class StudioRepositorySql : IStudioRepository
    {
        private readonly ApplicationDbContext _context;

        public StudioRepositorySql(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Studio>> GetAllAsync()
        {
            return await _context.Studios
                .Include(s => s.VideoGames)
                .ToListAsync();
        }

        public async Task<Studio?> GetByIdAsync(int id)
        {
            return await _context.Studios
                .Include(s => s.VideoGames)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Studio studio)
        {
            _context.Studios.Add(studio);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Studio studio)
        {
            var existingStudio = await _context.Studios.FindAsync(studio.Id);

            if (existingStudio == null)
                return;

            existingStudio.Name = studio.Name;
            existingStudio.Country = studio.Country;
            existingStudio.FoundedDate = studio.FoundedDate;
            existingStudio.Headquarters = studio.Headquarters;
            existingStudio.Description = studio.Description;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var studio = await _context.Studios
                .Include(s => s.VideoGames)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studio == null)
                return;

            foreach (var videoGame in studio.VideoGames)
            {
                videoGame.StudioId = null;
            }

            _context.Studios.Remove(studio);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Studios.AnyAsync(s => s.Id == id);
        }
    }
}
