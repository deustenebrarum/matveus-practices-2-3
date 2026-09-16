using Alba;
using Shouldly;
using Xunit;

namespace VaultCore.Tests.Integration;

[Collection("IntegrationTests")]
public class CatalogApiIntegrationTests
{
    private readonly VaultCoreFixture _fixture;

    public CatalogApiIntegrationTests(VaultCoreFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetHealthz_ReturnsHealthyStatus()
    {
        var response = await _fixture.Host.Scenario(s =>
        {
            s.Get.Url("/healthz");
            s.StatusCodeShouldBeOk();
        });

        var text = response.ReadAsText();
        text.ShouldContain("Healthy");
        text.ShouldContain("VaultCore");
    }

    [Fact]
    public async Task GetCatalog_ReturnsSeededMiniaturesList()
    {
        var response = await _fixture.Host.Scenario(s =>
        {
            s.Get.Url("/api/catalog");
            s.StatusCodeShouldBeOk();
        });

        var text = response.ReadAsText();
        text.ShouldContain("Roboute Guilliman");
        text.ShouldContain("Space Marines");
        text.ShouldContain("Ultimate Starter Set");
    }

    [Fact]
    public async Task GetCatalog_FilterByFactionImperium_ReturnsOnlyImperiumMiniatures()
    {
        var response = await _fixture.Host.Scenario(s =>
        {
            s.Get.Url("/api/catalog?faction=Imperium");
            s.StatusCodeShouldBeOk();
        });

        var text = response.ReadAsText();
        text.ShouldContain("Imperium");
        text.ShouldNotContain("Abaddon the Despoiler"); // Chaos should not be here
    }

    [Fact]
    public async Task GetFactions_ReturnsFactionSummaries()
    {
        var response = await _fixture.Host.Scenario(s =>
        {
            s.Get.Url("/api/catalog/factions");
            s.StatusCodeShouldBeOk();
        });

        var text = response.ReadAsText();
        text.ShouldContain("Imperium");
        text.ShouldContain("Chaos");
    }

    [Fact]
    public async Task GetMiniatureById_ValidId_ReturnsMiniatureDetailsWithWargear()
    {
        var guillimanId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var response = await _fixture.Host.Scenario(s =>
        {
            s.Get.Url($"/api/catalog/{guillimanId}");
            s.StatusCodeShouldBeOk();
        });

        var text = response.ReadAsText();
        text.ShouldContain("Roboute Guilliman");
        text.ShouldContain("The Emperor's Sword");
        text.ShouldContain("Armour of Fate");
    }
}
