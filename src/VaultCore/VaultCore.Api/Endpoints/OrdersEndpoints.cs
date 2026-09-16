using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using VaultCore.Modules.Catalog.Models;
using VaultCore.Modules.Orders.Commands;
using VaultCore.Modules.Orders.Models;
using VaultCore.Modules.Orders.Services;
using VaultCore.SharedKernel.Contracts;
using Wolverine;

namespace VaultCore.Api.Endpoints;

public static class OrdersEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapPost("/", async (
            CreateOrderCommand command,
            IValidator<CreateOrderCommand> validator,
            IDocumentSession session,
            IMessageBus bus) =>
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var miniatureIds = command.Items.Select(x => x.MiniatureId).ToList();
            var miniatures = await session.LoadManyAsync<Miniature>(miniatureIds);
            var miniatureDict = miniatures.ToDictionary(x => x.Id);

            var missing = miniatureIds.FirstOrDefault(id => !miniatureDict.ContainsKey(id));
            if (missing != Guid.Empty)
            {
                return Results.BadRequest(new { Message = $"Miniature {missing} not found in catalog." });
            }

            var orderItems = command.Items.Select(req =>
            {
                var miniature = miniatureDict[req.MiniatureId];
                return new OrderItem
                {
                    MiniatureId = miniature.Id,
                    MiniatureName = miniature.Name,
                    Faction = miniature.Faction,
                    IsStarterSet = miniature.Tags.Contains("StarterSet"),
                    Quantity = req.Quantity,
                    UnitPrice = miniature.Price
                };
            }).ToList();

            var calc = DiscountCalculator.Calculate(orderItems, command.PromoCode);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                Customer = command.Customer,
                Items = orderItems,
                Subtotal = calc.Subtotal,
                BundleDiscountAmount = calc.BundleDiscount,
                PromoDiscountAmount = calc.PromoDiscount,
                AppliedPromoCode = string.IsNullOrWhiteSpace(command.PromoCode) ? null : command.PromoCode.Trim().ToUpperInvariant(),
                TotalAmount = calc.Total,
                Status = OrderStatus.Confirmed,
                CreatedAt = DateTimeOffset.UtcNow
            };

            session.Store(order);

            var outboxEvent = new OrderPlacedEvent(
                order.Id,
                order.OrderNumber,
                order.Customer.Email,
                order.Items.Select(i => new OrderItemDto(i.MiniatureId, i.MiniatureName, i.Quantity, i.UnitPrice)).ToList(),
                order.TotalAmount,
                order.CreatedAt
            );

            await bus.PublishAsync(outboxEvent);
            await session.SaveChangesAsync();

            return Results.Created($"/api/orders/{order.Id}", order);
        });

        group.MapGet("/{id:guid}", async (IQuerySession session, Guid id) =>
        {
            var order = await session.LoadAsync<Order>(id);
            return order is not null ? Results.Ok(order) : Results.NotFound(new { Message = $"Order {id} not found." });
        });

        group.MapGet("/user/{email}", async (IQuerySession session, string email) =>
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var orders = await session.Query<Order>()
                .Where(x => x.Customer.Email.ToLower() == normalizedEmail)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Results.Ok(orders);
        });

        return app;
    }
}
