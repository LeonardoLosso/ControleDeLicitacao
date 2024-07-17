using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Services.Documentos.Baixa;
using ControleDeLicitacao.App.Services.Logger;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Documentos.Baixa;

[Route("[controller]")]
public class EmpenhoController : BaseController
{
    private readonly EmpenhoService _service;
    public EmpenhoController(LogInfoService logInfoService, EmpenhoService service) : base(logInfoService)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Listar(int id)
    {
        var lista = await _service.Listar(id);
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Novo([FromBody] BaixaDTO dto)
    {
        await base.ValidaRecurso(402);

        var novo = await _service.Adicionar(dto);

        return await RetornaNovo(novo);
    }

    [HttpDelete]
    public async Task<IActionResult> ExcluirReajuste([FromQuery] int id)
    {
        await base.ValidaRecurso(403);

        await _service.Excluir(id);
        return Ok(id);
    }
}
