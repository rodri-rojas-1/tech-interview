namespace TaskManager.Application.Contracts;

public sealed record RegisterRequest(string Email, string Password);

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponse(string Token, Guid UserId, string Email);

public sealed record CurrentUserResponse(Guid UserId, string Email);
