namespace MovieVault.Api.Domain.DTOs;

public class ActorDto
{
    public int ActorId { get; set; }
    public string Name { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Gender { get; set; }
}

public class ActorPerformanceDto
{
    public int ActorId { get; set; }
    public string Name { get; set; } = "";
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<ActorMovieDto> Movies { get; set; } = new();
}

public class ActorMovieDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = "";
    public string? CharacterName { get; set; }
    public string RoleType { get; set; } = "";
    public int TicketsSold { get; set; }
    public decimal Revenue { get; set; }
}
