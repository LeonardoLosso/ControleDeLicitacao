﻿using ControleDeLicitacao.App.Services.Documentos;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Documentos;

[Route("[controller]")]
public class UploadController : BaseController
{
    private readonly UploadService _service;

    public UploadController
        (UploadService service) : base()
    {
        _service = service;
    }

    [HttpPost("ata")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Nenhum arquivo enviado.");
        }

        var result = await _service.UploadAta(file);

        return await RetornaNovo(result);
    }

    [HttpPost("empenho/{idBaixa}")]
    public async Task<IActionResult> UploadEmpenho(int idBaixa, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Nenhum arquivo enviado.");
        }

        var result = await _service.UploadEmpenho(file, idBaixa);

        return await RetornaNovo(result);
    }
}
