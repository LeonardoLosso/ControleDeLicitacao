using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.Domain.Ressources;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace ControleDeLicitacao.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{

    public BaseController()
    {
    }

    protected async Task<IActionResult> RetornaEdicao<T>(JsonPatchDocument<T> patchDoc) where T : class
    {
        string patch = ".";

        foreach (var operacao in patchDoc.Operations)
        {
            patch += $"{{\nOperação: {operacao.op}, \nDado: {operacao.path}, \nValor: {operacao.value}\n}}";
        }

        return Ok(true);
    }

    protected async Task<IActionResult> RetornaNovo<T>(T entidade)
    {
        var objeto = JsonSerializer.Serialize(entidade);

        return Ok(entidade);
    }
    protected async Task<IActionResult> RetornaDelete<T>(T id)
    {

        return Ok(id);
    }

    protected async Task<bool> ValidaRecurso(int recurso)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (!string.IsNullOrEmpty(token))
        {
            var decoded = DecodeToken(token);
            
            if (decoded != null)
            {
                var validade = decoded.ValidTo;

                if(validade < DateTime.Now)
                {
                    throw new GenericException("Token expirado", 512);
                }

                var permitido = decoded.Claims.Where(c => c.Value == recurso.ToString()).Any();
                if (permitido)
                {
                    return true;

                }
            }
        }
        throw new GenericException("Não autorizado", 401);
    }

    //protected async Task<string?> GetUserAsync()
    //{
    //    var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    //    if (!string.IsNullOrEmpty(token))
    //    {
    //        var decoded = DecodeToken(token);

    //        if (decoded != null)
    //        {
    //            return decoded.Claims.Where(c => c.Properties == );
    //        }
    //    }
    //    return null;
    //}
    private JwtSecurityToken DecodeToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken;
    }
}
