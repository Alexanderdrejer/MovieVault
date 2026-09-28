using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieVault.Api.Domain.Models;

public class Director
{
    public int DirectorId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Column(TypeName = "date")]
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(60)]
    public string? Nationality { get; set; }

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
