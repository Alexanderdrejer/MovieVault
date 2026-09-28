using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.Models;

public class MovieActor
{
    public int MovieId { get; set; }
    public int ActorId { get; set; }

    [MaxLength(100)]
    public string? CharacterName { get; set; }

    [Required]
    [MaxLength(20)]
    public string RoleType { get; set; } = "";

    public Movie Movie { get; set; } = null!;
    public Actor Actor { get; set; } = null!;
}
