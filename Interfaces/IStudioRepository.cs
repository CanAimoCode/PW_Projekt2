using GameCatalogApp.Models;

namespace GameCatalogApp.Interfaces
{
    public interface IStudioRepository
    {
        Task<List<Studio>> GetAllAsync();
        Task<Studio?> GetByIdAsync(int id);
        Task AddAsync(Studio studio);
        Task UpdateAsync(Studio studio);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
