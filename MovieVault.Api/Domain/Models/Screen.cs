using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.Models;

public class Screen
{
    public int ScreenId { get; set; }
    public int CinemaId { get; set; }

    [Required]
    [MaxLength(40)]
    public string ScreenNumberOrName { get; set; } = "";

    public int SeatingCapacity { get; set; }

    [MaxLength(30)]
    public string? ScreenType { get; set; }

    public Cinema Cinema { get; set; } = null!;
    public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
}
