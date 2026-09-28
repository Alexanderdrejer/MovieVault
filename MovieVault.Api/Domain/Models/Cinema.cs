using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.Models;

public class Cinema
{
    public int CinemaId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(60)]
    public string? City { get; set; }

    [MaxLength(150)]
    public string? StreetAddress { get; set; }

    public int? OpeningYear { get; set; }

    public ICollection<Screen> Screens { get; set; } = new List<Screen>();
}
