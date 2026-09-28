using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.Models;

public class Location
{
    public int LocationId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(60)]
    public string? City { get; set; }

    [MaxLength(60)]
    public string? Country { get; set; }

    [Required]
    [MaxLength(20)]
    public string LocationType { get; set; } = "";

    public ICollection<MovieLocation> MovieLocations { get; set; } = new List<MovieLocation>();
}
