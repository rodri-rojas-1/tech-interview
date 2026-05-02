using System.Net;
using System.Text.Json;

namespace TaskManager.Tests.Integration;

public sealed class PublicApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public PublicApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Public_Info_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/public/info");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal("TaskManager API", doc.RootElement.GetProperty("name").GetString());
        Assert.Equal("ok", doc.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Public_Live_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/public/live");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Auth_Me_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Tasks_List_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/tasks");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
