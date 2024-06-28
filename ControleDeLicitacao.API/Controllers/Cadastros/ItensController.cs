using ControleDeLicitacao.App.DTOs.Itens;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.App.Services.Logger;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Cadastros;

[Route("[controller]")]
public class ItensController : BaseController
{
    private ItemService _service;

    public ItensController(LogInfoService logInfoService, ItemService service) : base(logInfoService)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> NovoItem([FromBody] ItemDTO item)
    {
        await _service.Adicionar(item);

        return await RetornaNovo(
            CreatedAtAction(
                nameof(ObterPorID),
                new { id = item.Id }, item));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditarItem(int id, [FromBody] JsonPatchDocument<ItemDTO> patchDoc)
    {
        var itemDTO = await _service.ObterPorIDParaEdicao(id);
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

        await _service.Editar(itemDTO);

        return await RetornaEdicao(patchDoc);
    }

    [HttpPatch("status/{id}")]
    public async Task<IActionResult> AlteraStatus(int id, JsonPatchDocument<ItemDTO> patchDoc)
    {
        var itemDTO = await _service.ObterPorID(id);
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

        await _service.Editar(itemDTO, false);

        return await RetornaEdicao(patchDoc);
    }

    [HttpGet]
    public async Task<IActionResult> ListarItens(
        [FromQuery] int? pagina = null,
        [FromQuery] string? tipo = null,
        [FromQuery] int? status = null,
        [FromQuery] string? unidadePrimaria = null,
        [FromQuery] string? unidadeSecundaria = null,
        [FromQuery] string? search = null
        )
    {
        var lista = await _service.Listar(pagina, tipo, status, unidadePrimaria, unidadeSecundaria, search);

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
