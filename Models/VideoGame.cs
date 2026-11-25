using System.ComponentModel.DataAnnotations;

namespace GameCatalogApp.Models
{
    public class VideoGame
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Podaj tytuł gry")]
        [StringLength(100)]
        [Display(Name = "Tytuł gry")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj gatunek")]
        [Display(Name = "Gatunek")]
        public string Genre { get; set; } = string.Empty;

        [Range(1970, 2100, ErrorMessage = "Podaj prawidłowy rok wydania")]
        [Display(Name = "Rok wydania")]
        public int ReleaseYear { get; set; }

        [Range(0, 10, ErrorMessage = "Ocena musi być z przedziału 0–10")]
        [Display(Name = "Ocena")]
        public decimal Rating { get; set; }

        [Display(Name = "Opis gry")]
        public string? Description { get; set; }

        public int? StudioId { get; set; }

        public Studio? Studio { get; set; }
    }
}
