using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using VaultCore.Modules.Catalog.Models;

namespace VaultCore.Api.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog").WithTags("Catalog");

        group.MapGet("/", async (
            IQuerySession session,
            string? q,
            string? universe,
            string? faction,
            string? tag,
            decimal? minPrice,
            decimal? maxPrice,
            string? sort,
            int page = 1,
            int pageSize = 20) =>
        {
            var query = session.Query<Miniature>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(term) || x.Description.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(universe))
            {
                query = query.Where(x => x.Universe == universe);
            }

            if (!string.IsNullOrWhiteSpace(faction))
            {
                query = query.Where(x => x.Faction == faction);
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                query = query.Where(x => x.Tags.Contains(tag));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(x => x.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= maxPrice.Value);
            }

            query = sort switch
            {
                "price_asc" => query.OrderBy(x => x.Price),
                "price_desc" => query.OrderByDescending(x => x.Price),
                "name" => query.OrderBy(x => x.Name),
                _ => query.OrderByDescending(x => x.IsFeatured).ThenBy(x => x.Name)
            };

            var totalItems = await query.CountAsync();
            var skip = Math.Max(0, (page - 1) * pageSize);
            var items = await query.Skip(skip).Take(pageSize).ToListAsync();

            return Results.Ok(new
            {
                Total = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = items
            });
        });

        group.MapGet("/{id:guid}", async (IQuerySession session, Guid id) =>
        {
            var miniature = await session.LoadAsync<Miniature>(id);
            return miniature is not null ? Results.Ok(miniature) : Results.NotFound(new { Message = $"Miniature {id} not found" });
        });

        group.MapGet("/factions", async (IQuerySession session) =>
        {
            var all = await session.Query<Miniature>().ToListAsync();
            var factions = all
                .GroupBy(x => x.Faction)
                .Select(g => new
                {
                    Faction = g.Key,
                    Count = g.Count(),
                    Universe = g.FirstOrDefault()?.Universe ?? ""
                })
                .OrderBy(x => x.Universe).ThenBy(x => x.Faction)
                .ToList();

            return Results.Ok(factions);
        });

        group.MapPost("/", async (IDocumentSession session, Miniature miniature) =>
        {
            session.Store(miniature);
            await session.SaveChangesAsync();
            return Results.Created($"/api/catalog/{miniature.Id}", miniature);
        });

        return app;
    }
}
