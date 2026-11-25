using GameCatalogApp.Models;

namespace GameCatalogApp.Interfaces
{
    public interface IVideoGameRepository
    {
        Task<List<VideoGame>> GetAllAsync();
        Task<VideoGame?> GetByIdAsync(int id);
        Task AddAsync(VideoGame videoGame);
        Task UpdateAsync(VideoGame videoGame);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
