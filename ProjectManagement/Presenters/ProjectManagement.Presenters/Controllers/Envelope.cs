using System.Net;
using ProjectManagement.Domain.Utilities;

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

    public static Envelope FromResult<T, Y>(Result<T, Error> result, Func<T, Y> successMapper)
    {
        if (result.IsSuccess)
            return new Envelope(HttpStatusCode.OK, successMapper(result.OnSuccess));
        HttpStatusCode code = StatusCodeFromResult(result);
        return new Envelope(code, null, result.OnError.Message);
    }

    private static HttpStatusCode StatusCodeFromResult<T>(Result<T, Error> result)
    {
        return result.OnError.Type switch
        {
            ErrorType.NotFound => HttpStatusCode.NotFound,
            ErrorType.Conflict => HttpStatusCode.Conflict,
            ErrorType.InternalError => HttpStatusCode.InternalServerError,
            ErrorType.Validation => HttpStatusCode.BadRequest,
            ErrorType.InvalidFormat => HttpStatusCode.BadRequest,
            ErrorType.None => throw new InvalidOperationException("None error type specified in operation result."),
            _ => HttpStatusCode.InternalServerError
        };
    }
}