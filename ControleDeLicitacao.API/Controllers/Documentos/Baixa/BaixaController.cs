using ControleDeLicitacao.App.Services.Documentos.Baixa;
using ControleDeLicitacao.App.Services.Logger;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ControleDeLicitacao.App.DTOs.Baixa;

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

    [HttpPatch("status/{id}")]
    public async Task<IActionResult> AlteraStatus(int id, JsonPatchDocument<BaixaDTO> patchDoc)
    {
        await base.ValidaRecurso(405);
        var dto = await _service.ObterPorID(id);
        if (dto is null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(dto, ModelState);
        }
        catch (JsonPatchException ex)
        {
            return BadRequest(ex.Message);
        }

        await _service.AlterarStatus(dto);

        return await RetornaEdicao(patchDoc);
    }
}
