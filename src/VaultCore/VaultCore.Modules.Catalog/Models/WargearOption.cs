using System.Text.Json.Serialization;

namespace VaultCore.Modules.Catalog.Models;

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
