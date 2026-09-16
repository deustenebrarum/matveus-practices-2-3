# SPEC_CATALOG.md — Спецификация модуля Catalog

## 1. Назначение
Хранение, поиск и категоризация каталога миниатюр, арсенала и сопутствующих товаров настольных варгеймов Warhammer 40,000 и Warhammer Age of Sigmar.

## 2. Модель данных (Marten Document)
```csharp
public class Miniature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Universe { get; set; } = "Warhammer 40,000"; // "Warhammer 40,000" | "Age of Sigmar"
    public string Faction { get; set; } = string.Empty;       // "Imperium", "Chaos", "Xenos", "Order", "Death", "Destruction"
    public string Subfaction { get; set; } = string.Empty;    // "Adeptus Astartes", "Necrons", etc.
    public decimal Price { get; set; }
    public string Material { get; set; } = "Citadel Plastic"; // "Citadel Plastic", "Finecast Resin"
    public string Scale { get; set; } = "28-32mm";
    public int BaseSizeMm { get; set; } = 32;
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];              // "Infantry", "Hero", "Vehicle", "StarterSet", "Paints"
    public List<WargearOption> WargearOptions { get; set; } = [];
    public bool IsFeatured { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
```

### Полиморфная иерархия WargearOption (JSONB)
```csharp
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MeleeWargear), "melee")]
[JsonDerivedType(typeof(RangedWargear), "ranged")]
[JsonDerivedType(typeof(SpecialEquipment), "special")]
public abstract record WargearOption(string Name, int PointsCost);

public record MeleeWargear(string Name, int PointsCost, string WeaponSkill, int StrengthModifier, int Damage)
    : WargearOption(Name, PointsCost);

public record RangedWargear(string Name, int PointsCost, int RangeInches, int Attacks, int ArmorPenetration, string SpecialRules)
    : WargearOption(Name, PointsCost);

public record SpecialEquipment(string Name, int PointsCost, string EffectDescription)
    : WargearOption(Name, PointsCost);
```

## 3. Индексация Marten
- GIN-индекс по полю `Tags` для мгновенной фильтрации по категориям (`Infantry`, `StarterSet`).
- B-Tree составной индекс по `(Universe, Faction, Price)`.

## 4. Скомпилированный запрос (CompiledQuery)
```csharp
public class MiniaturesByFactionQuery : ICompiledListQuery<Miniature>
{
    public string Faction { get; set; } = string.Empty;
    public Expression<Func<IQueryable<Miniature>, IEnumerable<Miniature>>> QueryIs() =>
        q => q.Where(x => x.Faction == Faction).OrderBy(x => x.Price);
}
```

## 5. Эндпоинты HTTP
| Метод | Путь | Назначение | Параметры |
|---|---|---|---|
| `GET` | `/api/catalog` | Поиск и фильтрация каталога | `q`, `universe`, `faction`, `tag`, `minPrice`, `maxPrice`, `sort`, `page`, `pageSize` |
| `GET` | `/api/catalog/{id}` | Детальная карточка миниатюры | `id: Guid` |
| `GET` | `/api/catalog/factions` | Статистика фракций и числа товаров | - |
| `POST` | `/api/catalog` | Добавление миниатюры (Admin) | JSON Miniature |
