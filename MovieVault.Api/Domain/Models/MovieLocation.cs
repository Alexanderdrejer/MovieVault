namespace MovieVault.Api.Domain.Models;

public class MovieLocation
{
    public int MovieId { get; set; }
    public int LocationId { get; set; }
    public int? ShootingDays { get; set; }

    public Movie Movie { get; set; } = null!;
    public Location Location { get; set; } = null!;
}
