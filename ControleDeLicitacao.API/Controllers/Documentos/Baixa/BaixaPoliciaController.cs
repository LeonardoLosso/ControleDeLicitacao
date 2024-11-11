using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Services.Documentos.Baixa;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Documentos.Baixa;

[Route("[controller]")]
public class BaixaPoliciaController : BaseController
{
    private readonly BaixaPoliciaService _service;
    public BaixaPoliciaController(
        BaixaPoliciaService service) : base ()
    {
        _service = service;
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
    public async Task<IActionResult> Salvar(int id, [FromBody] JsonPatchDocument<List<EmpenhoPoliciaDTO>> patchDoc)
    {
        await base.ValidaRecurso(304);

        var dto = await _service.ObterEmpenhos(id);
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

        await _service.Salvar(dto, id);

        return await RetornaEdicao(patchDoc);
    }
}
