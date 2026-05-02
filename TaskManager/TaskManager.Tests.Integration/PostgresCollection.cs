using TaskManager.Tests.Integration.Fixtures;

namespace TaskManager.Tests.Integration;

[CollectionDefinition("postgres")]
public sealed class PostgresCollection : ICollectionFixture<PostgresIntegrationFixture>;
