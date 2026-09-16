using Shouldly;
using VaultCore.Modules.Orders.Models;
using VaultCore.Modules.Orders.Services;
using Xunit;

namespace VaultCore.Tests.Unit;

public class DiscountCalculatorTests
{
    [Fact]
    public void Calculate_EmptyCart_ReturnsZero()
    {
        var result = DiscountCalculator.Calculate([], null);

        result.Subtotal.ShouldBe(0);
        result.BundleDiscount.ShouldBe(0);
        result.PromoDiscount.ShouldBe(0);
        result.Total.ShouldBe(0);
    }

    [Fact]
    public void Calculate_StandardItemsWithoutDiscounts_CalculatesExactTotal()
    {
        var items = new List<OrderItem>
        {
            new() { MiniatureId = Guid.NewGuid(), MiniatureName = "Intercessor", Faction = "Imperium", UnitPrice = 1000m, Quantity = 2, IsStarterSet = false }
        };

        var result = DiscountCalculator.Calculate(items, null);

        result.Subtotal.ShouldBe(2000m);
        result.BundleDiscount.ShouldBe(0m);
        result.PromoDiscount.ShouldBe(0m);
        result.Total.ShouldBe(2000m);
    }

    [Fact]
    public void Calculate_ThreeItemsFromSameFaction_AppliesTenPercentBundleDiscount()
    {
        var items = new List<OrderItem>
        {
            new() { MiniatureId = Guid.NewGuid(), MiniatureName = "Intercessors", Faction = "Imperium", UnitPrice = 1000m, Quantity = 3, IsStarterSet = false }
        };

        var result = DiscountCalculator.Calculate(items, null);

        result.Subtotal.ShouldBe(3000m);
        result.BundleDiscount.ShouldBe(300m); // 10% of 3000
        result.PromoDiscount.ShouldBe(0m);
        result.Total.ShouldBe(2700m);
    }

    [Fact]
    public void Calculate_StarterSet_AppliesTenPercentBundleDiscount()
    {
        var items = new List<OrderItem>
        {
            new() { MiniatureId = Guid.NewGuid(), MiniatureName = "Starter Set", Faction = "Imperium", UnitPrice = 10000m, Quantity = 1, IsStarterSet = true }
        };

        var result = DiscountCalculator.Calculate(items, null);

        result.Subtotal.ShouldBe(10000m);
        result.BundleDiscount.ShouldBe(1000m);
        result.Total.ShouldBe(9000m);
    }

    [Fact]
    public void Calculate_PromoCodeWarhammer10_AppliesTenPercentOnSubtotalAfterBundle()
    {
        var items = new List<OrderItem>
        {
            new() { MiniatureId = Guid.NewGuid(), MiniatureName = "Captain", Faction = "Imperium", UnitPrice = 5000m, Quantity = 1, IsStarterSet = false }
        };

        var result = DiscountCalculator.Calculate(items, "WARHAMMER10");

        result.Subtotal.ShouldBe(5000m);
        result.BundleDiscount.ShouldBe(0m);
        result.PromoDiscount.ShouldBe(500m); // 10%
        result.Total.ShouldBe(4500m);
    }

    [Fact]
    public void Calculate_Emperor20WithBundleDiscount_StacksDiscountsMultiplicatively()
    {
        var items = new List<OrderItem>
        {
            new() { MiniatureId = Guid.NewGuid(), MiniatureName = "Intercessors", Faction = "Imperium", UnitPrice = 10000m, Quantity = 3, IsStarterSet = false }
        };

        // Subtotal: 30,000
        // Bundle 10%: 3,000 -> Remainder: 27,000
        // Promo 20%: 27,000 * 0.20 = 5,400
        // Total: 27,000 - 5,400 = 21,600
        var result = DiscountCalculator.Calculate(items, "EMPEROR20");

        result.Subtotal.ShouldBe(30000m);
        result.BundleDiscount.ShouldBe(3000m);
        result.PromoDiscount.ShouldBe(5400m);
        result.Total.ShouldBe(21600m);
    }

    [Fact]
    public void Calculate_InvalidPromoCode_IgnoredWithoutDiscount()
    {
        var items = new List<OrderItem>
        {
            new() { MiniatureId = Guid.NewGuid(), MiniatureName = "Necron Warrior", Faction = "Xenos", UnitPrice = 2000m, Quantity = 1 }
        };

        var result = DiscountCalculator.Calculate(items, "UNKNOWN_CODE_999");

        result.PromoDiscount.ShouldBe(0m);
        result.Total.ShouldBe(2000m);
    }
}
