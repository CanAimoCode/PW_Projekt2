using System.Text.Json;
using GameCatalogApp.Interfaces;
using GameCatalogApp.Models;

namespace GameCatalogApp.Data
{
    public class VideoGameRepositoryFile : IVideoGameRepository
    {
        private readonly string _filePath;
        private readonly string _studiosFilePath;
        private static readonly SemaphoreSlim _lock = new(1, 1);
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public VideoGameRepositoryFile(IWebHostEnvironment environment)
        {
            string directory = Path.Combine(
                environment.ContentRootPath,
                "DataFiles"
            );

            Directory.CreateDirectory(directory);
            _filePath = Path.Combine(directory, "videogames.json");
            _studiosFilePath = Path.Combine(directory, "studios.json");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }

            if (!File.Exists(_studiosFilePath))
            {
                File.WriteAllText(_studiosFilePath, "[]");
            }
        }

        private async Task<List<VideoGame>> ReadUnlockedAsync()
        {
            string json = await File.ReadAllTextAsync(_filePath);

            return JsonSerializer.Deserialize<List<VideoGame>>(
                json,
                _jsonOptions
            ) ?? new List<VideoGame>();
        }

        private async Task<List<Studio>> ReadStudiosUnlockedAsync()
        {
            string json = await File.ReadAllTextAsync(_studiosFilePath);

            return JsonSerializer.Deserialize<List<Studio>>(
                json,
                _jsonOptions
            ) ?? new List<Studio>();
        }

        private async Task WriteUnlockedAsync(List<VideoGame> videoGames)
        {
            string json = JsonSerializer.Serialize(videoGames, _jsonOptions);

            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<List<VideoGame>> GetAllAsync()
        {
            await _lock.WaitAsync();

            try
            {
                var videoGames = await ReadUnlockedAsync();
                var studios = await ReadStudiosUnlockedAsync();

                foreach (var videoGame in videoGames)
                {
                    videoGame.Studio = studios.FirstOrDefault(
                        s => s.Id == videoGame.StudioId
                    );
                }

                return videoGames;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<VideoGame?> GetByIdAsync(int id)
        {
            await _lock.WaitAsync();

            try
            {
                var videoGames = await ReadUnlockedAsync();
                var studios = await ReadStudiosUnlockedAsync();

                var videoGame = videoGames.FirstOrDefault(g => g.Id == id);

                if (videoGame != null)
                {
                    videoGame.Studio = studios.FirstOrDefault(
                        s => s.Id == videoGame.StudioId
                    );
                }

                return videoGame;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task AddAsync(VideoGame videoGame)
        {
            await _lock.WaitAsync();

            try
            {
                var videoGames = await ReadUnlockedAsync();

                videoGame.Id = videoGames.Count == 0
                    ? 1
                    : videoGames.Max(g => g.Id) + 1;

                videoGames.Add(videoGame);

                await WriteUnlockedAsync(videoGames);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task UpdateAsync(VideoGame videoGame)
        {
            await _lock.WaitAsync();

            try
            {
                var videoGames = await ReadUnlockedAsync();

                var existingGame = videoGames.FirstOrDefault(
                    g => g.Id == videoGame.Id
                );

                if (existingGame == null)
                    return;

                existingGame.Title = videoGame.Title;
                existingGame.Genre = videoGame.Genre;
                existingGame.ReleaseYear = videoGame.ReleaseYear;
                existingGame.Rating = videoGame.Rating;
                existingGame.Description = videoGame.Description;
                existingGame.StudioId = videoGame.StudioId;

                await WriteUnlockedAsync(videoGames);
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
                var videoGames = await ReadUnlockedAsync();
                var videoGame = videoGames.FirstOrDefault(g => g.Id == id);

                if (videoGame == null)
                    return;

                videoGames.Remove(videoGame);

                await WriteUnlockedAsync(videoGames);
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
                var videoGames = await ReadUnlockedAsync();

                return videoGames.Any(g => g.Id == id);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
