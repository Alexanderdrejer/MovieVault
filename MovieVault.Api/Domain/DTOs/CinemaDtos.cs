namespace MovieVault.Api.Domain.DTOs;

public class CinemaDto
{
    public int CinemaId { get; set; }
    public string Name { get; set; } = "";
    public string? City { get; set; }
    public string? StreetAddress { get; set; }
    public List<ScreenDto> Screens { get; set; } = new();
}

public class ScreenDto
{
    public int ScreenId { get; set; }
    public string ScreenNumberOrName { get; set; } = "";
    public int SeatingCapacity { get; set; }
    public string? ScreenType { get; set; }
}
