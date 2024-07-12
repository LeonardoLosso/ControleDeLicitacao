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

    [HttpPost]
    public async Task<IActionResult> Novo([FromBody] int id)
    {
        await base.ValidaRecurso(302);

        var novo = await _service.Adicionar(id);

        return await RetornaNovo(novo);
    }

    [HttpPut("atualizar")]
    public async Task<IActionResult> Editar([FromBody] int id)
    {
        await base.ValidaRecurso(304);
        await _service.Editar(id);
        _logInfoService.SetOperacao($"baixa editada{id}");
        return Ok(true);
    }
}
