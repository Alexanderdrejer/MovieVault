using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieVault.Api.Domain.Models;

public class Movie
{
    public int MovieId { get; set; }
    public int? DirectorId { get; set; }
    public int? ProductionCompanyId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = "";

    [Column(TypeName = "date")]
    public DateTime? ReleaseDate { get; set; }

    public int? RuntimeMinutes { get; set; }

    [MaxLength(10)]
    public string? AgeRating { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "";

    public Director? Director { get; set; }
    public ProductionCompany? ProductionCompany { get; set; }
    public ProductionRecord? ProductionRecord { get; set; }
    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    public ICollection<MovieLocation> MovieLocations { get; set; } = new List<MovieLocation>();
    public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
}
