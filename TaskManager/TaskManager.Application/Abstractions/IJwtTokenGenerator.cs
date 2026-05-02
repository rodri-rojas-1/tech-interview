namespace TaskManager.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string CreateToken(Guid userId, string email);
}
