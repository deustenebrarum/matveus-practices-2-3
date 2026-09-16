using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using VaultCore.Modules.Inventory.Models;

namespace VaultCore.Api.Endpoints;

public record AdjustInventoryRequest(Guid MiniatureId, int DeltaQuantity);

public static class InventoryEndpoints
{
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory").WithTags("Inventory");

        group.MapGet("/", async (IQuerySession session) =>
        {
            var items = await session.Query<InventoryItem>()
                .OrderBy(x => x.MiniatureName)
                .ToListAsync();

            return Results.Ok(items);
        });

        group.MapGet("/{miniatureId:guid}", async (IQuerySession session, Guid miniatureId) =>
        {
            var item = await session.LoadAsync<InventoryItem>(miniatureId);
            return item is not null ? Results.Ok(item) : Results.NotFound(new { Message = $"Inventory item {miniatureId} not found." });
        });

        group.MapPost("/adjust", async (IDocumentSession session, AdjustInventoryRequest req) =>
        {
            var item = await session.LoadAsync<InventoryItem>(req.MiniatureId);
            if (item is null)
            {
                return Results.NotFound(new { Message = $"Inventory item {req.MiniatureId} not found." });
            }

            item.AvailableStock += req.DeltaQuantity;
            if (item.AvailableStock < 0)
            {
                item.AvailableStock = 0;
            }
            item.LastUpdatedAt = DateTimeOffset.UtcNow;

            session.Store(item);
            await session.SaveChangesAsync();

            return Results.Ok(item);
        });

        return app;
    }
}
