namespace VaultCore.Modules.Orders.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OrderNumber { get; set; } = string.Empty;
    public CustomerInfo Customer { get; set; } = new();
    public List<OrderItem> Items { get; set; } = [];
    public decimal Subtotal { get; set; }
    public decimal BundleDiscountAmount { get; set; }
    public decimal PromoDiscountAmount { get; set; }
    public string? AppliedPromoCode { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class CustomerInfo
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string CourierService { get; set; } = string.Empty;
}

public class OrderItem
{
    public Guid MiniatureId { get; set; }
    public string MiniatureName { get; set; } = string.Empty;
    public string Faction { get; set; } = string.Empty;
    public bool IsStarterSet { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
