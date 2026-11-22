using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Domain.DTOs;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public string Headline { get; set; } = "";
    public string Text { get; set; } = "";
    public int Rating { get; set; }
    public DateTime PublishedAt { get; set; }
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = "";
    public int ReviewerId { get; set; }
    public string ReviewerName { get; set; } = "";
    public int MediaOutletId { get; set; }
    public string MediaOutletName { get; set; } = "";

    public static Expression<Func<Review, ReviewDto>> Projection => r => new ReviewDto
    {
        ReviewId = r.ReviewId,
        Headline = r.Headline,
        Text = r.Text,
        Rating = r.Rating,
        PublishedAt = r.PublishedAt,
        MovieId = r.MovieId,
        MovieTitle = r.Movie.Title,
        ReviewerId = r.ReviewerId,
        ReviewerName = r.Reviewer.Name,
        MediaOutletId = r.MediaOutletId,
        MediaOutletName = r.MediaOutlet.Name
    };
}

public class CreateReviewDto
{
    [Required]
    public int MovieId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Headline { get; set; } = "";

    [Required]
    [MaxLength(4000)]
    public string Text { get; set; } = "";

    [Range(1, 5)]
    public int Rating { get; set; }

    public int? ReviewerId { get; set; }
}

public class UpdateReviewDto
{
    [Required]
    [MaxLength(150)]
    public string Headline { get; set; } = "";

    [Required]
    [MaxLength(4000)]
    public string Text { get; set; } = "";

    [Range(1, 5)]
    public int Rating { get; set; }
}
