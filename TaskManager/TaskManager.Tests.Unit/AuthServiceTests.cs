using Moq;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Contracts;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Services;
using TaskManager.Domain;

namespace TaskManager.Tests.Unit;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ThrowsConflictException()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(x => x.GetByEmailAsync("a@b.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User(Guid.NewGuid(), "a@b.com", "hash", DateTime.UtcNow));

        var sut = new AuthService(users.Object, Mock.Of<IPasswordHasher>(), Mock.Of<IJwtTokenGenerator>());

        await Assert.ThrowsAsync<ConflictException>(() =>
            sut.RegisterAsync(new RegisterRequest("A@b.com", "password12"), CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordWrong_ThrowsUnauthorizedAppException()
    {
        var users = new Mock<IUserRepository>();
        var user = new User(Guid.NewGuid(), "a@b.com", "hash", DateTime.UtcNow);
        users.Setup(x => x.GetByEmailAsync("a@b.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(x => x.Verify("wrong", user.PasswordHash)).Returns(false);

        var sut = new AuthService(users.Object, hasher.Object, Mock.Of<IJwtTokenGenerator>());

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            sut.LoginAsync(new LoginRequest("a@b.com", "wrong"), CancellationToken.None));
    }
}
