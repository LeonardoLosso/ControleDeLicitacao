using Microsoft.AspNetCore.Mvc;
using ControleDeLicitacao.App.DTOs.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using ControleDeLicitacao.App.Services.Cadastros;

namespace ControleDeLicitacao.API.Controllers.Cadastros;

[ApiController]
[Route("[controller]")]
public class EntidadesController : ControllerBase
{
    private EntidadeService _service;
    public EntidadesController(EntidadeService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult NovaEntidade([FromBody] EntidadeDTO entidade)
    {
        _service.Adicionar(entidade);

        return CreatedAtAction(nameof(ObterPorID), new { id = entidade.ID }, entidade);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditarEntidade(int id, [FromBody] JsonPatchDocument<EntidadeDTO> patchDoc)
    {
        var entidadeDTO = _service.ObterPorIDParaEdicao(id);
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

        _service.Editar(entidadeDTO);

        return NoContent();
    }

    [HttpPatch("status/{id}")]
    public IActionResult AlteraStatus(int id, JsonPatchDocument<EntidadeDTO> patchDoc)
    {
        var entidadeDTO = _service.ObterPorID(id);
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

        _service.Editar(entidadeDTO, false);

        return NoContent();
    }

    [HttpGet]
    public IActionResult ListarEntidades(
        [FromQuery] int? pagina = null,
        [FromQuery] int? tipo = null,
        [FromQuery] int? status = null,
        [FromQuery] string? cidade = null,
        [FromQuery] string? search = null
        )
    {
        var lista = _service.Listar(pagina, tipo, status, cidade, search);

        return Ok(lista);
    }
    [HttpGet("{id}")]
    public IActionResult ObterPorID(int id)
    {
        var entidade = _service.ObterPorID(id);

        if (entidade == null) return NotFound();

        return Ok(entidade);
    }
}
