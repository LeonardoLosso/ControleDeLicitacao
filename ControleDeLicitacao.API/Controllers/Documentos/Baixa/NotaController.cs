using ControleDeLicitacao.App.DTOs.Baixa.NotasEmpenhos;
using ControleDeLicitacao.App.Services.Documentos.Baixa;
using ControleDeLicitacao.App.Services.Logger;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Documentos.Baixa;

[Route("[controller]")]
public class NotaController : BaseController
{
    private readonly NotaService _service;
    public NotaController(LogInfoService logInfoService, NotaService service) : base(logInfoService)
    {
        _service = service;
    }

    [HttpGet("obter/{id}")]
    public async Task<IActionResult> ObterPorID(int id)
    {
        await base.ValidaRecurso(501);

        var dto = await _service.ObterPorID(id);

        if (dto is null) return NotFound();

        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Listar(int id)
    {
        await base.ValidaRecurso(501); // se erro excluir tab notas

        var lista = await _service.Listar(id);
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Novo([FromBody] NotaDTO dto)
    {
        await base.ValidaRecurso(502);

        var novo = await _service.Adicionar(dto);

        return await RetornaNovo(novo);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] JsonPatchDocument<NotaDTO> patchDoc)
    {
        await base.ValidaRecurso(406);

        var dto = await _service.ObterPorID(id);
        if (dto is null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(dto, ModelState);
        }
        catch (JsonPatchException e)
        {
            return BadRequest(e.Message);
        }

        await _service.Editar(dto);

        return await RetornaEdicao(patchDoc);
    }

    [HttpDelete]
    public async Task<IActionResult> Excluir([FromQuery] int id)
    {
        await base.ValidaRecurso(504);

        await _service.Excluir(id);
        return await RetornaDelete(id);
    }
}
