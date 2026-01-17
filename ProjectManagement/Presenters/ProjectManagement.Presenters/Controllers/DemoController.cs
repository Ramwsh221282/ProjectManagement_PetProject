using Microsoft.AspNetCore.Mvc;

namespace ProjectManagement.Presenters.Controllers;

public class DemoController
{
    [HttpPost("exception-suppress")]
    public async Task<Envelope> DoSomething()
    {
        throw new Exception("Test exception");
    }
}