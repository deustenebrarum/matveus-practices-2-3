# SPEC_ORDERS.md — Спецификация модуля Orders

## 1. Назначение
Валидация корзины, расчет бандл-скидок и промокодов, оформление заказов и отправка события в шину через Transactional Outbox.

## 2. Модель заказа
```csharp
public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OrderNumber { get; set; } = string.Empty; // Format: "ORD-YYYYMMDD-XXXX"
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

public record CustomerInfo(string FullName, string Email, string Phone, string ShippingAddress);

public record OrderItem(Guid MiniatureId, string MiniatureName, string Faction, int Quantity, decimal UnitPrice, decimal TotalPrice);

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
```

## 3. Бизнес-правила расчета стоимости
1. **Бандл-скидка (Bundle Discount):**
   - 10% от базовой стоимости, если заказ содержит хотя бы 1 стартовый набор (`StarterSet`) ИЛИ суммарно от 3 миниатюр одной фракции.
2. **Промокоды:**
   - `WARHAMMER10` — скидка 10% на оставшуюся сумму.
   - `EMPEROR20` — скидка 20% на оставшуюся сумму.
3. **Логика применения:** `Total = Subtotal - BundleDiscount - PromoDiscount`. Округление до 2 знаков. Сумма не может быть меньше 0.

## 4. Валидация (FluentValidation)
- `Customer.FullName`: не пусто, от 2 до 100 символов.
- `Customer.Email`: корректный формат email.
- `Customer.Phone`: не пусто, от 7 до 20 символов.
- `Customer.ShippingAddress`: не пусто, от 5 до 250 символов.
- `Items`: не пустой список; для каждого элемента `Quantity > 0`, `UnitPrice > 0`.

## 5. Эндпоинты HTTP
| Метод | Путь | Назначение |
|---|---|---|
| `POST` | `/api/orders` | Оформление заказа (валидация, сохранение в Marten, публикация `OrderPlacedEvent` в Outbox) |
| `GET` | `/api/orders/{id}` | Детализация заказа |
| `GET` | `/api/orders/user/{email}` | История заказов пользователя для ЛК `/account` |
