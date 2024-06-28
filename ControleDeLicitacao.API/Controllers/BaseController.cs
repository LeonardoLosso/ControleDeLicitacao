using ControleDeLicitacao.App.Services.Logger;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ControleDeLicitacao.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly LogInfoService _logInfoService;

    public BaseController(LogInfoService logInfoService)
    {
        _logInfoService = logInfoService;
    }

    protected async Task<IActionResult> RetornaEdicao<T>(JsonPatchDocument<T> patchDoc) where T : class
    {
        var patch = string.Empty;

        foreach(var operacao  in patchDoc.Operations)
        {
            patch += $"{{\nOperação: {operacao.op}, \nDado: {operacao.path}, \nValor: {operacao.value}\n}}";
        }

        _logInfoService.SetOperacao(patch);
        return NoContent();
    }

    protected async Task<IActionResult> RetornaNovo<T>(T entidade) 
    { 
        var objeto = JsonSerializer.Serialize(entidade);
        _logInfoService.SetOperacao(objeto);

        return Ok(entidade);
    }
}
