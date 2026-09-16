using Shouldly;
using VaultCore.Modules.Orders.Commands;
using VaultCore.Modules.Orders.Models;
using Xunit;

namespace VaultCore.Tests.Unit;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CreateOrderCommand(
            new CustomerInfo
            {
                FullName = "Chapter Master Dante",
                Email = "dante@ba.imperium.org",
                Phone = "+79991234567",
                ShippingAddress = "Fortress-Monastery, Baal Prime, Sector Ultima"
            },
            [new CreateOrderItemRequest(Guid.NewGuid(), 2)],
            "WARHAMMER10"
        );

        var result = _validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("invalid@")]
    [InlineData("@domain.com")]
    public void Validate_InvalidEmail_FailsValidation(string invalidEmail)
    {
        var command = new CreateOrderCommand(
            new CustomerInfo
            {
                FullName = "Test Commander",
                Email = invalidEmail,
                Phone = "+79991234567",
                ShippingAddress = "Sector Gothic 42"
            },
            [new CreateOrderItemRequest(Guid.NewGuid(), 1)],
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName.Contains("Email"));
    }

    [Fact]
    public void Validate_EmptyItemsList_FailsValidation()
    {
        var command = new CreateOrderCommand(
            new CustomerInfo
            {
                FullName = "Commissar Yarrick",
                Email = "yarrick@armageddon.org",
                Phone = "+79998765432",
                ShippingAddress = "Hive Hades, Armageddon"
            },
            [],
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == "Items");
    }

    [Fact]
    public void Validate_ItemQuantityZeroOrNegative_FailsValidation()
    {
        var command = new CreateOrderCommand(
            new CustomerInfo
            {
                FullName = "Inquisitor Eisenhorn",
                Email = "eisenhorn@ordo.xenos.org",
                Phone = "+79998887766",
                ShippingAddress = "Scintilla Inquisitorial Compound"
            },
            [new CreateOrderItemRequest(Guid.NewGuid(), 0)],
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.ErrorMessage.Contains("Quantity must be at least 1"));
    }
}
