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
        //var ata = await _service.ObterPorID(id);

        //if (ata == null) return NotFound();

        //return Ok(ata);
        return Ok();
    }
}
