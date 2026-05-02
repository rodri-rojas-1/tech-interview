using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Application.Contracts;
using TaskManager.Domain;
using TaskManager.Tests.Integration.Fixtures;
using Xunit;

namespace TaskManager.Tests.Integration.Api;

[Collection("postgres")]
public sealed class TasksHttpIntegrationTests
{
    private readonly PostgresIntegrationFixture _fixture;

    public TasksHttpIntegrationTests(PostgresIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    [SkippableFact]
    public async Task Tasks_crud_flow_with_jwt()
    {
        Skip.If(_fixture.Factory is null, _fixture.SkipReason);
        var client = _fixture.Factory!.CreateClient();
        var email = $"http-{Guid.NewGuid():N}@local";
        const string password = "Password12";

        var reg = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, password));
        reg.EnsureSuccessStatusCode();
        var auth = await reg.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(auth);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth!.Token);

        var create = await client.PostAsJsonAsync(
            "/api/tasks",
            new CreateTaskRequest("Http task", null, TaskItemStatus.Todo, null));
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<TaskDtoContract>(JsonOptions);
        Assert.NotNull(created);

        var list = await client.GetFromJsonAsync<List<TaskDtoContract>>(
            "/api/tasks",
            JsonOptions);
        Assert.NotNull(list);
        Assert.Contains(list!, t => t.Id == created!.Id);

        var put = await client.PutAsJsonAsync(
            $"/api/tasks/{created!.Id}",
            new UpdateTaskRequest("Http task updated", "note", TaskItemStatus.Done, null));
        put.EnsureSuccessStatusCode();

        var del = await client.DeleteAsync($"/api/tasks/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
    }

    /// <summary>JSON shape returned by the API (camelCase).</summary>
    private sealed record TaskDtoContract(
        Guid Id,
        string Title,
        string? Description,
        TaskItemStatus Status,
        DateTime? DueDateUtc,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc);
}
