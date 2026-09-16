# TODO_BACKEND.md — План реализации и чеклист Backend (VaultCore)

Проект: **VaultCore** (.NET 10 / C# 14 / ASP.NET Core 10 Web API / Critter Stack: Marten + Wolverine + Alba / PostgreSQL 16+)  
Архитектура: **Modular Monolith (Vertical Slice)** с изолированными модулями (`Catalog`, `Orders`, `Inventory`) и шиной сообщений Transactional Outbox.

---

## Порядок выполнения задач (Execution Pipeline)

### Этап 1. Инициализация структуры решения (.NET 10 Solution)
- [x] 1.1. Создать корневую директорию `src/VaultCore/`.
- [x] 1.2. Создать решение `VaultCore.sln` / `VaultCore.slnx` на платформе .NET 10 (`net10.0`).
- [x] 1.3. Создать проекты модульного монолита:
  - [x] `VaultCore.SharedKernel` (Class Library: контракты сообщений, события, результаты, общие DTO).
  - [x] `VaultCore.Modules.Catalog` (Class Library: доменные модели, Marten схемы, запросы, DTO).
  - [x] `VaultCore.Modules.Orders` (Class Library: оформление заказов, расчет скидок, FluentValidation, outbox события).
  - [x] `VaultCore.Modules.Inventory` (Class Library: учет остатков, Marten optimistic concurrency, обработчики Wolverine).
  - [x] `VaultCore.Api` (Web API: точка входа, конфигурация Marten/Wolverine, Swagger/OpenAPI, health checks, seed runner).
  - [x] `VaultCore.Tests.Unit` (xUnit + Shouldly: юнит-тесты доменной логики и валидации).
  - [x] `VaultCore.Tests.Integration` (xUnit + Alba + Shouldly + PostgreSQL: интеграционные тесты API и Transactional Outbox).
- [x] 1.4. Настроить проектные ссылки и зависимости между проектами в соответствии с границами модулей.

### Этап 2. Конфигурация ядра и зависимостей (Critter Stack)
- [x] 2.1. Подключить пакеты Marten (v7+) и Weasel Schema Management.
- [x] 2.2. Подключить пакеты WolverineFx, WolverineFx.Http, WolverineFx.Postgresql (Transactional Outbox).
- [x] 2.3. Подключить FluentValidation, Shouldly, Alba, xUnit.
- [x] 2.4. Настроить `Program.cs` с регистрацией Marten (PostgreSQL подключение, Weasel AutoCreate, полиморфные типы wargear).
- [x] 2.5. Настроить Wolverine в `Program.cs` с обнаружением обработчиков сообщений и маршрутизацией эндпоинтов.
- [x] 2.6. Настроить CORS для взаимодействия с frontend (`vault-browser`).

### Этап 3. Реализация модуля «Каталог» (CatalogModule)
- [x] 3.1. Создать спецификацию модуля `specs/SPEC_CATALOG.md`.
- [x] 3.2. Определить доменную модель `Miniature`:
  - Идентификатор (Guid), название, описание, вселенная (`Warhammer40K`, `AgeOfSigmar`), фракция (`Imperium`, `Chaos`, `Xenos`, `Order`, `Death`, `Destruction`), подфракция.
  - Цена, материал (`CitadelPlastic`, `FinecastResin`), масштаб (28-32 мм), размер базы (32 мм, 40 мм и т.д.).
  - Теги сборки и категории (`Infantry`, `Vehicle`, `Hero`, `StarterSet`, `Paints`).
  - Коллекция опций вооружения `List<WargearOption>` с полиморфной иерархией:
    - `MeleeWargear` (Name, WeaponSkill, Strength, Damage).
    - `RangedWargear` (Name, RangeInches, Attacks, ArmorPenetration, SpecialRules).
    - `SpecialEquipment` (Name, EffectDescription).
- [x] 3.3. Настроить схему Marten с индексом по фракциям и GIN-индексом по JSONB-тегам.
- [x] 3.4. Реализовать скомпилированный запрос Marten `CompiledQuery` для высокопроизводительной фильтрации каталога.
- [x] 3.5. Реализовать Wolverine HTTP Handlers:
  - `GET /api/catalog` — фильтрация по вселенной, фракции, категории, поиск по названию, пагинация.
  - `GET /api/catalog/{id}` — получение подробной карточки миниатюры.
  - `GET /api/catalog/factions` — получение перечня активных фракций с количеством товаров.

### Этап 4. Реализация модуля «Заказы» (OrdersModule)
- [x] 4.1. Создать спецификацию модуля `specs/SPEC_ORDERS.md`.
- [x] 4.2. Определить доменные сущности `Order`, `OrderItem`, `CustomerInfo`, `OrderStatus` (Pending, Confirmed, Shipped, Cancelled).
- [x] 4.3. Реализовать валидацию команды `CreateOrderCommand` через FluentValidation:
  - Проверка имени, корректности email, телефона, адреса доставки.
  - Проверка наличия позиций и количества (> 0).
- [x] 4.4. Реализовать бизнес-логику расчета стоимости:
  - Бандл-скидка: 10% при покупке стартового набора или >= 3 миниатюр одной фракции.
  - Поддержка промокодов (`WARHAMMER10` — скидка 10%, `EMPEROR20` — скидка 20%).
  - Расчет накопительной скидки лояльности клиента.
- [x] 4.5. Реализовать генерацию и отправку события `OrderPlacedEvent` через Transactional Outbox Wolverine.
- [x] 4.6. Реализовать Wolverine HTTP Handlers:
  - `POST /api/orders` — создание заказа с валидацией, расчетом цены и публикацией события.
  - `GET /api/orders/{id}` — получение статуса и состава заказа.
  - `GET /api/orders/user/{email}` — история заказов клиента для личного кабинета.

### Этап 5. Реализация модуля «Склад» (InventoryModule)
- [x] 5.1. Создать спецификацию модуля `specs/SPEC_INVENTORY.md`.
- [x] 5.2. Определить модель документа Marten `InventoryItem` (MiniatureId, StockQuantity, ReservedQuantity, Version).
- [x] 5.3. Настроить оптимистическую конкурентность Marten (`[Version]`) для защиты от состояния гонки при списании остатков.
- [x] 5.4. Реализовать Wolverine Message Handler для `OrderPlacedEvent`:
  - Проверка достаточности складского остатка.
  - Атомарное списание остатка с контролем версии документа.
  - Wolverine Retry Policy (автоматический повтор с экспоненциальной задержкой при `ConcurrencyException`).
- [x] 5.5. Реализовать HTTP Handlers склада:
  - `GET /api/inventory` — получение таблицы складских остатков всех миниатюр.
  - `GET /api/inventory/{miniatureId}` — остаток по конкретной модели.
  - `POST /api/inventory/adjust` — административная корректировка количества на складе.

### Этап 6. Наполнение витрины и сидирование (Warhammer Seed Data)
- [x] 6.1. Подготовить реалистичный набор данных миниатюр Warhammer 40,000 и Warhammer Age of Sigmar:
  - Космодесант: Intercessors Squad, Captain in Terminator Armor, Dreadnought Redemptor.
  - Силы Хаоса: Chaos Space Marines Legionaries, Abaddon the Despoiler.
  - Ксеносы: Necron Warriors with Canoptek Scarabs, Ork Boyz Mob.
  - Age of Sigmar: Stormcast Eternals Liberators, Skaven Clanrats Vermintide.
  - Стартовые наборы и модельная химия (Citadel Shade, Starter Set WH40k).
- [x] 6.2. Настроить сервис автоматического посева (`SeedDataService`) при запуске приложения в dev-режиме.

### Этап 7. Модульное и интеграционное тестирование
- [x] 7.1. Разработать модульные тесты в `VaultCore.Tests.Unit`:
  - Валидация `CreateOrderCommand` (граничные значения, некорректный email, пустая корзина).
  - Алгоритм расчета бандл-скидок и промокодов.
  - Моделирование полиморфного вооружения `WargearOption`.
- [x] 7.2. Разработать интеграционные тесты в `VaultCore.Tests.Integration`:
  - Настройка фикстуры PostgreSQL с автоматическим созданием схем Weasel.
  - Тест сценария Alba: `GET /api/catalog` с фильтрами по фракциям и цене.
  - Тест сценария Alba: `POST /api/orders` → проверка сохранения заказа в Marten.
  - Тест сквозного Transactional Outbox: `OrderPlacedEvent` → успешное списание остатка в `InventoryItem`.
  - Тест конкурентного списания: симуляция одновременной покупки последнего товара (проверка оптимистической блокировки и отката при нехватке).
- [x] 7.3. Выполнить запуск тестов `dotnet test` и убедиться в успешном прохождении 100% тестов.
