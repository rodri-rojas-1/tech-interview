using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Controllers;
using TaskManager.Application.Contracts;
using TaskManager.Application.Services;

namespace TaskManager.Tests.Unit;

public sealed class AuthControllerTests
{
    [Fact]
    public async Task Register_calls_service_and_returns_ok()
    {
        var auth = new Mock<IAuthService>();
        var expected = new AuthResponse("tok", Guid.NewGuid(), "a@b.com");
        auth.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var sut = new AuthController(auth.Object);
        var result = await sut.Register(new RegisterRequest("a@b.com", "password12"), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task Login_calls_service_and_returns_ok()
    {
        var auth = new Mock<IAuthService>();
        var expected = new AuthResponse("tok-login", Guid.NewGuid(), "a@b.com");
        auth.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var sut = new AuthController(auth.Object);
        var result = await sut.Login(new LoginRequest("a@b.com", "password12"), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public void Me_with_valid_claims_returns_current_user()
    {
        var auth = new Mock<IAuthService>();
        var sut = new AuthController(auth.Object);
        var userId = Guid.NewGuid();
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = CreateHttpContext(userId, "x@y.com"),
        };

        var result = sut.Me();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<CurrentUserResponse>(ok.Value);
        Assert.Equal(userId, body.UserId);
        Assert.Equal("x@y.com", body.Email);
    }

    [Fact]
    public void Me_without_email_returns_unauthorized()
    {
        var auth = new Mock<IAuthService>();
        var sut = new AuthController(auth.Object);
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) },
            "Test");
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) },
        };

        var result = sut.Me();

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public void Me_with_invalid_guid_returns_unauthorized()
    {
        var auth = new Mock<IAuthService>();
        var sut = new AuthController(auth.Object);
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "not-a-guid"),
                new Claim(ClaimTypes.Email, "x@y.com"),
            },
            "Test");
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) },
        };

        var result = sut.Me();
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    private static DefaultHttpContext CreateHttpContext(Guid userId, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        return new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
    }
}
