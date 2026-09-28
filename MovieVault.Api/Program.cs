using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Enable OpenAPI + Scalar
app.MapOpenApi("/openapi/v1.json");
app.MapScalarApiReference(options =>
{
    options.Title = "MovieVault API";
});

// Minimal example endpoint
app.MapGet("/api/movies", () =>
{
    return new[]
    {
        new Movie(
            "API-calypse Now",
            new DateTime(1979, 4, 1),
            147,
            "War Epic",
            "R"
        ),
        new Movie(
            "The API Strikes Back",
            new DateTime(1980, 3, 3),
            124,
            "Si-Fi Epic",
            "PG"
        )
    };
});

app.Run();

record Movie(
    string Title,
    DateTime ReleaseDate,
    int Runtime,
    string Genre,
    string ParentalGuide
);
