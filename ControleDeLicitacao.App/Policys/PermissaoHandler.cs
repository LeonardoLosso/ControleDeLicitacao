using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Cadastros.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ControleDeLicitacao.App.Policys;

public class PermissaoHandler : AuthorizationHandler<PermissaoRequirement>
{
    private readonly TokenService _tokenService;
    public PermissaoHandler(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissaoRequirement requirement)
    {
        var resource = context.Resource;
        var _context = resource as HttpContext;
        var reques = _context?.Request;
        var token = "";
        if (reques != null)
        {
            token = reques.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            var decoded = _tokenService.DecodeToken(token);
            var precisa = requirement.Permissao;

            if (decoded != null)
            {
                var permitido = decoded.Claims.Where(c => c.Type.Equals("recurso")).Any(c => c.Value == precisa.ToString());


                if (permitido)
                {
                    context.Succeed(requirement);

                }
            }
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
