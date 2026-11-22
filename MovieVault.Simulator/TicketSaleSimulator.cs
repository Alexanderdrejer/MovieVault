using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MovieVault.Simulator;

public class TicketSaleSimulator : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TicketSaleSimulator> _logger;
    private string? _token;

    public TicketSaleSimulator(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<TicketSaleSimulator> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(_configuration.GetValue("Simulator:IntervalSeconds", 15));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SimulateSaleAsync(stoppingToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning("API not reachable ({Message}). Retrying in {Seconds} seconds.", ex.Message, interval.TotalSeconds);
            }
            catch (TaskCanceledException) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("API request timed out. Retrying in {Seconds} seconds.", interval.TotalSeconds);
            }

            await Task.Delay(interval, stoppingToken);
        }
    }

    private async Task SimulateSaleAsync(CancellationToken stoppingToken)
    {
        var client = _httpClientFactory.CreateClient("MovieVaultApi");

        _token ??= await LoginAsync(client, stoppingToken);
        if (_token == null)
            return;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var screeningsResponse = await client.GetAsync("api/screenings/upcoming", stoppingToken);
        if (!screeningsResponse.IsSuccessStatusCode)
        {
            HandleFailedResponse(screeningsResponse, "Fetching upcoming screenings");
            return;
        }

        var screenings = await screeningsResponse.Content.ReadFromJsonAsync<List<ScreeningInfo>>(stoppingToken) ?? [];
        var available = screenings.Where(s => s.TicketsSold < s.SeatingCapacity).ToList();
        if (available.Count == 0)
        {
            _logger.LogInformation("No upcoming screenings with free seats.");
            return;
        }

        var screening = available[Random.Shared.Next(available.Count)];
        var ticketCount = Random.Shared.Next(1, Math.Min(4, screening.SeatingCapacity - screening.TicketsSold) + 1);

        var sale = new { screeningId = screening.ScreeningId, ticketCount, pricePerTicket = screening.TicketPrice };
        var saleResponse = await client.PostAsJsonAsync("api/ticketsales", sale, stoppingToken);

        if (saleResponse.IsSuccessStatusCode)
        {
            _logger.LogInformation("Sold {TicketCount} ticket(s) for '{MovieTitle}' (screening {ScreeningId}).",
                ticketCount, screening.MovieTitle, screening.ScreeningId);
        }
        else
        {
            HandleFailedResponse(saleResponse, $"Ticket sale for screening {screening.ScreeningId}");
        }
    }

    private async Task<string?> LoginAsync(HttpClient client, CancellationToken stoppingToken)
    {
        var credentials = new
        {
            userName = _configuration["Simulator:UserName"],
            password = _configuration["Simulator:Password"]
        };

        var response = await client.PostAsJsonAsync("api/auth/login", credentials, stoppingToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Login failed with status {StatusCode}.", (int)response.StatusCode);
            return null;
        }

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>(stoppingToken);
        _logger.LogInformation("Logged in as simulator user.");
        return token?.AccessToken;
    }

    private void HandleFailedResponse(HttpResponseMessage response, string action)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            _token = null;

        _logger.LogWarning("{Action} failed with status {StatusCode}.", action, (int)response.StatusCode);
    }

    private record ScreeningInfo(int ScreeningId, int MovieId, string MovieTitle, DateTime StartsAt, decimal TicketPrice, int SeatingCapacity, int TicketsSold);

    private record TokenResponse(string AccessToken, DateTime ExpiresAt);
}
