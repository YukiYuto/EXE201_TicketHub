using Microsoft.AspNetCore.Mvc;

namespace TicketHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult GetHello()
    {
        return Ok(new { message = "Hello from TestController!" });
    }

    [HttpPost("echo")]
    public IActionResult Echo([FromBody] string input)
    {
        return Ok(new { message = $"You said: {input}" });
    }
}