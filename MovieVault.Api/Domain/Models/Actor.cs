using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieVault.Api.Domain.Models;

public class Actor
{
    public int ActorId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Column(TypeName = "date")]
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(60)]
    public string? Nationality { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
