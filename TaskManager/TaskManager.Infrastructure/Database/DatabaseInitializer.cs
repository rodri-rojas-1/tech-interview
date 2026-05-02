using Npgsql;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Database;

public sealed class DatabaseInitializer
{
    private readonly NpgsqlConnectionFactory _connections;

    public DatabaseInitializer(NpgsqlConnectionFactory connections)
    {
        _connections = connections;
    }

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);

        const string ddl =
            """
            CREATE TABLE IF NOT EXISTS users (
              id UUID PRIMARY KEY,
              email VARCHAR(320) NOT NULL UNIQUE,
              password_hash TEXT NOT NULL,
              created_at_utc TIMESTAMPTZ NOT NULL
            );

            CREATE TABLE IF NOT EXISTS tasks (
              id UUID PRIMARY KEY,
              user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
              title VARCHAR(500) NOT NULL,
              description TEXT,
              status SMALLINT NOT NULL,
              due_date_utc TIMESTAMPTZ,
              created_at_utc TIMESTAMPTZ NOT NULL,
              updated_at_utc TIMESTAMPTZ NOT NULL
            );

            CREATE INDEX IF NOT EXISTS ix_tasks_user_id ON tasks(user_id);
            """;

        await using var command = new NpgsqlCommand(ddl, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
