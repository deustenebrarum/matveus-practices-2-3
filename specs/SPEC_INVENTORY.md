# SPEC_INVENTORY.md — Спецификация модуля Inventory

## 1. Назначение
Учет складских запасов миниатюр, атомарное списание при оформлении заказов, защита от состояния гонки через Marten Optimistic Concurrency и автоматические повторы Wolverine.

## 2. Модель остатка (Marten Document)
```csharp
public class InventoryItem
{
    public Guid Id { get; set; }                  // MiniatureId является первичным ключом Id
    public string MiniatureName { get; set; } = string.Empty;
    public int AvailableStock { get; set; }
    public int ReservedStock { get; set; }
    public Guid Version { get; set; }             // Marten Optimistic Concurrency Guid Version
    public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
```

## 3. Обработчик событий OrderPlacedHandler
- Слушает `OrderPlacedEvent` из Transactional Outbox.
- Для каждой позиции `OrderItemDto`:
  - Загружает `InventoryItem` по `MiniatureId`.
  - Если `AvailableStock < Quantity`, выбрасывает `InsufficientStockException`.
  - Уменьшает `AvailableStock -= Quantity`.
  - Сохраняет сессию Marten.
- При возникновении `Marten.Exceptions.ConcurrencyException`:
  - Вступает в действие Wolverine Retry Policy (до 3 попыток с повторным чтением актуального состояния документа).

## 4. Эндпоинты HTTP
| Метод | Путь | Назначение |
|---|---|---|
| `GET` | `/api/inventory` | Таблица всех складских остатков |
| `GET` | `/api/inventory/{miniatureId}` | Остаток по конкретной модели |
| `POST` | `/api/inventory/adjust` | Корректировка остатка (Admin: `MiniatureId`, `DeltaQuantity`) |
