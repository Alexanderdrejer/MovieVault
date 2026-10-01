using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddDbContext<MovieVaultContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieVault")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next();

    var method = context.Request.Method;
    if (HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsDelete(method))
    {
        var logInfo = new
        {
            Method = method,
            Path = context.Request.Path.Value,
            StatusCode = context.Response.StatusCode,
            Timestamp = DateTime.UtcNow
        };
        app.Logger.LogInformation("Change request {@LogInfo}", logInfo);
    }
});

app.UseExceptionHandler(action =>
{
    action.Run(async context =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();
        app.Logger.LogError(exceptionHandler?.Error, "An unhandled exception occurred.");

        var details = new ProblemDetails();
        details.Detail = exceptionHandler?.Error.Message;
        details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
        details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
        details.Status = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(details));
    });
});

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MovieVaultContext>();
    context.Database.Migrate();
}

app.MapOpenApi("/openapi/v1.json");
app.MapScalarApiReference(options =>
{
    options.Title = "MovieVault API";
});

app.MapControllers();

app.Run();
