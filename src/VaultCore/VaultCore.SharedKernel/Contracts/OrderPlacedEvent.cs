namespace VaultCore.SharedKernel.Contracts;

public record OrderPlacedEvent(
    Guid OrderId,
    string OrderNumber,
    string CustomerEmail,
    IReadOnlyList<OrderItemDto> Items,
    decimal TotalAmount,
    DateTimeOffset PlacedAt
);

public record OrderItemDto(
    Guid MiniatureId,
    string MiniatureName,
    int Quantity,
    decimal UnitPrice
);
