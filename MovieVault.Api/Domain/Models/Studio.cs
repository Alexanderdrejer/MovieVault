using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.Models;

public class Studio
{
    public int StudioId { get; set; }
    public int ProductionCompanyId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(60)]
    public string? City { get; set; }

    [MaxLength(60)]
    public string? Country { get; set; }

    public int? SoundStages { get; set; }
    public int? MaxStageCapacity { get; set; }

    public ProductionCompany ProductionCompany { get; set; } = null!;
}
