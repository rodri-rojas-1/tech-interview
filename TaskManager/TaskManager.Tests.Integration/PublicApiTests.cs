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
    }
}
