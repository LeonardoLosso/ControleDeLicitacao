using ControleDeLicitacao.App.Services.Documentos.Baixa;
using ControleDeLicitacao.App.Services.Logger;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Documentos.Baixa;

[Route("[controller]")]
public class BaixaController : BaseController
{
    private readonly BaixaService _service;

    public BaixaController(
        LogInfoService logInfoService,
        BaixaService baixaService) : base(logInfoService)
    {
        _service = baixaService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorID(int id)
    {
        await base.ValidaRecurso(401);

        var dto = await _service.ObterPorID(id);

        if (dto is null) return NotFound();

        return Ok(dto);
    }
    [HttpGet("itens/{id}")]
    public async Task<IActionResult> ObterItensPorID(int id, [FromQuery] string? search = null)
    {
        var dto = await _service.ObterItensPorID(id, search);

        if (dto is null) return NotFound();

        return Ok(dto);
    }
}
