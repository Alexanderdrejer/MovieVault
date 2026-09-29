using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Domain.DTOs;

public class TicketSaleDto
{
    public int TicketSaleId { get; set; }
    public int ScreeningId { get; set; }
    public string MovieTitle { get; set; } = "";
    public string CinemaName { get; set; } = "";
    public DateTime ScreeningStartsAt { get; set; }
    public DateTime SaleDateTime { get; set; }
    public int TicketCount { get; set; }
    public decimal PricePerTicket { get; set; }
    public decimal TotalPrice { get; set; }

    public static Expression<Func<TicketSale, TicketSaleDto>> Projection => ts => new TicketSaleDto
    {
        TicketSaleId = ts.TicketSaleId,
        ScreeningId = ts.ScreeningId,
        MovieTitle = ts.Screening.Movie.Title,
        CinemaName = ts.Screening.Screen.Cinema.Name,
        ScreeningStartsAt = ts.Screening.StartsAt,
        SaleDateTime = ts.SaleDateTime,
        TicketCount = ts.TicketCount,
        PricePerTicket = ts.PricePerTicket,
        TotalPrice = ts.TicketCount * ts.PricePerTicket
    };
}

public class CreateTicketSaleDto
{
    [Required]
    public int? ScreeningId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "TicketCount must be at least 1.")]
    public int? TicketCount { get; set; }

    [Range(0, 10000, ErrorMessage = "PricePerTicket cannot be negative.")]
    public decimal? PricePerTicket { get; set; }
}

public class UpdateTicketSaleDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "TicketCount must be at least 1.")]
    public int? TicketCount { get; set; }

    [Required]
    [Range(0, 10000, ErrorMessage = "PricePerTicket cannot be negative.")]
    public decimal? PricePerTicket { get; set; }
}
