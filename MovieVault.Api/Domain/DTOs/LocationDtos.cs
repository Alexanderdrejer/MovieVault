namespace MovieVault.Api.Domain.DTOs;

public class LocationDto
{
    public int LocationId { get; set; }
    public string Name { get; set; } = "";
    public string? City { get; set; }
    public string? Country { get; set; }
    public string LocationType { get; set; } = "";
}
