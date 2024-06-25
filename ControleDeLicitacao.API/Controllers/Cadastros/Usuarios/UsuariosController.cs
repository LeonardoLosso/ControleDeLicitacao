using ControleDeLicitacao.App.DTOs.User;
using ControleDeLicitacao.App.Services.Cadastros.User;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ControleDeLicitacao.App.Error;

namespace ControleDeLicitacao.API.Controllers.Cadastros.Usuarios;

[ApiController]
[Route("[controller]")]
public class UsuariosController : ControllerBase
{
    private UsuarioService _service;

    public UsuariosController(UsuarioService service)
    {
        _service = service;
    }
    [HttpPost]
    public async Task<IActionResult> CadastraUsuario
            (UsuarioDTO dto)
    {
        await _service.Cadastrar(dto);
        return CreatedAtAction(nameof(ObterPorID), new { id = dto.Id }, dto);
    }


    [HttpPatch("status/{id}")]
    public async Task<IActionResult> AlteraStatus(int id, JsonPatchDocument<UsuarioDTO> patchDoc)
    {
        var dto = _service.ObterPorID(id);
        if (dto == null)
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
            return NoContent();
        }
        catch (GenericException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditarEntidade(int id, [FromBody] JsonPatchDocument<UsuarioDTO> patchDoc)
    {
        var dto = _service.ObterPorID(id);
        if (dto == null)
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
            return NoContent();
        }
        catch (GenericException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginUsuarioDTO dto)
    {
        var token = await _service.Login(dto);
        return Ok(token);
    }
    [HttpGet]
    public IActionResult ListarUsuarios(
        [FromQuery] int? pagina = null,
        [FromQuery] int? status = null,
        [FromQuery] string? search = null
        )
    {
        var lista = _service.Listar(pagina, status, search);

        return Ok(lista);
    }

    [HttpGet("{id}")]
    public IActionResult ObterPorID(int id)
    {
        var usuario = _service.ObterPorID(id);

        if (usuario == null) return NotFound();

        return Ok(usuario);
    }

    [HttpGet("username/{userName}")]
    public IActionResult ObterUsuario(string userName)
    {
        var existe= _service.ObterUsuarioExistente(userName);

        return Ok(existe);
    }

    [HttpGet("recursos")]
    public IActionResult RetornaPermissoes()
    {
        var permissoes = _service.RetornaPermissoes();
        return Ok(permissoes);
    }
}
