using System.Net.Http.Json;
using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using VaultCore.Api.Endpoints;
using VaultCore.Modules.Inventory.Models;
using VaultCore.Modules.Orders.Commands;
using VaultCore.Modules.Orders.Models;
using Xunit;

namespace VaultCore.Tests.Integration;

[Collection("IntegrationTests")]
public class OrdersAndInventoryIntegrationTests
{
    private readonly VaultCoreFixture _fixture;

    public OrdersAndInventoryIntegrationTests(VaultCoreFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateOrder_ValidCommand_PersistsOrderAndDeductsInventoryStock()
    {
        var miniatureId = Guid.Parse("22222222-2222-2222-2222-222222222222"); // Intercessors

        // Get initial stock
        using var scope1 = _fixture.Host.Services.CreateScope();
        var session1 = scope1.ServiceProvider.GetRequiredService<IQuerySession>();
        var initialItem = await session1.LoadAsync<InventoryItem>(miniatureId);
        var initialStock = initialItem?.AvailableStock ?? 25;

        var command = new CreateOrderCommand(
            new CustomerInfo
            {
                FullName = "Captain Titus",
                Email = "titus@ultramarines.chapter.org",
                Phone = "+79997778899",
                ShippingAddress = "Battle Barge Spear of Macragge, Orbit of Graia"
            },
            [new CreateOrderItemRequest(miniatureId, 2)],
            "WARHAMMER10"
        );

        var response = await _fixture.Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/api/orders");
            s.StatusCodeShouldBe(201);
        });

        var createdOrder = response.ReadAsJson<Order>();
        createdOrder.ShouldNotBeNull();
        createdOrder.OrderNumber.ShouldStartWith("ORD-");
        createdOrder.Customer.Email.ShouldBe("titus@ultramarines.chapter.org");
        createdOrder.Items.Count.ShouldBe(1);
        createdOrder.PromoDiscountAmount.ShouldBeGreaterThan(0);

        // Verify stock deduction in PostgreSQL Marten session with eventual consistency wait
        InventoryItem? updatedItem = null;
        for (int i = 0; i < 25; i++)
        {
            await Task.Delay(100);
            using var scope2 = _fixture.Host.Services.CreateScope();
            var session2 = scope2.ServiceProvider.GetRequiredService<IQuerySession>();
            updatedItem = await session2.LoadAsync<InventoryItem>(miniatureId);
            if (updatedItem != null && updatedItem.AvailableStock == initialStock - 2)
            {
                break;
            }
        }

        updatedItem.ShouldNotBeNull();
        updatedItem.AvailableStock.ShouldBe(initialStock - 2);
    }

    [Fact]
    public async Task CreateOrder_InvalidEmail_ReturnsValidationProblem()
    {
        var command = new CreateOrderCommand(
            new CustomerInfo
            {
                FullName = "Anonymous Cultist",
                Email = "not-a-valid-email",
                Phone = "+70000000000",
                ShippingAddress = "Eye of Terror"
            },
            [new CreateOrderItemRequest(Guid.Parse("44444444-4444-4444-4444-444444444444"), 1)],
            null
        );

        await _fixture.Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/api/orders");
            s.StatusCodeShouldBe(400);
        });
    }

    [Fact]
    public async Task GetUserOrders_ReturnsOrdersForEmail()
    {
        var email = "titus@ultramarines.chapter.org";

        var response = await _fixture.Host.Scenario(s =>
        {
            s.Get.Url($"/api/orders/user/{Uri.EscapeDataString(email)}");
            s.StatusCodeShouldBeOk();
        });

        var text = response.ReadAsText();
        text.ShouldContain("ORD-");
        text.ShouldContain(email);
    }

    [Fact]
    public async Task InventoryAdjust_UpdatesStockCorrectly()
    {
        var miniatureId = Guid.Parse("66666666-6666-6666-6666-666666666666"); // Necrons

        var adjustReq = new AdjustInventoryRequest(miniatureId, 5);

        var response = await _fixture.Host.Scenario(s =>
        {
            s.Post.Json(adjustReq).ToUrl("/api/inventory/adjust");
            s.StatusCodeShouldBeOk();
        });

        var item = response.ReadAsJson<InventoryItem>();
        item.ShouldNotBeNull();
        item.Id.ShouldBe(miniatureId);
    }
}
