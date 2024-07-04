using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.App.Services.Documentos.Ata;
using ControleDeLicitacao.App.Services.Logger;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Documentos.Ata;

[Route("[controller]")]
public class AtaController : BaseController
{
    private readonly AtaService _service;
    public AtaController(
        LogInfoService logInfoService,
        AtaService service)
         : base(logInfoService)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> NovaEntidade([FromBody] AtaDTO ata)
    {
        await base.ValidaRecurso(302);

        var novo = await _service.Adicionar(ata);

        return await RetornaNovo(novo);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorID(int id)
    {
        var ata = await _service.ObterPorID(id);

        if (ata == null) return NotFound();

        return Ok(ata);
    }

    [HttpGet]
    public async Task<IActionResult> ListarEntidades(
        [FromQuery] int? pagina = null,
        [FromQuery] int? tipo = null,
        [FromQuery] int? status = null,
        [FromQuery] int? unidade = null,
        [FromQuery] DateTime? dataInicial = null,
        [FromQuery] DateTime? dataFinal = null,
        [FromQuery] DateTime? dataAtaInicial = null,
        [FromQuery] DateTime? dataAtaFinal = null,
        [FromQuery] string? search = null
        )
    {
        var lista = await _service.Listar(
            pagina, tipo, status, unidade, dataInicial, 
            dataFinal, dataAtaInicial, dataAtaFinal, search);

        return Ok(lista);
    }
}
