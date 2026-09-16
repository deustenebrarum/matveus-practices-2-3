using System.Text.Json;
using Shouldly;
using VaultCore.Modules.Catalog.Models;
using Xunit;

namespace VaultCore.Tests.Unit;

public class PolymorphicWargearSerializationTests
{
    [Fact]
    public void SerializeAndDeserialize_PolymorphicWargearCollection_PreservesConcreteSubtypes()
    {
        var original = new List<WargearOption>
        {
            new MeleeWargear("Thunder Hammer", 15, "3+", 4, 3),
            new RangedWargear("Plasma Cannon", 20, 36, 3, -3, "Blast, Hazardous"),
            new SpecialEquipment("Iron Halo", 10, "4+ invulnerable saving throw")
        };

        var json = JsonSerializer.Serialize(original);

        // Verify type discriminators in JSON
        json.ShouldContain("\"$type\":\"melee\"");
        json.ShouldContain("\"$type\":\"ranged\"");
        json.ShouldContain("\"$type\":\"special\"");

        var deserialized = JsonSerializer.Deserialize<List<WargearOption>>(json);

        deserialized.ShouldNotBeNull();
        deserialized.Count.ShouldBe(3);

        var melee = deserialized[0].ShouldBeOfType<MeleeWargear>();
        melee.Name.ShouldBe("Thunder Hammer");
        melee.WeaponSkill.ShouldBe("3+");
        melee.StrengthModifier.ShouldBe(4);
        melee.Damage.ShouldBe(3);

        var ranged = deserialized[1].ShouldBeOfType<RangedWargear>();
        ranged.Name.ShouldBe("Plasma Cannon");
        ranged.RangeInches.ShouldBe(36);
        ranged.SpecialRules.ShouldContain("Hazardous");

        var special = deserialized[2].ShouldBeOfType<SpecialEquipment>();
        special.Name.ShouldBe("Iron Halo");
        special.EffectDescription.ShouldContain("4+ invulnerable");
    }
}
