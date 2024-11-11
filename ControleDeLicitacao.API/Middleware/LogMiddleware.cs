using ControleDeLicitacao.App.Services.Cadastros.User;
using ControleDeLicitacao.Infrastructure;
using System.Text;

namespace ControleDeLicitacao.API.Middleware;
public class LogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TokenService _tokenService;
    private readonly UserKeeper _userKeeper;

    public LogMiddleware(
        RequestDelegate next,
        TokenService tokenService,
        UserKeeper userKeeper)
    {
        _next = next;
        _tokenService = tokenService;
        _userKeeper = userKeeper;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        if (ValidarRequest(context))
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var userId = "";

            if (token != null)
            {
                var decodedToken = _tokenService.DecodeToken(token);
                _userKeeper.CurrentUser = decodedToken.Claims.First().Value;
            }

            await _next(context);

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
