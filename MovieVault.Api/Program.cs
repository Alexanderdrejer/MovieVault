using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Identity;
using MovieVault.Api.Domain.Models;
using MovieVault.Api.Domain;
using MovieVault.Api.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;


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
builder.Services.AddScoped<MovieAccessService>();

var logDatabaseUrl = new MongoUrl(builder.Configuration["Serilog:WriteTo:0:Args:databaseUrl"]);
builder.Services.AddSingleton(new MongoClient(logDatabaseUrl)
    .GetDatabase(logDatabaseUrl.DatabaseName)
    .GetCollection<BsonDocument>(builder.Configuration["Serilog:WriteTo:0:Args:collectionName"]));

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
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        document.Security = [new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] }];
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next();

    var method = context.Request.Method;
    if (HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsDelete(method))
    {
        var isPublicEndpoint = context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null;
        var username = !isPublicEndpoint && context.User.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name
            : "anonymous";

        var logInfo = new
        {
            Username = username,
            Timestamp = DateTime.UtcNow,
            RequestType = method,
            Route = context.Request.Path.Value,
            StatusCode = context.Response.StatusCode
        };
        app.Logger.LogInformation("Request log {@LogInfo}", logInfo);
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
    await IdentitySeeder.SeedUserAsync(userManager, "reviewer2", demoPassword, Roles.Reviewer);
    await IdentitySeeder.SeedUserAsync(userManager, "admin", demoPassword, Roles.Admin);
    await IdentitySeeder.LinkDomainUsersAsync(context, userManager);

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
    options.AddPreferredSecuritySchemes("Bearer");
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
