using ControleDeLicitacao.App.DTOs.User;
using ControleDeLicitacao.App.Services.Cadastros.User;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Logger;

namespace ControleDeLicitacao.API.Controllers.Cadastros.Usuarios;

[Route("[controller]")]
public class UsuariosController : BaseController
{
    private UsuarioService _service;

    public UsuariosController(LogInfoService logInfoService, UsuarioService service): base(logInfoService)
    {
        _service = service;
    }
    [HttpPost]
    public async Task<IActionResult> CadastraUsuario
            (UsuarioDTO dto)
    {
        await base.ValidaRecurso(602);

        await _service.Cadastrar(dto);
        return await RetornaNovo(dto);
    }


    [HttpPatch("status/{id}")]
    public async Task<IActionResult> AlteraStatus(int id, JsonPatchDocument<UsuarioDTO> patchDoc)
    {
        await base.ValidaRecurso(604);

        var dto = await _service.ObterPorID(id);
        if (dto is null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(dto, ModelState);
        }
        catch (JsonPatchException ex)
        {
            return BadRequest(ex.Message);
        }

        try
        {
            await _service.EditarAsync(dto);
            return await RetornaEdicao(patchDoc);
        }
        catch (GenericException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditarUsuario(int id, [FromBody] JsonPatchDocument<UsuarioDTO> patchDoc)
    {
        await base.ValidaRecurso(603);

        var dto = await _service.ObterPorID(id);
        if (dto is null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(dto, ModelState);
        }
        catch (JsonPatchException ex)
        {
            return BadRequest(ex.Message);
        }

        try
        {
            await _service.EditarAsync(dto);
            return await RetornaEdicao(patchDoc);
        }
        catch (GenericException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginUsuarioDTO dto)
    {
        var access_token = await _service.Login(dto);
        return Ok(new { access_token });
    }
    [HttpGet]
    public async Task<IActionResult> ListarUsuarios(
        [FromQuery] int? pagina = null,
        [FromQuery] int? status = null,
        [FromQuery] string? search = null
        )
    {
        await base.ValidaRecurso(601);

        var lista = await _service.Listar(pagina, status, search);

        return Ok(lista);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorID(int id)
    {
        await base.ValidaRecurso(601);

        var usuario = await _service.ObterPorID(id);

        if (usuario is null) return NotFound();

        return Ok(usuario);
    }

    [HttpGet("username/{userName}")]
    public async Task<IActionResult> ObterUsuario(string userName)
    {
        await base.ValidaRecurso(601);

        var existe = await _service.ObterUsuarioExistente(userName);

        return Ok(existe);
    }

    [HttpGet("recursos")]
    public async Task<IActionResult> RetornaPermissoes()
    {
        await base.ValidaRecurso(601);

        var permissoes = _service.RetornaPermissoes();
        return Ok(permissoes);
    }
}
