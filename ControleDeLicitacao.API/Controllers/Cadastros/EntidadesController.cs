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
        await base.ValidaRecurso(102);

        var novo = await _service.Adicionar(entidade);

        return await RetornaNovo(novo);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditarEntidade(int id, [FromBody] JsonPatchDocument<EntidadeDTO> patchDoc)
    {
        await base.ValidaRecurso(103);

        var entidadeDTO = await _service.ObterPorIDParaEdicao(id);
        if (entidadeDTO is null)
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
        await base.ValidaRecurso(104);
        var entidadeDTO = await _service.ObterPorID(id);
        if (entidadeDTO is null)
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
        await base.ValidaRecurso(101);

        var lista = await _service.Listar(pagina, tipo, status, cidade, search);

        return Ok(lista);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorID(int id)
    {
        await base.ValidaRecurso(101);

        var entidade = await _service.ObterPorID(id);

        if (entidade is null) return NotFound();

        return Ok(entidade);
    }
}
