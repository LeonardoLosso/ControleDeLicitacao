using ControleDeLicitacao.App.DTOs.User;
using ControleDeLicitacao.App.Services.Cadastros.User;
using Microsoft.AspNetCore.Mvc;

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
        var entidade = _service.ObterPorID(id);

        if (entidade == null) return NotFound();

        return Ok(entidade);
    }

    [HttpGet("recursos")]
    public IActionResult RetornaPermissoes()
    {
        var permissoes = _service.RetornaPermissoes();
        return Ok(permissoes);
    }
}
