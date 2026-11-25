using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GameCatalogApp.Interfaces;
using GameCatalogApp.Models;

namespace GameCatalogApp.Controllers
{
    public class VideoGamesController : Controller
    {
        private readonly IVideoGameRepository _repository;
        private readonly IStudioRepository _studioRepository;

        public VideoGamesController(
            IVideoGameRepository repository,
            IStudioRepository studioRepository)
        {
            _repository = repository;
            _studioRepository = studioRepository;
        }

        // GET: VIDEOGAMES
        public async Task<IActionResult> Index()
        {
            var videoGames = await _repository.GetAllAsync();

            return View(videoGames);
        }

        // GET: VIDEOGAMES/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoGame = await _repository.GetByIdAsync(id.Value);

            if (videoGame == null)
            {
                return NotFound();
            }

            return View(videoGame);
        }

        // GET: VIDEOGAMES/Create
        public async Task<IActionResult> Create()
        {
            await LoadStudiosAsync();

            return View();
        }

        // POST: VIDEOGAMES/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Title,Genre,ReleaseYear,Rating,Description,StudioId")]
            VideoGame videoGame)
        {
            await ValidateStudioAsync(videoGame.StudioId);

            if (ModelState.IsValid)
            {
                await _repository.AddAsync(videoGame);

                return RedirectToAction(nameof(Index));
            }

            await LoadStudiosAsync(videoGame.StudioId);

            return View(videoGame);
        }

        // GET: VIDEOGAMES/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoGame = await _repository.GetByIdAsync(id.Value);

            if (videoGame == null)
            {
                return NotFound();
            }

            await LoadStudiosAsync(videoGame.StudioId);

            return View(videoGame);
        }

        // POST: VIDEOGAMES/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int? id,
            [Bind("Id,Title,Genre,ReleaseYear,Rating,Description,StudioId")]
            VideoGame videoGame)
        {
            if (id != videoGame.Id)
            {
                return NotFound();
            }

            await ValidateStudioAsync(videoGame.StudioId);

            if (ModelState.IsValid)
            {
                if (!await _repository.ExistsAsync(videoGame.Id))
                {
                    return NotFound();
                }

                await _repository.UpdateAsync(videoGame);

                return RedirectToAction(nameof(Index));
            }

            await LoadStudiosAsync(videoGame.StudioId);

            return View(videoGame);
        }

        // GET: VIDEOGAMES/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoGame = await _repository.GetByIdAsync(id.Value);

            if (videoGame == null)
            {
                return NotFound();
            }

            return View(videoGame);
        }

        // POST: VIDEOGAMES/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id.Value);

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadStudiosAsync(int? selectedStudioId = null)
        {
            var studios = await _studioRepository.GetAllAsync();

            ViewData["Studios"] = new SelectList(
                studios,
                "Id",
                "Name",
                selectedStudioId
            );
        }

        private async Task ValidateStudioAsync(int? studioId)
        {
            if (studioId.HasValue &&
                !await _studioRepository.ExistsAsync(studioId.Value))
            {
                ModelState.AddModelError(
                    nameof(VideoGame.StudioId),
                    "Wybrane studio nie istnieje."
                );
            }
        }
    }
}
