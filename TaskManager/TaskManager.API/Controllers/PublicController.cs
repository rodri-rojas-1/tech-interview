using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public sealed class PublicController : ControllerBase
{
    [HttpGet("info")]
    [HttpGet("live")]
    public IActionResult Info()
    {
        return Ok(new
        {
            name = "TaskManager API",
            status = "ok",
        });
    }
}
