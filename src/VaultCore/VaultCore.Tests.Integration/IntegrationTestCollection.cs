using Xunit;

namespace VaultCore.Tests.Integration;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<VaultCoreFixture>
{
}
