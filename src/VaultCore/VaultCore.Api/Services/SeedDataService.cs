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

        _logger.LogInformation("Syncing initial Warhammer miniatures and inventory in USD...");

        var miniatures = GetSeedMiniatures();
        foreach (var m in miniatures)
        {
            session.Store(m);

            var existingInv = await session.LoadAsync<InventoryItem>(m.Id);
            if (existingInv is null)
            {
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
        }

        await session.SaveChangesAsync();
        _logger.LogInformation("Successfully synced {Count} miniatures and inventory records in USD.", miniatures.Count);
    }

    public static List<Miniature> GetSeedMiniatures()
    {
        return
        [
            new Miniature
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Roboute Guilliman, Primarch of the Ultramarines",
                Description = "Lord Commander of the Imperium, wielding the burning blade of the Emperor Himself. Armed with ancient chapter relics and unyielding faith in the Golden Throne.",
                Universe = "Warhammer 40,000",
                Faction = "Imperium",
                Subfaction = "Adeptus Astartes",
                Price = 65.00m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 60,
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAvrktdY3q6xdAIuPjE0b34GVMYg9lwLvNdBQNC7loqq9AxB3EaWDhCfqORxN01S6iAY-hceugVnndPPZOUvnP0CWBtJmgk3XfRWZsQpjX9RzGUC37ltKBzZVcLI6IHE8CobjltV46D0D-o4N1HcXbU3IpPndAKvMkVE3KKqlId-z6mc5dMiLBKICoAmMJzKnJg3Q2VFGDKmiaU9CtXY9OlC2-7BVAhmLLK2QJDHrVlqJp_kcDwquU",
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
                Description = "The tactical backbone of Primaris infantry formations, providing relentless, flexible firepower across all engagement envelopes.",
                Universe = "Warhammer 40,000",
                Faction = "Imperium",
                Subfaction = "Adeptus Astartes",
                Price = 60.00m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 32,
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBKRaab5L7LIKbwJDE_7SrDtmjNt79FlTVGK5MV13rEGCq6RdA0qnS1HP_MMUmeKERtCkc-BKSn5_u6GGQd2qfRzW7wqJlhejzX5FWMntwjnQ720Bhw96AVewLj3YnNAECLzzzfytisYy1zotoE5vHdkGxxSECkWpoV1o9VXUFW1tyxyUIXjCAGLNsi18VoG5JN5epJhPzy7ADG3mhFJeaCuoZvQwymncJyVCbRu6ZYP7HuejiZ5xI",
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
                Price = 120.00m,
                Material = "Citadel Plastic",
                Scale = "28-32mm",
                BaseSizeMm = 40,
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuB_9V-tJHe4D7WwYeTv4uBYT2zlFmzUXfbqbWb7JZ69tmXplaTPPTky-_4s4YSp3WRi5DcsbSK4MJBnKRjBMGGeQ0dDkO_YAPAWsewk_miuZ3UF7Gxp7Albd2djKjeUm2rHgprCU4YQgNuNwOAWe8uF1jHnhfCI57MGpD_Atnl7LZd7cFTsKFuSD4vNhZCgCilVJDW8oYbHE9jUl-NqThkXGrRZfjLTq5ZZtQO97ly3XXE6C038Vfs",
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
                Description = "Supreme Commander of the Black Legion, chosen champion of the Ruinous Powers, wielding the Talon of Horus and the daemon sword Drach'nyen.",
                Universe = "Warhammer 40,000",
                Faction = "Chaos",
                Subfaction = "Black Legion",
                Price = 70.00m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 60,
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCDb8lNEnNNFviDCFAaqTRGy5loL1XcOODtuOB2Zft7-MUYIWHPDin4BQMxFKkf3JQUAwbD1V_j_NTbu0d2bPn4zHsRbCBFReaBapiir31deS7XdRGYeY7Y_FT7DcLghOUKoNpYDWYACExmQIg3w6KIq-PHGo-dBVvOKe5rbq_gTyL2BAh2jRz1DihPBHsuUIEPRTkCDoy8lMYcKjuxpbZhadioRHmM0yuNHeTYJYAUkIY17k9I72g",
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
                Description = "Veterans of ten millennia of heresy, sworn to tear down the False Emperor and subjugate the galaxy to the warp.",
                Universe = "Warhammer 40,000",
                Faction = "Chaos",
                Subfaction = "Heretic Astartes",
                Price = 60.00m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 32,
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCZpvbZTTNFLuVr53faNe6zwmgdTCmnhcSxP7lKmIMFYuLk7BP9eZQ25ZTEIcaO4IBuRCb3FYEiQu_uRvhkps1_N8ww0Ouvd41yzvZsNR9hoX_WJNWPVWk8s7ix3wsUss1-PO84fAS0SdetchRbBx0DDdHi68taWfAGH-2x_bvohwnqKN3fBg4XtANakB2uf4EbVjCrb_TcsvjHyVsWEyl_25GayLeKtgH8cGLv8P9EnPDSJmSzyfY",
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
                Description = "Ancient soulless android legions marching forward with terrifying inevitability and gauss flayer salvos.",
                Universe = "Warhammer 40,000",
                Faction = "Xenos",
                Subfaction = "Necrons",
                Price = 50.00m,
                Material = "Citadel Plastic",
                Scale = "28mm",
                BaseSizeMm = 32,
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuB3D2tP4c1JIFlL4EXEGLvpLtj__xnMxy7dWwni0WjDCbsOrC9EUH8E334jajk4Q1ZiOEoY5XfyP0whf2zMQ6cy0FO1EheyGMPC4x9Zo91jnP2NL6NpiZdoI84R6JioRAeD-k-oVFuwuq1vspyActu8VycUaYX7hmVHKkW5xYnJaWCV-fkEFTOUbKESBADOGEgAHUsWgdD87NJSxTgzD6RqfP_7a_wcwONJN8o8XfgLPgevue0J9jE",
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
                Description = "Sigmar's golden-armored crusaders hurled from the celestial heavens onto Mortal Realms to smite chaos.",
                Universe = "Age of Sigmar",
                Faction = "Order",
                Subfaction = "Stormcast Eternals",
                Price = 55.00m,
                Material = "Citadel Plastic",
                Scale = "32mm",
                BaseSizeMm = 40,
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBxNa6rIn3rpi0X7fTquTnnXpbhfPRge9JvYu_fCw53xpgUPoPtC-mXdGzGDqRnaS9xMBMNJ_uARoeM1VLTyIW1nLJeYXhEKwaNxQVyHGAIXbZsnEI1tIkvoiN2YM-CHrUyGiW6WqAs_VP9Um4EMomL1LWe6w-GFByor_oAnRelOGM8VqQ6svV6xKrrm9uCFF5ORH01zPCCBkKvFm8g3LtE0fChSacsyhNR2PD2St6ApDLZWLnCj-o",
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
                Description = "A chittering tidal wave of ratmen armed with rusty spears and diseased daggers, overwhelming enemies by sheer mass.",
                Universe = "Age of Sigmar",
                Faction = "Chaos",
                Subfaction = "Skaven",
                Price = 45.00m,
                Material = "Citadel Plastic",
                Scale = "28mm",
                BaseSizeMm = 25,
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAnrlmIFE2mo5sBm48HGhY2LA-6ysyzEY9DC-BJrrFYulcQskW4qOXKOA6lGRuIwXJ929YZzscAqJ1WsL7MG23sjQyFAjyWmb-EFN31njtBSQTAOxAFqywvCdAyJoeofVIyCJ3v7_pq3yVPNLtrkXC94wPahcgO91u8XGetrHQlAt13TGBRN-GQUi9uB9AYLl8B04qX0fRQR6bLxSnywo3RMIfG9ailStFjsQAH61flGsJOEA-TXpg",
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
                Description = "Essential acrylic paints for basecoating, shading, and technical weathering across Citadel miniatures.",
                Universe = "Warhammer 40,000",
                Faction = "Imperium",
                Subfaction = "Citadel Supplies",
                Price = 45.00m,
                Material = "Acrylic Paint",
                Scale = "Standard 12ml pots",
                BaseSizeMm = 0,
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuC_ACLw9p31Jh1kwE_qI64Yc9_I70cB16fQq0swI-7Oz_GLOXiblkUrOxkDXZxO2jkSnqTEGzKERsfMRfOOp6sNAmKM6iJReJBZraSFnnf-F2-hS3Ik6emRbetv5M7zRHE8FyXWUZX4K8MDfDAnZDPufdsmlpv-POIK4ufkvmGqlgwu_A4nnAL5VVdZCv4JlQXlhyAdWr5Ql_VK5RA4KqfJxRcbGKapdNBPGC1q49SpGoJoVjwQJSU",
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
