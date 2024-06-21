using ControleDeLicitacao.App.DTOs.Itens;
using ControleDeLicitacao.App.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Cadastros;

[ApiController]
[Route("[controller]")]
public class ItensController : ControllerBase
{
    private ItemService _service;

    public ItensController(ItemService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult NovoItem([FromBody] ItemDTO item)
    {
        _service.Adicionar(item);

        return CreatedAtAction(nameof(ObterPorID), new { id = item.Id }, item);
    }

    [HttpPatch("{id}")]
    public IActionResult EditarItem(int id, [FromBody] JsonPatchDocument<ItemDTO> patchDoc)
    {
        var itemDTO = _service.ObterPorIDParaEdicao(id);
        if (itemDTO == null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(itemDTO, ModelState);
        }
        catch (JsonPatchException ex)
        {
            return BadRequest(ex.Message);
        }

        _service.Editar(itemDTO);

        return NoContent();
    }

    [HttpPatch("status/{id}")]
    public IActionResult AlteraStatus(int id, JsonPatchDocument<ItemDTO> patchDoc)
    {
        var itemDTO = _service.ObterPorID(id);
        if (itemDTO == null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(itemDTO, ModelState);
        }
        catch (JsonPatchException ex)
        {
            return BadRequest(ex.Message);
        }

        _service.Editar(itemDTO, false);

        return NoContent();
    }

    [HttpGet]
    public IActionResult ListarItens(
        [FromQuery] int? pagina = null,
        [FromQuery] string? tipo = null,
        [FromQuery] int? status = null,
        [FromQuery] string? unidadePrimaria = null,
        [FromQuery] string? unidadeSecundaria = null,
        [FromQuery] string? search = null
        )
    {
        var lista = _service.Listar(pagina, tipo, status, unidadePrimaria, unidadeSecundaria, search);

        return Ok(lista);
    }

    [HttpGet("{id}")]
    public IActionResult ObterPorID(int id)
    {
        var item = _service.ObterPorID(id);

        if (item == null) return NotFound();

        return Ok(item);
    }
}
