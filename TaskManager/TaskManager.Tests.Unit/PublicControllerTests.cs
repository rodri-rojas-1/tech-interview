using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Controllers;

namespace TaskManager.Tests.Unit;

public sealed class PublicControllerTests
{
    [Fact]
    public void Info_returns_ok_payload()
    {
        var sut = new PublicController();
        var result = sut.Info();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }
}
