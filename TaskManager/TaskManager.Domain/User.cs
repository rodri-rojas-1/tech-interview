namespace TaskManager.Domain;

public sealed record User(Guid Id, string Email, string PasswordHash, DateTime CreatedAtUtc);
