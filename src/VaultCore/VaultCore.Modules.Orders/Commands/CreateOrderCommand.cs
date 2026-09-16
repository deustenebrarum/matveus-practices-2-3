using FluentValidation;
using VaultCore.Modules.Orders.Models;

namespace VaultCore.Modules.Orders.Commands;

public record CreateOrderItemRequest(
    Guid MiniatureId,
    int Quantity
);

public record CreateOrderCommand(
    CustomerInfo Customer,
    List<CreateOrderItemRequest> Items,
    string? PromoCode
);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Customer).NotNull().WithMessage("Customer information is required.");

        When(x => x.Customer != null, () =>
        {
            RuleFor(x => x.Customer.FullName)
                .NotEmpty().WithMessage("Customer full name is required.")
                .MinimumLength(2).MaximumLength(100);

            RuleFor(x => x.Customer.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(x => x.Customer.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .MinimumLength(7).MaximumLength(20);

            RuleFor(x => x.Customer.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required.")
                .MinimumLength(5).MaximumLength(250);
        });

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.MiniatureId).NotEmpty().WithMessage("Miniature ID is required.");
            item.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
        });
    }
}
