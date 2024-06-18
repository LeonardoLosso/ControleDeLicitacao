using ControleDeLicitacao.App.Error;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace ControleDeLicitacao.API.Middleware;

public class ErrorHandlingMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int code;
        string result;

        if (exception is GenericException customException)
        {
            code = customException.StatusCode;
            result = JsonConvert.SerializeObject(new
            {
                StatusCode = customException.StatusCode,
                Message = customException.Message
            });
        }
        else
        {
            code = (int)HttpStatusCode.InternalServerError;
            result = JsonConvert.SerializeObject(new
            {
                StatusCode = code,
                Message = "Internal Server Error from the custom middleware.",
                Detailed = exception.Message
            });
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;
        return context.Response.WriteAsync(result);
    }
}
