using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Services.Documentos.Baixa;
using ControleDeLicitacao.App.Services.Logger;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Documentos.Baixa;

[Route("controller")]
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

    [HttpPatch("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody]JsonPatchDocument<BaixaDTO> patchDoc)
    {
        return Ok(patchDoc);
    }
}
