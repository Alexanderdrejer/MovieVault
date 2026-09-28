using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieVault.Api.Domain.Models;

public class ProductionRecord
{
    [Key]
    public int MovieId { get; set; }

    [Precision(15, 2)]
    public decimal? ProductionBudget { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ShootingStartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ShootingEndDate { get; set; }

    public Movie Movie { get; set; } = null!;
}
