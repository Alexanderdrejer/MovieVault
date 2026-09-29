using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Domain.DTOs;

public class ScreeningDto
{
    public int ScreeningId { get; set; }
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = "";
    public int CinemaId { get; set; }
    public string CinemaName { get; set; } = "";
    public int ScreenId { get; set; }
    public string ScreenName { get; set; } = "";
    public DateTime StartsAt { get; set; }
    public decimal TicketPrice { get; set; }
    public bool IsCancelled { get; set; }
    public int SeatingCapacity { get; set; }
    public int TicketsSold { get; set; }

    public static Expression<Func<Screening, ScreeningDto>> Projection => s => new ScreeningDto
    {
        ScreeningId = s.ScreeningId,
        MovieId = s.MovieId,
        MovieTitle = s.Movie.Title,
        CinemaId = s.Screen.CinemaId,
        CinemaName = s.Screen.Cinema.Name,
        ScreenId = s.ScreenId,
        ScreenName = s.Screen.ScreenNumberOrName,
        StartsAt = s.StartsAt,
        TicketPrice = s.TicketPrice,
        IsCancelled = s.IsCancelled,
        SeatingCapacity = s.Screen.SeatingCapacity,
        TicketsSold = s.TicketSales.Sum(ts => ts.TicketCount)
    };
}

public class CreateScreeningDto
{
    [Required]
    public int? MovieId { get; set; }

    [Required]
    public int? ScreenId { get; set; }

    [Required]
    public DateTime? StartsAt { get; set; }

    [Required]
    [Range(0, 10000, ErrorMessage = "TicketPrice cannot be negative.")]
    public decimal? TicketPrice { get; set; }
}

public class UpdateScreeningDto
{
    [Required]
    public int? ScreenId { get; set; }

    [Required]
    public DateTime? StartsAt { get; set; }

    [Required]
    [Range(0, 10000, ErrorMessage = "TicketPrice cannot be negative.")]
    public decimal? TicketPrice { get; set; }

    public bool IsCancelled { get; set; }
}
