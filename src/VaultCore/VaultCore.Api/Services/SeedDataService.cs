using Marten;
using Microsoft.Extensions.Logging;
using VaultCore.Modules.Catalog.Models;
using VaultCore.Modules.Inventory.Models;

namespace VaultCore.Api.Services;

public class SeedDataService
{
    private readonly IDocumentStore _store;
    private readonly ILogger<SeedDataService> _logger;

    public SeedDataService(IDocumentStore store, ILogger<SeedDataService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await using var session = _store.LightweightSession();

        var existingCount = await session.Query<Miniature>().CountAsync();
        if (existingCount > 0)
        {
            _logger.LogInformation("Database already seeded with {Count} miniatures.", existingCount);
            return;
        }

        _logger.LogInformation("Seeding initial Warhammer miniatures and inventory...");

        var miniatures = GetSeedMiniatures();
        foreach (var m in miniatures)
        {
            session.Store(m);

            var inv = new InventoryItem
            {
                Id = m.Id,
                MiniatureName = m.Name,
                AvailableStock = m.Tags.Contains("StarterSet") ? 10 : 25,
                ReservedStock = 0,
                LastUpdatedAt = DateTimeOffset.UtcNow
            };
            session.Store(inv);
        }

        await session.SaveChangesAsync();
        _logger.LogInformation("Successfully seeded {Count} miniatures and inventory records.", miniatures.Count);
    }

    public static List<Miniature> GetSeedMiniatures()
    {
        return
        [
            new Miniature
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Roboute Guilliman, Primarch of the Ultramarines",
                Description = "Lord Commander of the Imperium, wielding the burning blade of the Emperor Himself.",
                Universe = "Warhammer 40,000",
                Faction = "Imperium",
                Subfaction = "Adeptus Astartes",
                Price = 12500m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 60,
                ImageUrl = "/images/guilliman.jpg",
                Tags = ["Hero", "Imperium", "Adeptus Astartes", "Primarch"],
                IsFeatured = true,
                WargearOptions =
                [
                    new MeleeWargear("The Emperor's Sword", 0, "2+", 4, 3),
                    new RangedWargear("Hand of Dominion", 0, 24, 6, -2, "Rapid Fire 3"),
                    new SpecialEquipment("Armour of Fate", 0, "3+ Invulnerable save, resurrect on 2+ with 6 wounds.")
                ]
            },
            new Miniature
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Space Marines: Intercessor Squad",
                Description = "The backbone of Primaris infantry formations, providing relentless tactical firepower.",
                Universe = "Warhammer 40,000",
                Faction = "Imperium",
                Subfaction = "Adeptus Astartes",
                Price = 5200m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 32,
                ImageUrl = "/images/intercessors.jpg",
                Tags = ["Infantry", "Imperium", "Adeptus Astartes", "Troops"],
                IsFeatured = true,
                WargearOptions =
                [
                    new RangedWargear("Bolt Rifle", 0, 24, 2, -1, "Assault, Heavy"),
                    new MeleeWargear("Astartes Chainsword", 5, "3+", 0, 1)
                ]
            },
            new Miniature
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Warhammer 40,000: Ultimate Starter Set",
                Description = "Complete battle set including 44 push-fit Citadel miniatures, terrain, boards, dice, and full rulebook.",
                Universe = "Warhammer 40,000",
                Faction = "Imperium",
                Subfaction = "Adeptus Astartes",
                Price = 16900m,
                Material = "Citadel Plastic",
                Scale = "28-32mm",
                BaseSizeMm = 40,
                ImageUrl = "/images/starter_set.jpg",
                Tags = ["StarterSet", "Imperium", "Tyranids", "BoxSet"],
                IsFeatured = true,
                WargearOptions =
                [
                    new SpecialEquipment("Imperial Rulebook & Dice Set", 0, "Comprehensive rules and measurement gauges.")
                ]
            },
            new Miniature
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Abaddon the Despoiler, Warmaster of Chaos",
                Description = "Supreme Commander of the Black Legion, chosen champion of the Ruinous Powers.",
                Universe = "Warhammer 40,000",
                Faction = "Chaos",
                Subfaction = "Black Legion",
                Price = 6800m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 60,
                ImageUrl = "/images/abaddon.jpg",
                Tags = ["Hero", "Chaos", "Black Legion", "Warmaster"],
                IsFeatured = true,
                WargearOptions =
                [
                    new MeleeWargear("Drach'nyen", 0, "2+", 5, 3),
                    new RangedWargear("Talon of Horus", 0, 24, 4, -2, "Combi-Bolter")
                ]
            },
            new Miniature
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Chaos Space Marines Legionaries",
                Description = "Veterans of ten millennia of heresy, sworn to tear down the False Emperor.",
                Universe = "Warhammer 40,000",
                Faction = "Chaos",
                Subfaction = "Heretic Astartes",
                Price = 5400m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 32,
                ImageUrl = "/images/legionaries.jpg",
                Tags = ["Infantry", "Chaos", "Heretic Astartes", "Troops"],
                IsFeatured = false,
                WargearOptions =
                [
                    new RangedWargear("Boltgun", 0, 24, 2, 0, "Standard issue"),
                    new MeleeWargear("Daemon blade", 10, "3+", 2, 2)
                ]
            },
            new Miniature
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "Necron Warriors with Canoptek Scarabs",
                Description = "Ancient soulless android legions marching forward with terrifying inevitability.",
                Universe = "Warhammer 40,000",
                Faction = "Xenos",
                Subfaction = "Necrons",
                Price = 4500m,
                Material = "Citadel Plastic",
                Scale = "28mm",
                BaseSizeMm = 32,
                ImageUrl = "/images/necrons.jpg",
                Tags = ["Infantry", "Xenos", "Necrons", "Troops"],
                IsFeatured = false,
                WargearOptions =
                [
                    new RangedWargear("Gauss Flayer", 0, 24, 1, -1, "Lethal Hits")
                ]
            },
            new Miniature
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Name = "Stormcast Eternals: Liberators Squad",
                Description = "Sigmar's golden-armored crusaders hurled from the celestial heavens onto Mortal Realms.",
                Universe = "Age of Sigmar",
                Faction = "Order",
                Subfaction = "Stormcast Eternals",
                Price = 4800m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 40,
                ImageUrl = "/images/liberators.jpg",
                Tags = ["Infantry", "Order", "Stormcast Eternals", "Troops"],
                IsFeatured = true,
                WargearOptions =
                [
                    new MeleeWargear("Warhammer & Sigmarite Shield", 0, "3+", 1, 2)
                ]
            },
            new Miniature
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Name = "Skaven: Clanrats Vermintide Mob",
                Description = "A chittering tidal wave of ratmen armed with rusty spears and diseased daggers.",
                Universe = "Age of Sigmar",
                Faction = "Chaos",
                Subfaction = "Skaven",
                Price = 4100m,
                Material = "Citadel Plastic",
                Scale = "28mm",
                BaseSizeMm = 25,
                ImageUrl = "/images/clanrats.jpg",
                Tags = ["Infantry", "Chaos", "Skaven", "Troops"],
                IsFeatured = false,
                WargearOptions =
                [
                    new MeleeWargear("Rusty Spear", 0, "4+", 0, 1)
                ]
            },
            new Miniature
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Name = "Citadel Colour: Base Paint Set & Shade",
                Description = "Essential acrylic paints for basecoating, shading, and technical weathering.",
                Universe = "Warhammer 40,000",
                Faction = "Imperium",
                Subfaction = "Citadel Supplies",
                Price = 3900m,
                Material = "Acrylic Paint",
                Scale = "Standard 12ml pots",
                BaseSizeMm = 0,
                ImageUrl = "/images/citadel_paints.jpg",
                Tags = ["Paints", "Hobby", "Tools"],
                IsFeatured = false,
                WargearOptions =
                [
                    new SpecialEquipment("11 Citadel Base Paints & 1 Wash", 0, "Nuln Oil, Retributor Armour, Macragge Blue, etc.")
                ]
            }
        ];
    }
}
