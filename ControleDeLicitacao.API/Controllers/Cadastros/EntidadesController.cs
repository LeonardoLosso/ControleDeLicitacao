using Microsoft.AspNetCore.Mvc;
using ControleDeLicitacao.App.DTOs.Entidades;
using ControleDeLicitacao.App.Services;
using Microsoft.AspNetCore.JsonPatch;

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

        //método de caminho para obter, id que se deve obter, retorno no payload
        // se pah mudar para OK só pq não precisa ser consumida
        return CreatedAtAction(nameof(ObterPorID), new { id = entidade.ID }, entidade);
    }

    [HttpPut]
    public IActionResult EditarEntidade([FromBody] EntidadeDTO dto)
    {
       
        _service.Editar(dto);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult AlteraStatus(int id, JsonPatchDocument<EntidadeDTO> patch)
    {
        _service.AlterarStatus(id);
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
