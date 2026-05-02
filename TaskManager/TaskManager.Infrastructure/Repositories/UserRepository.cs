using Npgsql;
using TaskManager.Application.Abstractions;
using TaskManager.Domain;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly NpgsqlConnectionFactory _connections;

    public UserRepository(NpgsqlConnectionFactory connections)
    {
        _connections = connections;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            """
            INSERT INTO users (id, email, password_hash, created_at_utc)
            VALUES (@id, @email, @password_hash, @created_at_utc)
            """,
            connection);

        command.Parameters.Add(new NpgsqlParameter("id", user.Id));
        command.Parameters.Add(new NpgsqlParameter("email", user.Email));
        command.Parameters.Add(new NpgsqlParameter("password_hash", user.PasswordHash));
        command.Parameters.Add(new NpgsqlParameter("created_at_utc", user.CreatedAtUtc));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            """
            SELECT id, email, password_hash, created_at_utc
            FROM users
            WHERE email = @email
            LIMIT 1
            """,
            connection);

        command.Parameters.Add(new NpgsqlParameter("email", email));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return Map(reader);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            """
            SELECT id, email, password_hash, created_at_utc
            FROM users
            WHERE id = @id
            LIMIT 1
            """,
            connection);

        command.Parameters.Add(new NpgsqlParameter("id", id));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return Map(reader);
    }

    private static User Map(NpgsqlDataReader reader)
    {
        var id = reader.GetGuid(0);
        var email = reader.GetString(1);
        var hash = reader.GetString(2);
        var created = reader.GetDateTime(3);
        if (created.Kind == DateTimeKind.Unspecified)
            created = DateTime.SpecifyKind(created, DateTimeKind.Utc);

        return new User(id, email, hash, created);
    }
}
