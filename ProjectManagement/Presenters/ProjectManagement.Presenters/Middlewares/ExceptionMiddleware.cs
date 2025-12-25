using System.Net;
using ProjectManagement.Presenters.Controllers;

namespace ProjectManagement.Presenters.Middlewares;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError;
            Envelope envelope = new Envelope(code, null, "Ошибка на стороне сервера.");
            await httpContext.Response.WriteAsJsonAsync(envelope);
        }
    }
}