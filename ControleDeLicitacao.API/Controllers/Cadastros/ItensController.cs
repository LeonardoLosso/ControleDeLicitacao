using ControleDeLicitacao.App.DTOs.Itens;
using ControleDeLicitacao.App.Services;
using Microsoft.AspNetCore.JsonPatch;
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

    [HttpPut]
    public IActionResult EditarItem([FromBody] ItemDTO dto)
    {
        _service.Editar(dto, true);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult AlteraStatus(int id, JsonPatchDocument<ItemDTO> patch)
    {
        _service.AlterarStatus(id);
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
