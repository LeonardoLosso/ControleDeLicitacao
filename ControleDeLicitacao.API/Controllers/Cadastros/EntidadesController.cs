using Microsoft.AspNetCore.Mvc;
using ControleDeLicitacao.App.DTOs.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.App.Services.Logger;

namespace ControleDeLicitacao.API.Controllers.Cadastros;

[Route("[controller]")]
public class EntidadesController : BaseController
{
    private readonly EntidadeService _service;
    public EntidadesController(LogInfoService logInfoService, EntidadeService service) : base(logInfoService)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> NovaEntidade([FromBody] EntidadeDTO entidade)
    {
        await _service.Adicionar(entidade);

        return await RetornaNovo(
            CreatedAtAction(
                nameof(ObterPorID), 
                new { id = entidade.ID }, entidade));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditarEntidade(int id, [FromBody] JsonPatchDocument<EntidadeDTO> patchDoc)
    {
        var entidadeDTO = await _service.ObterPorIDParaEdicao(id);
        if (entidadeDTO == null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(entidadeDTO, ModelState);
        }
        catch (JsonPatchException ex)
        {
            return BadRequest(ex.Message);
        }

        await _service.Editar(entidadeDTO);

        return await RetornaEdicao(patchDoc);
    }

    [HttpPatch("status/{id}")]
    public async Task<IActionResult> AlteraStatus(int id, JsonPatchDocument<EntidadeDTO> patchDoc)
    {
        var entidadeDTO = await _service.ObterPorID(id);
        if (entidadeDTO == null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(entidadeDTO, ModelState);
        }
        catch (JsonPatchException ex)
        {
            return BadRequest(ex.Message);
        }

        await _service.Editar(entidadeDTO, false);

        return await RetornaEdicao(patchDoc);
    }

    [HttpGet]
    public async Task<IActionResult> ListarEntidades(
        [FromQuery] int? pagina = null,
        [FromQuery] int? tipo = null,
        [FromQuery] int? status = null,
        [FromQuery] string? cidade = null,
        [FromQuery] string? search = null
        )
    {
        var lista = await _service.Listar(pagina, tipo, status, cidade, search);

        return Ok(lista);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorID(int id)
    {
        var entidade = await _service.ObterPorID(id);

        if (entidade == null) return NotFound();

        return Ok(entidade);
    }
}
