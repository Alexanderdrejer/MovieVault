using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Identity;
using MovieVault.Api.Domain.Models;
using MovieVault.Api.Domain;
using MovieVault.Api.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddDbContext<MovieVaultContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieVault")));

builder.Services.AddIdentityCore<ApplicationUser>() // Jeg vil nu have brugere og de skal se ud som min APplicationUser.
    .AddRoles<IdentityRole>() // Brugere kan nu have roller: Det er seederen der bestemmre rollerne, så de er på det her tidspunkt ikke derinde endnu. 
    .AddEntityFrameworkStores<MovieVaultContext>(); // .. Og alle mine rolelr og brugere skal gemmes i min database som du finde ri MovieVaultContext aka MovieVaultContext.cs.
    builder.Services.AddScoped<TokenService>(); 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!))
        };
    });



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

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeeder.SeedRolesAsync(roleManager);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    const string demoPassword = "Demo123!";

    await IdentitySeeder.SeedUserAsync(userManager, "actor", demoPassword, Roles.Actor);
    await IdentitySeeder.SeedUserAsync(userManager, "director", demoPassword, Roles.Director);
    await IdentitySeeder.SeedUserAsync(userManager, "reviewer", demoPassword, Roles.Reviewer);
    await IdentitySeeder.SeedUserAsync(userManager, "admin", demoPassword, Roles.Admin);

    var simulatorUserName = app.Configuration["Simulator:UserName"];
    var simulatorPassword = app.Configuration["Simulator:Password"];

    if (!string.IsNullOrWhiteSpace(simulatorUserName) && !string.IsNullOrWhiteSpace(simulatorPassword))
    {
        await IdentitySeeder.SeedUserAsync(userManager, simulatorUserName, simulatorPassword, Roles.Admin);
    }
}

app.MapOpenApi("/openapi/v1.json");
app.MapScalarApiReference(options =>
{
    options.Title = "MovieVault API";
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
