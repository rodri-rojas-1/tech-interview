using TaskManager.Application.Abstractions;
using TaskManager.Domain;

namespace TaskManager.Infrastructure.Database;

public sealed class DemoDataSeeder
{
    public const string DemoEmail = "demo@local";
    public const string DemoPassword = "Demo123!";

    private readonly IUserRepository _users;
    private readonly ITaskRepository _tasks;
    private readonly IPasswordHasher _passwordHasher;

    public DemoDataSeeder(IUserRepository users, ITaskRepository tasks, IPasswordHasher passwordHasher)
    {
        _users = users;
        _tasks = tasks;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existing = await _users.GetByEmailAsync(DemoEmail, cancellationToken);
        if (existing is not null)
            return;

        var user = new User(
            Id: Guid.NewGuid(),
            Email: DemoEmail,
            PasswordHash: _passwordHasher.Hash(DemoPassword),
            CreatedAtUtc: DateTime.UtcNow);

        await _users.AddAsync(user, cancellationToken);

        var now = DateTime.UtcNow;
        var samples = new[]
        {
            new TaskItem(
                Guid.NewGuid(),
                user.Id,
                "Prepare technical demo",
                "Review architecture and tests.",
                TaskItemStatus.InProgress,
                now.AddDays(2),
                now,
                now),
            new TaskItem(
                Guid.NewGuid(),
                user.Id,
                "Buy coffee",
                null,
                TaskItemStatus.Todo,
                null,
                now,
                now),
        };

        foreach (var task in samples)
            await _tasks.AddAsync(task, cancellationToken);
    }
}
