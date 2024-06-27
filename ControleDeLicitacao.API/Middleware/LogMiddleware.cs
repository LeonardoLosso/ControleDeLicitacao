using ControleDeLicitacao.App.Services.Cadastros.User;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

namespace ControleDeLicitacao.API.Middleware;
public class LogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LogMiddleware> _logger;
    private readonly TokenService _tokenService;

    public LogMiddleware(
        RequestDelegate next,
        ILogger<LogMiddleware> logger,
        TokenService tokenService)
    {
        _next = next;
        _logger = logger;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ValidarRequest(context))
        {
            

            var timestamp = new DateTime();

            var path = context.Request.Path;

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var user = "";

            if (token != null)
            {
                var decodedToken = _tokenService.DecodeToken(token);
                user = decodedToken.Claims.First().Value;
            }

            var operacao = "";

            if (context.Request.Method.Equals(HttpMethods.Patch))
            {
                operacao = "Edição";
                var patchDoc = await context.Request.ReadFromJsonAsync<JsonPatchDocument>();
            }
            else
            {
                operacao = "Adição";

            }
            //using var reader = new StreamReader(context.Request.Body);
            //var requestBody = await reader.ReadToEndAsync();
            //var alteracoes = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Chamar o próximo middleware
            await _next(context);

            // Registrar o log da resposta
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);

            _logger.LogInformation($"Resposta - Método: {context.Request.Method}, " +
                                  $"URL: {context.Request.Path}, " +
                                  $"Status: {context.Response.StatusCode}, " +
                                  $"Corpo da Resposta: {responseBodyText}");
        }
        else
        {
            // Chamar o próximo middleware se não for um método que altera dados
            await _next(context);
        }
    }

    private bool ValidarRequest(HttpContext context)
    {
        if (ValidarMetodo(context.Request.Method)) return false;

        if (context.Request.Path == "/usuarios/login") return false;

        if (context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() == null) return false;


        return true;
    }
    public bool ValidarMetodo(string method)
    {
        return !(method.Equals(HttpMethods.Put)
            || method.Equals(HttpMethods.Post)
            || method.Equals(HttpMethods.Patch));
    }
}
