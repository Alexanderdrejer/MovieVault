using Microsoft.EntityFrameworkCore;

namespace MovieVault.Api.Domain.Models;

public class TicketSale
{
    public int TicketSaleId { get; set; }
    public int ScreeningId { get; set; }
    public DateTime SaleDateTime { get; set; }
    public int TicketCount { get; set; }

    [Precision(6, 2)]
    public decimal PricePerTicket { get; set; }

    public Screening Screening { get; set; } = null!;
}
