using Microsoft.EntityFrameworkCore;

namespace MovieVault.Api.Domain.Models;

public class Screening
{
    public int ScreeningId { get; set; }
    public int MovieId { get; set; }
    public int ScreenId { get; set; }
    public DateTime StartsAt { get; set; }

    [Precision(6, 2)]
    public decimal TicketPrice { get; set; }

    public bool IsCancelled { get; set; }

    public Movie Movie { get; set; } = null!;
    public Screen Screen { get; set; } = null!;
    public ICollection<TicketSale> TicketSales { get; set; } = new List<TicketSale>();
}
