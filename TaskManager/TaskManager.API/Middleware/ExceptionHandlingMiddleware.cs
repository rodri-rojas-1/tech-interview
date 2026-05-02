using System.Net;
using System.Text.Json;
using TaskManager.Application.Exceptions;

namespace TaskManager.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case AppValidationException vx:
                    _logger.LogWarning(vx, "Validation");
                    await WriteProblemAsync(context, HttpStatusCode.BadRequest, vx.Message);
                    return;
                case NotFoundException nx:
                    _logger.LogWarning(nx, "Not found");
                    await WriteProblemAsync(context, HttpStatusCode.NotFound, nx.Message);
                    return;
                case ConflictException cx:
                    _logger.LogWarning(cx, "Conflict");
                    await WriteProblemAsync(context, HttpStatusCode.Conflict, cx.Message);
                    return;
                case UnauthorizedAppException ux:
                    _logger.LogWarning(ux, "Unauthorized (application)");
                    await WriteProblemAsync(context, HttpStatusCode.Unauthorized, ux.Message);
                    return;
            }

            _logger.LogError(ex, "Unhandled error");
            await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode status, string detail)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/problem+json";

        var payload = new
        {
            type = "about:blank",
            title = status.ToString(),
            status = (int)status,
            detail,
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseAppExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
