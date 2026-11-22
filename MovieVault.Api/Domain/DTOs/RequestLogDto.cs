namespace MovieVault.Api.Domain.DTOs;

public class RequestLogDto
{
    public string Username { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string RequestType { get; set; } = "";
    public string Route { get; set; } = "";
    public int StatusCode { get; set; }
}
