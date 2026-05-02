using TaskManager.Application.Abstractions;
using TaskManager.Application.Contracts;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Validation;
using TaskManager.Domain;

namespace TaskManager.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwt)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = InputValidators.NormalizeEmail(request.Email);
        InputValidators.EnsureValidEmail(email);
        InputValidators.EnsurePasswordStrength(request.Password);

        var existing = await _users.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
            throw new ConflictException("A user with this email already exists.");

        var user = new User(
            Id: Guid.NewGuid(),
            Email: email,
            PasswordHash: _passwordHasher.Hash(request.Password),
            CreatedAtUtc: DateTime.UtcNow);

        await _users.AddAsync(user, cancellationToken);

        var token = _jwt.CreateToken(user.Id, user.Email);
        return new AuthResponse(token, user.Id, user.Email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = InputValidators.NormalizeEmail(request.Email);
        InputValidators.EnsureValidEmail(email);

        if (string.IsNullOrEmpty(request.Password))
            throw new UnauthorizedAppException("Invalid credentials.");

        var user = await _users.GetByEmailAsync(email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAppException("Invalid credentials.");

        var token = _jwt.CreateToken(user.Id, user.Email);
        return new AuthResponse(token, user.Id, user.Email);
    }
}
