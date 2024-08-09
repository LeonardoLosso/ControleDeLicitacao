using ControleDeLicitacao.App.Services.Documentos;
using ControleDeLicitacao.App.Services.Logger;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Documentos;

[Route("[controller]")]
public class UploadController : BaseController
{
    private readonly UploadService _service;

    public UploadController
        (LogInfoService logInfoService, UploadService service):base(logInfoService)
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

        await _service.UploadAta(file);

        return Ok();
    }
}
