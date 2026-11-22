using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.Models;

public class Review
{
    public int ReviewId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Headline { get; set; } = "";

    [Required]
    [MaxLength(4000)]
    public string Text { get; set; } = "";

    [Range(1, 5)]
    public int Rating { get; set; }

    public DateTime PublishedAt { get; set; }

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public int ReviewerId { get; set; }
    public Reviewer Reviewer { get; set; } = null!;

    public int MediaOutletId { get; set; }
    public MediaOutlet MediaOutlet { get; set; } = null!;
}
