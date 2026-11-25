using System.Text.Json;
using GameCatalogApp.Interfaces;
using GameCatalogApp.Models;

namespace GameCatalogApp.Data
{
    public class StudioRepositoryFile : IStudioRepository
    {
        private readonly string _filePath;
        private readonly string _videoGamesFilePath;
        private static readonly SemaphoreSlim _lock = new(1, 1);
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public StudioRepositoryFile(IWebHostEnvironment environment)
        {
            string directory = Path.Combine(
                environment.ContentRootPath,
                "DataFiles"
            );

            Directory.CreateDirectory(directory);

            _filePath = Path.Combine(directory, "studios.json");
            _videoGamesFilePath = Path.Combine(directory, "videogames.json");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }

            if (!File.Exists(_videoGamesFilePath))
            {
                File.WriteAllText(_videoGamesFilePath, "[]");
            }
        }

        private async Task<List<Studio>> ReadUnlockedAsync()
        {
            string json = await File.ReadAllTextAsync(_filePath);

            return JsonSerializer.Deserialize<List<Studio>>(
                json,
                _jsonOptions
            ) ?? new List<Studio>();
        }

        private async Task<List<VideoGame>> ReadVideoGamesUnlockedAsync()
        {
            string json = await File.ReadAllTextAsync(_videoGamesFilePath);

            return JsonSerializer.Deserialize<List<VideoGame>>(
                json,
                _jsonOptions
            ) ?? new List<VideoGame>();
        }

        private async Task WriteUnlockedAsync(List<Studio> studios)
        {
            string json = JsonSerializer.Serialize(studios, _jsonOptions);

            await File.WriteAllTextAsync(_filePath, json);
        }

        private async Task WriteVideoGamesUnlockedAsync(
            List<VideoGame> videoGames)
        {
            string json = JsonSerializer.Serialize(videoGames, _jsonOptions);

            await File.WriteAllTextAsync(_videoGamesFilePath, json);
        }

        public async Task<List<Studio>> GetAllAsync()
        {
            await _lock.WaitAsync();

            try
            {
                var studios = await ReadUnlockedAsync();
                var videoGames = await ReadVideoGamesUnlockedAsync();

                foreach (var studio in studios)
                {
                    studio.VideoGames = videoGames
                        .Where(g => g.StudioId == studio.Id)
                        .ToList();
                }

                return studios;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<Studio?> GetByIdAsync(int id)
        {
            await _lock.WaitAsync();

            try
            {
                var studios = await ReadUnlockedAsync();

                return studios.FirstOrDefault(s => s.Id == id);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task AddAsync(Studio studio)
        {
            await _lock.WaitAsync();

            try
            {
                var studios = await ReadUnlockedAsync();

                studio.Id = studios.Count == 0
                    ? 1
                    : studios.Max(s => s.Id) + 1;

                studios.Add(studio);

                await WriteUnlockedAsync(studios);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task UpdateAsync(Studio studio)
        {
            await _lock.WaitAsync();

            try
            {
                var studios = await ReadUnlockedAsync();

                var existingStudio = studios.FirstOrDefault(
                    s => s.Id == studio.Id
                );

                if (existingStudio == null)
                    return;

                existingStudio.Name = studio.Name;
                existingStudio.Country = studio.Country;
                existingStudio.FoundedDate = studio.FoundedDate;
                existingStudio.Headquarters = studio.Headquarters;
                existingStudio.Description = studio.Description;

                await WriteUnlockedAsync(studios);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _lock.WaitAsync();

            try
            {
                var studios = await ReadUnlockedAsync();

                var studio = studios.FirstOrDefault(s => s.Id == id);

                if (studio == null)
                    return;

                var videoGames = await ReadVideoGamesUnlockedAsync();

                foreach (var videoGame in videoGames)
                {
                    if (videoGame.StudioId == id)
                    {
                        videoGame.StudioId = null;
                    }
                }

                await WriteVideoGamesUnlockedAsync(videoGames);

                studios.Remove(studio);

                await WriteUnlockedAsync(studios);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            await _lock.WaitAsync();

            try
            {
                var studios = await ReadUnlockedAsync();

                return studios.Any(s => s.Id == id);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
