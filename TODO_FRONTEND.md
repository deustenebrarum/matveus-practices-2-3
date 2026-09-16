# TODO_FRONTEND.md — План реализации и чеклист Frontend (vault-browser)

Проект: **vault-browser** (Svelte 5 / Vite / TypeScript / Tailwind CSS / SCSS)  
Дизайн-система: **Gothic Imperial Reliquary** (`design/index.html`, `design/account.html`, `DESIGN.md`: гримдарк-палитра, Cinzel, Inter, Work Sans, EB Garamond, готические рамки, 4 металлические золотые L-скобы, акценты имперского золота `#c5a059` и реликтового кармина `#8a1c1c`).  
Стандарт реактивности: **Строго Svelte 5 Runes** (`$state`, `$derived`, `$props`, `$effect`). Полный запрет на legacy `export let` и `$:`!

---

## Порядок выполнения задач (Execution Pipeline)

### Этап 1. Инициализация проекта и конфигурация окружения
- [x] 1.1. Создать проект `src/vault-browser/` на базе Vite + Svelte 5 + TypeScript.
- [x] 1.2. Установить и настроить Tailwind CSS и SCSS библиотеку с цветовыми токенами из `design/index.html` и `DESIGN.md`:
  - `vault-black` (`#0a0b0e`), `vault-obsidian` (`#101217`), `vault-slate` (`#171a22`), `vault-card` (`#141720`).
  - `gold` (`#c5a059`), `brightGold` (`#e5c07b`), `paleGold` (`#f3e5ab`), `goldBorder` (`#5b4a2d`).
  - `crimson` (`#8a1c1c`), `purityWax` (`#961d1d`), `parchment` (`#ebd9b2`).
  - SCSS файлы: `_variables.scss`, `_mixins.scss`, `_components.scss`, `main.scss`.
- [x] 1.3. Подключить шрифты Google Fonts: **Cinzel**, **Cinzel Decorative**, **Inter**, **Work Sans** и **EB Garamond**.
- [x] 1.4. Настроить алиасы путей и проксирование API запросов.

### Этап 2. Спецификация и TypeScript контракты (SDD)
- [x] 2.1. Создать спецификацию клиентских модулей `specs/SPEC_FRONTEND.md`.
- [x] 2.2. Создать файл типов `src/types/index.ts`:
  - Интерфейсы `Miniature`, `WargearOption` (`MeleeWargear`, `RangedWargear`, `SpecialEquipment`).
  - Типы перечислений `Universe` (`Warhammer 40,000`, `Age of Sigmar`), `FactionCount`.
  - Интерфейсы `CartItem`, `CustomerInfo`, `Order`, `OrderItem`, `CreateOrderRequest`, `InventoryItem`.
  - Типы фильтров каталога `CatalogFilterState`.

### Этап 3. Слой взаимодействия с API и клиентское состояние (Runes)
- [x] 3.1. Реализовать HTTP-клиент `src/lib/api.ts` с методами:
  - `fetchMiniatures()` — получение списка миниатюр с бэкенда с прозрачным fallback на `SEED_MINIATURES`.
  - `fetchMiniatureById(id)` — получение данных конкретной миниатюры.
  - `fetchFactions()` — получение списка фракций с подсчетом.
  - `createOrder(request)` — отправка заказа на backend (`/api/orders`).
  - `fetchUserOrders(email)` — загрузка истории заказов для ЛК.
  - `fetchInventory()` и `adjustInventory(req)` — складские операции админки с `InventoryModule`.
  - Механизм fallback / seed sync для бесперебойной работы в автономном режиме.
- [x] 3.2. Реализовать состояние корзины `src/lib/state/cart.svelte.ts` на Svelte 5 Runes:
  - `$state items` — список добавленных миниатюр с количеством и опциями вооружения.
  - `$state promoCode` — введенный промокод.
  - `$derived count` — общее число единиц товаров.
  - `$derived subtotal` — базовая стоимость.
  - `$derived hasBundleDiscount` и `bundleDiscount` — скидка на бандл 15% при 2+ товарах или стартере.
  - `$derived promoDiscount` — скидка по промокодам (`TERRA10`, `EMPEROR20`, `WARP-TITHE-10`).
  - `$derived total` — итоговая стоимость к оплате.
  - Методы: `addItem`, `removeItem`, `updateQuantity`, `applyPromo`, `clearCart`.
  - Сохранение корзины в `localStorage`.
- [x] 3.3. Реализовать глобальное состояние `src/lib/state/ui.svelte.ts`:
  - Управление активным представлением (`catalog`, `account`, `checkout`, `admin`).
  - Управление открытием/закрытием корзины (`openCart`, `closeCart`, `toggleCart`).
  - Управление просмотром модалки товара (`openProductModal`, `closeProductModal`).
  - Глобальный поиск, выбор сектора (вселенной), всплывающие уведомления (тосты).
- [x] 3.4. Реализовать состояние пользователя `src/lib/state/user.svelte.ts`:
  - Профиль командира, выбор геральдики, ранг лояльности (Ветеран 7%), активный трекер CDEK, сохраненные адреса доставки.

### Этап 4. Разработка библиотеки компонентов UI и экранов (Svelte 5 Runes)
- [x] 4.1. UI библиотека компонентов `src/components/ui/`:
  - `VaultButton.svelte` — переиспользуемые кнопки (`gold`, `outline`, `crimson`, `dark`).
  - `VaultBadge.svelte` — бейджи фракций с аутентичной геральдической палитрой и иконками.
  - `VaultCard.svelte` — готическая карточка с угловыми акцентами и эффектом наведения.
  - `CornerBrackets.svelte` — 4 металлические золотые L-скобы по углам блока.
  - `VaultModal.svelte` — модальное окно с мальтийским крестом `✠` и обработкой Escape.
  - `PuritySealBanner.svelte` — сургучная печать с пергаментным блоком скидки.
  - `TimelineStepper.svelte` — 4-шаговый индикатор трекинга доставки.
  - `NotificationToast.svelte` — всплывающие гримдарк-уведомления.
- [x] 4.2. `Header.svelte`:
  - Готический бар с золотой Аквилой Империума «WARHAMMER VAULT — The Emperor's Armoury».
  - Живой поиск миниатюр и снаряжения.
  - Навигационные ссылки («Catalog», «Account», «Armory Stock»).
  - Кнопка корзины с пульсацией, счетчиком `$derived(cart.count)` и суммой `$derived(cart.total)`.
- [x] 4.3. `CatalogView.svelte`:
  - Переключатель вселенных (Все / Warhammer 40,000 / Age of Sigmar).
  - Левый рельс фильтров (фракции, материалы, масштаб, цена ползунком $10-$250, наличие).
  - Сортировка по популярности, цене (возр/убыв) и названию.
  - Сетка карточек товаров со статусом наличия, ценой и кнопкой быстрого добавления «Add to Cart».
- [x] 4.4. `ProductDetailModal.svelte`:
  - Детальный осмотр товара с ракурсами фотогалереи (4 миниатюры).
  - Выбор конфигурации снаряжения (Wargear: Master-Crafted Power Sword, Heavy Bolt Pistol, Thunder Hammer и др.).
  - Цитата из имперского лора, ТТХ (масштаб, очки, фракция, остаток).
  - Селектор количества и кнопка «ADD TO CART».
- [x] 4.5. `CartDrawer.svelte`:
  - Выдвижная панель справа с оверлеем затемнения.
  - Список добавленных позиций с регулировкой количества (`−` / `+`) и удалением (`✕`).
  - Баннер сургучной печати бандл-скидки 15%.
  - Поле ввода промокода с валидацией (`TERRA10`, `EMPEROR20`, `WARP-TITHE-10`).
  - Расчет итогов и переход к оформлению заказа.
- [x] 4.6. `CheckoutView.svelte`:
  - Форма оформления: позывной командира, email, телефон, сектор/город, адрес ПВЗ / доставка.
  - Выбор службы доставки (CDEK Express, Direct Courier, Russian Post).
  - Отправка заказа в `POST /api/orders` и отображение экрана с Sanctified Order Number.
- [x] 4.7. `AccountView.svelte`:
  - Полное воспроизведение `design/account.html`.
  - Трекер активного отправления CDEK с 4-шаговой шкалой.
  - Карточка учетных данных командира с выбором любимой геральдики (Imperium, Chaos, Necrons, Orks).
  - Протокол лояльности Муниторума (ранг Ветеран 7%, шкала расходов, промокоды с копированием).
  - Архив истории заказов с возможностью повтора заказа («Repeat Order») и экспорта PDF.
  - Книга сохраненных адресов доставки с добавлением и переключением основного адреса.
- [x] 4.8. `AdminView.svelte`:
  - Панель управления складскими запасами Муниторума.
  - Таблица с остатками и резервами, синхронизированная с `InventoryModule`.
  - Оперативные кнопки корректировки запаса (`-5`, `-1`, `+1`, `+5`).
- [x] 4.9. `Footer.svelte`:
  - Подвал с аквилой, ссылками на разделы и печатью Муниторума.

### Этап 5. Браузерная верификация и сквозное тестирование (E2E)
- [x] 5.1. Настроить сборку `npm run build` — успешно собрано за <1 сек без предупреждений компилятора.
- [x] 5.2. Сквозные сценарии проверки:
  - Сценарий 1: Каталог загружается, переключение вселенных WH40K / AoS фильтрует товары.
  - Сценарий 2: Открытие модального окна миниатюры отображает корректные характеристики wargear и галерею.
  - Сценарий 3: Добавление товаров активирует бандл-скидку 15% и сургучную печать чистоты.
  - Сценарий 4: Применение промокода `TERRA10` или `WARP-TITHE-10` пересчитывает сумму.
  - Сценарий 5: Оформление заказа создает запись через API и переводит на экран подтверждения.
  - Сценарий 6: Просмотр оформленного заказа в ЛК `Account` по email покупателя.
  - Сценарий 7: Проверка отражения корректировок остатка в админ-панели склада `Armory Stock`.
- [x] 5.3. Проверить отсутствие ошибок в консоли и строгое соблюдение Svelte 5 Runes синтаксиса.
