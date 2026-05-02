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
    public async Task RegisterAsync_WhenValid_CreatesUser_AndReturnsToken()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(x => x.GetByEmailAsync("new@local.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(x => x.Hash("password12")).Returns("hashed");

        var jwt = new Mock<IJwtTokenGenerator>();
        jwt.Setup(x => x.CreateToken(It.IsAny<Guid>(), "new@local.com"))
            .Returns("jwt-token");

        var sut = new AuthService(users.Object, hasher.Object, jwt.Object);

        var result = await sut.RegisterAsync(
            new RegisterRequest("  New@Local.com  ", "password12"),
            CancellationToken.None);

        Assert.Equal("jwt-token", result.Token);
        Assert.Equal("new@local.com", result.Email);
        users.Verify(
            x => x.AddAsync(
                It.Is<User>(u =>
                    u.Email == "new@local.com" &&
                    u.PasswordHash == "hashed"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

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

    [Fact]
    public async Task LoginAsync_WhenUserMissing_ThrowsUnauthorizedAppException()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(x => x.GetByEmailAsync("missing@local", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = new AuthService(users.Object, Mock.Of<IPasswordHasher>(), Mock.Of<IJwtTokenGenerator>());

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            sut.LoginAsync(new LoginRequest("missing@local", "password12"), CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordEmpty_ThrowsUnauthorizedAppException()
    {
        var sut = new AuthService(Mock.Of<IUserRepository>(), Mock.Of<IPasswordHasher>(), Mock.Of<IJwtTokenGenerator>());

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            sut.LoginAsync(new LoginRequest("a@b.com", ""), CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_WhenValid_ReturnsAuthResponse()
    {
        var user = new User(Guid.NewGuid(), "ok@local", "hash", DateTime.UtcNow);
        var users = new Mock<IUserRepository>();
        users.Setup(x => x.GetByEmailAsync("ok@local", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(x => x.Verify("password12", "hash")).Returns(true);

        var jwt = new Mock<IJwtTokenGenerator>();
        jwt.Setup(x => x.CreateToken(user.Id, user.Email)).Returns("jwt-ok");

        var sut = new AuthService(users.Object, hasher.Object, jwt.Object);
        var result = await sut.LoginAsync(new LoginRequest(" OK@LOCAL ", "password12"), CancellationToken.None);

        Assert.Equal("jwt-ok", result.Token);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Email, result.Email);
    }
}
