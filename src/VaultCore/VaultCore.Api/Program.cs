using FluentValidation;
using JasperFx.Core;
using Marten;
using Marten.Exceptions;
using VaultCore.Api.Endpoints;
using VaultCore.Api.Services;
using VaultCore.Modules.Catalog.Models;
using VaultCore.Modules.Inventory.Handlers;
using VaultCore.Modules.Inventory.Models;
using VaultCore.Modules.Orders.Commands;
using VaultCore.Modules.Orders.Models;
using VaultCore.SharedKernel.Contracts;
using Weasel.Core;
using Wolverine;
using Wolverine.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL")
    ?? "Host=localhost;Port=5432;Database=vaultcore_dev;Username=postgres;Password=postgres;";

// Marten configuration (.NET 10 / Critter Stack)
builder.Services.AddMarten(opts =>
{
    opts.Connection(connectionString);
    opts.AutoCreateSchemaObjects = builder.Environment.IsProduction()
        ? AutoCreate.CreateOrUpdate
        : AutoCreate.All;

    // Schema configuration
    opts.Schema.For<Miniature>()
        .Identity(x => x.Id)
        .Index(x => x.Faction)
        .Index(x => x.Universe);

    opts.Schema.For<InventoryItem>()
        .Identity(x => x.Id)
        .UseOptimisticConcurrency(true);

    opts.Schema.For<Order>()
        .Identity(x => x.Id)
        .Index(x => x.OrderNumber);
})
.UseLightweightSessions();

// Wolverine Bus & Transactional Outbox
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(OrderPlacedHandler).Assembly);
    opts.PublishMessage<OrderPlacedEvent>().ToLocalQueue("inventory");

    // Concurrency Retry Policy
    opts.OnException<ConcurrencyException>()
        .RetryWithCooldown(50.Milliseconds(), 150.Milliseconds(), 300.Milliseconds());
});

// FluentValidation
builder.Services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();

// Seed service
builder.Services.AddScoped<SeedDataService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Health check
app.MapGet("/healthz", () => Results.Ok(new { Status = "Healthy", Service = "VaultCore", Timestamp = DateTimeOffset.UtcNow }));

// Modular Monolith Endpoints
app.MapCatalogEndpoints();
app.MapOrdersEndpoints();
app.MapInventoryEndpoints();

// Seed data
using (var scope = app.Services.CreateScope())
{
    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<SeedDataService>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Could not run initial seed (e.g. database not reachable yet during build/isolated test)");
    }
}

app.Run();

public partial class Program { }
