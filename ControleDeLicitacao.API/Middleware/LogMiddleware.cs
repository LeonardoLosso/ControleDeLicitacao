using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Cadastros.User;
using ControleDeLicitacao.App.Services.Logger;
using System.Text;

namespace ControleDeLicitacao.API.Middleware;
public class LogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TokenService _tokenService;

    public LogMiddleware(
        RequestDelegate next,
        TokenService tokenService)
    {
        _next = next;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        if (ValidarRequest(context))
        {
            try
            {
                var _logService = serviceProvider.GetRequiredService<LogInfoService>();

                var path = context.Request.Path;

                var method = context.Request.Method;

                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                var userId = "";

                if (token != null)
                {
                    var decodedToken = _tokenService.DecodeToken(token);
                    userId = decodedToken.Claims.First().Value;
                }

                var requestBodyContent = await ReadRequestBody(context.Request);
                var requestTime = DateTime.UtcNow;

                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    var responseBodyContent = await ReadResponseBody(responseBody);
                    var responseTime = DateTime.UtcNow;

                    var status = context.Response.StatusCode;
                    // Log the operation
                    if (status >= 200 && status <= 299)
                    {
                        _logService.SetRequestInfo(userId, method, path, requestTime);
                        //await _logService.SalvarLogAsync();
                    }

                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                throw new GenericException(ex.Message, 501);
            }
        }
        else
        {
            await _next(context);
        }
    }

    private bool ValidarRequest(HttpContext context)
    {
        if (ValidarMetodo(context.Request.Method)) return false;

        if (context.Request.Path == "/usuarios/login") return false;

        if (context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() is null) return false;


        return true;
    }
    private bool ValidarMetodo(string method)
    {
        return !(method.Equals(HttpMethods.Put)
            || method.Equals(HttpMethods.Post)
            || method.Equals(HttpMethods.Patch)
            || method.Equals(HttpMethods.Delete));
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        using (var streamReader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            var body = await streamReader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
    }

    private async Task<string> ReadResponseBody(MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        return body;
    }
}
