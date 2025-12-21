using System.Net;

namespace ProjectManagement.Presenters.Controllers;

public sealed class Envelope : IResult
{
    public int Status { get; }
    public object? Result { get; }
    public string? Error { get; }

    public Envelope(HttpStatusCode statusCode, object? result = null, string? error = null)
    {
        Status = (int)statusCode;
        Result = result;
        Error = error;
    }

    public Envelope(object result)
    {
        Status = 200;
        Error = null;
        Result = result;
    }

    public Envelope(HttpStatusCode statusCode, string error)
    {
        Status = (int)statusCode;
        Error = error;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = Status;
        httpContext.Response.ContentType = "application/json";
        return httpContext.Response.WriteAsJsonAsync(this);
    }
}