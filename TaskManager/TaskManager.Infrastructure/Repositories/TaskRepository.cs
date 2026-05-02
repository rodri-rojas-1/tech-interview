using Npgsql;
using TaskManager.Application.Abstractions;
using TaskManager.Domain;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly NpgsqlConnectionFactory _connections;

    public TaskRepository(NpgsqlConnectionFactory connections)
    {
        _connections = connections;
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            """
            INSERT INTO tasks (id, user_id, title, description, status, due_date_utc, created_at_utc, updated_at_utc)
            VALUES (@id, @user_id, @title, @description, @status, @due_date_utc, @created_at_utc, @updated_at_utc)
            """,
            connection);

        AddParameters(command, task);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid taskId, Guid userId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            """
            DELETE FROM tasks
            WHERE id = @id AND user_id = @user_id
            """,
            connection);

        command.Parameters.Add(new NpgsqlParameter("id", taskId));
        command.Parameters.Add(new NpgsqlParameter("user_id", userId));

        var affected = await command.ExecuteNonQueryAsync(cancellationToken);
        return affected > 0;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid taskId, Guid userId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            """
            SELECT id, user_id, title, description, status, due_date_utc, created_at_utc, updated_at_utc
            FROM tasks
            WHERE id = @id AND user_id = @user_id
            LIMIT 1
            """,
            connection);

        command.Parameters.Add(new NpgsqlParameter("id", taskId));
        command.Parameters.Add(new NpgsqlParameter("user_id", userId));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return Map(reader);
    }

    public async Task<IReadOnlyList<TaskItem>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            """
            SELECT id, user_id, title, description, status, due_date_utc, created_at_utc, updated_at_utc
            FROM tasks
            WHERE user_id = @user_id
            ORDER BY updated_at_utc DESC
            """,
            connection);

        command.Parameters.Add(new NpgsqlParameter("user_id", userId));

        var list = new List<TaskItem>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            list.Add(Map(reader));

        return list;
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            """
            UPDATE tasks
            SET title = @title,
                description = @description,
                status = @status,
                due_date_utc = @due_date_utc,
                updated_at_utc = @updated_at_utc
            WHERE id = @id AND user_id = @user_id
            """,
            connection);

        AddParameters(command, task);

        var affected = await command.ExecuteNonQueryAsync(cancellationToken);
        if (affected == 0)
            throw new InvalidOperationException("Could not update the task.");
    }

    private static void AddParameters(NpgsqlCommand command, TaskItem task)
    {
        command.Parameters.Add(new NpgsqlParameter("id", task.Id));
        command.Parameters.Add(new NpgsqlParameter("user_id", task.UserId));
        command.Parameters.Add(new NpgsqlParameter("title", task.Title));
        command.Parameters.Add(new NpgsqlParameter("description", (object?)task.Description ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("status", (short)task.Status));
        command.Parameters.Add(new NpgsqlParameter("due_date_utc", (object?)task.DueDateUtc ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("created_at_utc", task.CreatedAtUtc));
        command.Parameters.Add(new NpgsqlParameter("updated_at_utc", task.UpdatedAtUtc));
    }

    private static TaskItem Map(NpgsqlDataReader reader)
    {
        var id = reader.GetGuid(0);
        var userId = reader.GetGuid(1);
        var title = reader.GetString(2);
        var description = reader.IsDBNull(3) ? null : reader.GetString(3);
        var status = (TaskItemStatus)reader.GetInt16(4);
        DateTime? due = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
        var created = reader.GetDateTime(6);
        var updated = reader.GetDateTime(7);

        if (created.Kind == DateTimeKind.Unspecified)
            created = DateTime.SpecifyKind(created, DateTimeKind.Utc);
        if (updated.Kind == DateTimeKind.Unspecified)
            updated = DateTime.SpecifyKind(updated, DateTimeKind.Utc);
        if (due is { Kind: DateTimeKind.Unspecified } d)
            due = DateTime.SpecifyKind(d, DateTimeKind.Utc);

        return new TaskItem(id, userId, title, description, status, due, created, updated);
    }
}
