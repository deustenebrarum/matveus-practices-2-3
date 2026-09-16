# SPEC_OVERVIEW.md — Общая системная спецификация Vault

## 1. Архитектурный паттерн
- **Тип системы:** Модульный монолит (Modular Monolith) с вертикальными срезами (Vertical Slices).
- **Среда выполнения:** .NET 10 (C# 14), Kestrel HTTP Server.
- **Хранилище данных:** PostgreSQL 16+ с JSONB-документным движком Marten (Critter Stack).
- **Шина межмодульного взаимодействия:** Wolverine In-Process Bus с Transactional Outbox.
- **Клиент:** Svelte 5 SPA (Vite + TypeScript + Tailwind CSS).

## 2. Изоляция модулей
| Модуль | Ответственность | Таблицы / Документы Marten | События Outbox |
|---|---|---|---|
| `Catalog` | Каталог миниатюр, опции wargear, поиск, фильтрация | `mt_doc_miniature` | `MiniatureCreatedEvent` |
| `Orders` | Корзина, расчет скидок, валидация, оформление заказов | `mt_doc_order` | `OrderPlacedEvent` |
| `Inventory` | Складские остатки, резервирование, списание с контролем версий | `mt_doc_inventoryitem` | `InventoryDeductedEvent`, `StockExhaustedEvent` |

## 3. Сетевые порты и адреса
- Backend Web API: `http://localhost:5000` (Swagger: `http://localhost:5000/swagger`)
- Frontend SPA: `http://localhost:5173`
- PostgreSQL: `localhost:5432` (База: `vaultcore_dev`, `vaultcore_test`)

## 4. Контракт межмодульного события OrderPlacedEvent
```csharp
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
```
- Гарантия доставки: Transactional Outbox в PostgreSQL.
- Обработчик: `VaultCore.Modules.Inventory.Handlers.OrderPlacedHandler`.
- Политика обработки коллизий: Wolverine Retry Policy (3 попытки с задержкой 100ms, 250ms, 500ms при `ConcurrencyException`).
