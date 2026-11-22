using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.Models;

public class MediaOutlet
{
    public int MediaOutletId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [MaxLength(30)]
    public string OutletType { get; set; } = "";

    [MaxLength(200)]
    public string? Website { get; set; }

    public ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
