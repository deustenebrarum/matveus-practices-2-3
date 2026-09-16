using VaultCore.Modules.Orders.Models;

namespace VaultCore.Modules.Orders.Services;

public record CalculationResult(
    decimal Subtotal,
    decimal BundleDiscount,
    decimal PromoDiscount,
    decimal Total
);

public static class DiscountCalculator
{
    public const decimal BundleDiscountRate = 0.10m; // 10%

    public static CalculationResult Calculate(IReadOnlyList<OrderItem> items, string? promoCode)
    {
        if (items.Count == 0)
        {
            return new CalculationResult(0, 0, 0, 0);
        }

        var subtotal = items.Sum(x => x.UnitPrice * x.Quantity);

        // Rule 1: Bundle discount (10%) if contains a starter set OR >= 3 miniatures from the same faction
        var hasStarterSet = items.Any(x => x.IsStarterSet);
        var hasSquadOfSameFaction = items
            .GroupBy(x => x.Faction)
            .Any(g => g.Sum(x => x.Quantity) >= 3);

        var bundleDiscount = (hasStarterSet || hasSquadOfSameFaction)
            ? Math.Round(subtotal * BundleDiscountRate, 2, MidpointRounding.AwayFromZero)
            : 0m;

        var amountAfterBundle = subtotal - bundleDiscount;

        // Rule 2: Promo code discount
        var promoDiscount = 0m;
        if (!string.IsNullOrWhiteSpace(promoCode))
        {
            var code = promoCode.Trim().ToUpperInvariant();
            var rate = code switch
            {
                "WARHAMMER10" or "TERRA10" or "WARP-TITHE-10" => 0.10m,
                "EMPEROR20" => 0.20m,
                _ => 0m
            };

            if (rate > 0)
            {
                promoDiscount = Math.Round(amountAfterBundle * rate, 2, MidpointRounding.AwayFromZero);
            }
        }

        var total = Math.Max(0m, amountAfterBundle - promoDiscount);

        return new CalculationResult(subtotal, bundleDiscount, promoDiscount, total);
    }
}
