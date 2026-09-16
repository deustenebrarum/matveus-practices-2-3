using Marten;
using Microsoft.Extensions.Logging;
using VaultCore.Modules.Inventory.Models;
using VaultCore.SharedKernel.Contracts;
using VaultCore.SharedKernel.Exceptions;

namespace VaultCore.Modules.Inventory.Handlers;

public class OrderPlacedHandler
{
    private readonly ILogger<OrderPlacedHandler> _logger;

    public OrderPlacedHandler(ILogger<OrderPlacedHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(OrderPlacedEvent @event, IDocumentSession session)
    {
        _logger.LogInformation("Processing OrderPlacedEvent for order {OrderNumber} with {ItemCount} items",
            @event.OrderNumber, @event.Items.Count);

        foreach (var item in @event.Items)
        {
            var inventory = await session.LoadAsync<InventoryItem>(item.MiniatureId);
            if (inventory is null)
            {
                _logger.LogWarning("Inventory record not found for miniature {MiniatureId}, initializing item", item.MiniatureId);
                inventory = new InventoryItem
                {
                    Id = item.MiniatureId,
                    MiniatureName = item.MiniatureName,
                    AvailableStock = 20,
                    LastUpdatedAt = DateTimeOffset.UtcNow
                };
                session.Store(inventory);
            }

            if (inventory.AvailableStock < item.Quantity)
            {
                _logger.LogError("Insufficient stock for {MiniatureName} ({MiniatureId}). Available: {Available}, Requested: {Requested}",
                    inventory.MiniatureName, inventory.Id, inventory.AvailableStock, item.Quantity);
                throw new InsufficientStockException(inventory.Id, item.Quantity, inventory.AvailableStock);
            }

            inventory.AvailableStock -= item.Quantity;
            inventory.LastUpdatedAt = DateTimeOffset.UtcNow;
            session.Store(inventory);
        }

        await session.SaveChangesAsync();
        _logger.LogInformation("Stock successfully deducted for order {OrderNumber}", @event.OrderNumber);
    }
}
