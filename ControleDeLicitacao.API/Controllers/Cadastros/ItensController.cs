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
        await base.ValidaRecurso(202);

        var novo = await _service.Adicionar(item);

        return await RetornaNovo(novo);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditarItem(int id, [FromBody] JsonPatchDocument<ItemDTO> patchDoc)
    {
        await base.ValidaRecurso(203);

        var itemDTO = await _service.ObterPorIDParaEdicao(id);
        if (itemDTO is null)
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
        await base.ValidaRecurso(204);

        var itemDTO = await _service.ObterPorID(id);
        if (itemDTO is null)
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
        await base.ValidaRecurso(201);

        var lista = await _service.Listar(pagina, tipo, status, unidadePrimaria, unidadeSecundaria, search);

        return Ok(lista);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorID(int id)
    {
        await base.ValidaRecurso(201);

        var item = await _service.ObterPorID(id);

        if (item is null) return NotFound();

        return Ok(item);
    }
}
