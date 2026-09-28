using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.Models;

public class ProductionCompany
{
    public int ProductionCompanyId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    public int? FoundingYear { get; set; }

    [MaxLength(60)]
    public string? Country { get; set; }

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    public ICollection<Studio> Studios { get; set; } = new List<Studio>();
}
