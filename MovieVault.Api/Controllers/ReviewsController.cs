using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Domain;
using MovieVault.Api.Domain.DTOs;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public ReviewsController(MovieVaultContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReviewDto>>> GetReviews([FromQuery] int? movieId, [FromQuery] int? mediaOutletId)
    {
        var query = _context.Reviews.AsQueryable();

        if (movieId != null)
            query = query.Where(r => r.MovieId == movieId);

        if (mediaOutletId != null)
            query = query.Where(r => r.MediaOutletId == mediaOutletId);

        return await query
            .OrderByDescending(r => r.PublishedAt)
            .Select(ReviewDto.Projection)
            .ToListAsync();
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> GetReview(int id)
    {
        var review = await _context.Reviews
            .Where(r => r.ReviewId == id)
            .Select(ReviewDto.Projection)
            .FirstOrDefaultAsync();

        if (review == null)
        {
            return NotFound();
        }

        return review;
    }

    [Authorize(Roles = $"{Roles.Reviewer},{Roles.Admin}")]
    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto dto)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == dto.MovieId))
        {
            ModelState.AddModelError(nameof(dto.MovieId), "Movie does not exist.");
            return ValidationProblem(ModelState);
        }

        Reviewer? author;
        if (User.IsInRole(Roles.Admin))
        {
            if (dto.ReviewerId == null)
            {
                ModelState.AddModelError(nameof(dto.ReviewerId), "Admin must specify which reviewer wrote the review.");
                return ValidationProblem(ModelState);
            }

            author = await _context.Reviewers.FindAsync(dto.ReviewerId);
            if (author == null)
            {
                ModelState.AddModelError(nameof(dto.ReviewerId), "Reviewer does not exist.");
                return ValidationProblem(ModelState);
            }
        }
        else
        {
            author = await GetCurrentReviewerAsync();
            if (author == null)
            {
                return Forbid();
            }
        }

        var review = new Review
        {
            MovieId = dto.MovieId,
            Headline = dto.Headline,
            Text = dto.Text,
            Rating = dto.Rating,
            PublishedAt = DateTime.UtcNow,
            ReviewerId = author.ReviewerId,
            MediaOutletId = author.MediaOutletId
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var created = await _context.Reviews
            .Where(r => r.ReviewId == review.ReviewId)
            .Select(ReviewDto.Projection)
            .FirstAsync();

        return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, created);
    }

    [Authorize(Roles = $"{Roles.Reviewer},{Roles.Admin}")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateReview(int id, UpdateReviewDto dto)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        if (!await IsOwnerOrAdminAsync(review))
        {
            return Forbid();
        }

        review.Headline = dto.Headline;
        review.Text = dto.Text;
        review.Rating = dto.Rating;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = $"{Roles.Reviewer},{Roles.Admin}")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        if (!await IsOwnerOrAdminAsync(review))
        {
            return Forbid();
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    private async Task<Reviewer?> GetCurrentReviewerAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _context.Reviewers.FirstOrDefaultAsync(r => r.UserId == userId);
    }

    private async Task<bool> IsOwnerOrAdminAsync(Review review)
    {
        if (User.IsInRole(Roles.Admin))
            return true;

        var reviewer = await GetCurrentReviewerAsync();
        return reviewer != null && review.ReviewerId == reviewer.ReviewerId;
    }
}
