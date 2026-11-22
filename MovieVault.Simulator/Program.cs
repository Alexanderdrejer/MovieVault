using MovieVault.Simulator;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("MovieVaultApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHostedService<TicketSaleSimulator>();

var host = builder.Build();
host.Run();
