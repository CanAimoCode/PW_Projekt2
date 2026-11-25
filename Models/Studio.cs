using System.ComponentModel.DataAnnotations;

namespace GameCatalogApp.Models
{
    public class Studio
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Podaj nazwę studia")]
        [StringLength(100)]
        [Display(Name = "Nazwa studia")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj kraj")]
        [Display(Name = "Kraj")]
        public string Country { get; set; } = string.Empty;

        [Display(Name = "Data założenia")]
        public DateTime? FoundedDate { get; set; }

        [Display(Name = "Siedziba")]
        public string? Headquarters { get; set; }

        [Display(Name = "Opis studia")]
        public string? Description { get; set; }

        public ICollection<VideoGame> VideoGames { get; set; } = new List<VideoGame>();
    }
}
