# SPEC_FRONTEND.md — Спецификация Frontend (vault-browser)

## 1. Стек и технологические инварианты
- **Фреймворк:** Svelte 5 (Vite + TypeScript + Tailwind CSS v4 + SASS/SCSS).
- **Инвариант реактивности:** Строго Svelte 5 Runes (`$state`, `$derived`, `$props`, `$effect`). Legacy-синтаксис Svelte 3/4 (`export let`, `$:`) категорически запрещен.
- **Дизайн-система:** Полное соответствие прототипам `design/index.html` и `design/account.html` (стиль **Gothic Imperial Reliquary**, шрифты Cinzel, Cinzel Decorative, Inter, Work Sans, EB Garamond, палитра Grimdark с золотыми акцентами `#c5a059`, кармином `#8a1c1c`, обсидианом `#101217`, металлическими рамками и L-скобами).
- **SCSS библиотека:** `src/styles/` (`_variables.scss`, `_mixins.scss`, `_components.scss`, `main.scss`).
- **Глобальная доступность header-функций:** Корзина, поиск, навигация между витриной, досье командира и складом доступны отовсюду через реактивные синглтоны состояния (`ui.svelte.ts`, `cart.svelte.ts`, `user.svelte.ts`).

## 2. Архитектура UI библиотеки компонентов (`src/components/ui/`)
1. `VaultButton.svelte` — имперская кнопка с золотым градиентом, контурным вариантом или карминовым стилем.
2. `VaultBadge.svelte` — аутентичный шеврон фракции с геральдической палитрой (Imperium gold, Chaos crimson, Necron emerald, Ork amber, Order sky).
3. `VaultCard.svelte` — гримдарк карточка с угловыми псевдоэлементами и золотым свечением при наведении.
4. `CornerBrackets.svelte` — 4 металлические золотые L-скобы (`.bracket-tl`, `.bracket-tr`, `.bracket-bl`, `.bracket-br`).
5. `VaultModal.svelte` — модальное окно с мальтийским крестом `✠` в навершии (`.vault-frame-ornate`), обработкой Escape и клика вне окна.
6. `PuritySealBanner.svelte` — сургучная печать чистоты с пергаментной полосой скидки на бандл.
7. `TimelineStepper.svelte` — 4-шаговый индикатор отслеживания заказа с пульсирующим активным этапом транзита.
8. `NotificationToast.svelte` — гримдарк всплывающие уведомления о добавлении в арсенал, промокодах и ошибках.

## 3. Модули страниц и представлений SPA
- `Header.svelte` — липкий готический заголовок с Имперской Аквилой, живым поиском, переключателями страниц и счетчиком корзины.
- `CatalogView.svelte` — витрина с левым рельсом фильтров (фракции, материалы, масштаб, цена слайдером, наличие), сортировкой и сеткой реликвий.
- `ProductDetailModal.svelte` — детальный осмотр миниатюры, фотогалерея ракурсов, выбор комплектации вооружения (Wargear), цитата из лора, добавление в арсенал.
- `CartDrawer.svelte` — выдвижная панель корзины справа, управление количеством, активация промокодов (`TERRA10`, `EMPEROR20`, `WARP-TITHE-10`), расчет скидок.
- `CheckoutView.svelte` — протокол освящения заказа, сбор координат сектора и службы доставки, отправка в `POST /api/orders`, экран подтверждения.
- `AccountView.svelte` — Досье Командира: активный трекер CDEK, геральдический выбор, протокол лояльности (ранг Ветеран 7%), архив заказов и книга адресов доставки.
- `AdminView.svelte` — панель логистики склада Муниторума: остатки, резервы, оперативные корректировки (`adjustInventory`).
- `Footer.svelte` — подвал с аквилой, сертификацией Муниторума и навигационными ссылками.

## 4. Контракты данных и API-клиент (`src/lib/api.ts`)
- `fetchMiniatures(filter)` — запрос каталога к `/api/catalog` с прозрачным fallback на 9 стартовых позиций `SEED_MINIATURES`.
- `fetchMiniatureById(id)` — получение характеристик конкретной единицы.
- `fetchFactions()` — группировка фракций и числа юнитов.
- `createOrder(request)` — отправка заказа на backend (`/api/orders`) с генерацией Sanctified Order ID.
- `fetchUserOrders(email)` — выборка истории заказов из БД Marten.
- `fetchInventory()` / `adjustInventory(req)` — складские операции с `InventoryModule`.
