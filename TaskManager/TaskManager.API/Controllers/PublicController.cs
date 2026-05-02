using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public sealed class PublicController : ControllerBase
{
    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new
        {
            name = "TaskManager API",
            status = "ok",
        });
    }
}
