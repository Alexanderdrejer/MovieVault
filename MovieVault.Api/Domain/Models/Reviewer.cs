using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.Models;

public class Reviewer
{
    public int ReviewerId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    public int MediaOutletId { get; set; }
    public MediaOutlet MediaOutlet { get; set; } = null!;

    [MaxLength(450)]
    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
