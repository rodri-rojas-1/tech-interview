using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.API.Middleware;
using TaskManager.Application.Exceptions;

namespace TaskManager.Tests.Unit;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenValidationException_ReturnsBadRequestProblem()
    {
        var sut = CreateMiddleware(_ => throw new AppValidationException("bad input"));
        var ctx = CreateContext();

        await sut.InvokeAsync(ctx);

        Assert.Equal(StatusCodes.Status400BadRequest, ctx.Response.StatusCode);
        var body = await ReadBodyAsync(ctx);
        Assert.Contains("\"detail\":\"bad input\"", body);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundException_ReturnsNotFoundProblem()
    {
        var sut = CreateMiddleware(_ => throw new NotFoundException("missing"));
        var ctx = CreateContext();

        await sut.InvokeAsync(ctx);

        Assert.Equal(StatusCodes.Status404NotFound, ctx.Response.StatusCode);
        var body = await ReadBodyAsync(ctx);
        Assert.Contains("\"detail\":\"missing\"", body);
    }

    [Fact]
    public async Task InvokeAsync_WhenConflictException_ReturnsConflictProblem()
    {
        var sut = CreateMiddleware(_ => throw new ConflictException("exists"));
        var ctx = CreateContext();

        await sut.InvokeAsync(ctx);

        Assert.Equal(StatusCodes.Status409Conflict, ctx.Response.StatusCode);
        var body = await ReadBodyAsync(ctx);
        Assert.Contains("\"detail\":\"exists\"", body);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedAppException_ReturnsUnauthorizedProblem()
    {
        var sut = CreateMiddleware(_ => throw new UnauthorizedAppException("no auth"));
        var ctx = CreateContext();

        await sut.InvokeAsync(ctx);

        Assert.Equal(StatusCodes.Status401Unauthorized, ctx.Response.StatusCode);
        var body = await ReadBodyAsync(ctx);
        Assert.Contains("\"detail\":\"no auth\"", body);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnknownException_ReturnsInternalServerErrorProblem()
    {
        var sut = CreateMiddleware(_ => throw new InvalidOperationException("boom"));
        var ctx = CreateContext();

        await sut.InvokeAsync(ctx);

        Assert.Equal(StatusCodes.Status500InternalServerError, ctx.Response.StatusCode);
        var body = await ReadBodyAsync(ctx);
        Assert.Contains("An unexpected error occurred.", body);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_DelegatesToNext()
    {
        var called = false;
        var sut = CreateMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        });
        var ctx = CreateContext();

        await sut.InvokeAsync(ctx);

        Assert.True(called);
    }

    private static ExceptionHandlingMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new ExceptionHandlingMiddleware(next, Mock.Of<ILogger<ExceptionHandlingMiddleware>>());
    }

    private static DefaultHttpContext CreateContext()
    {
        return new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream(),
            },
        };
    }

    private static async Task<string> ReadBodyAsync(DefaultHttpContext ctx)
    {
        ctx.Response.Body.Position = 0;
        using var reader = new StreamReader(ctx.Response.Body, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}
