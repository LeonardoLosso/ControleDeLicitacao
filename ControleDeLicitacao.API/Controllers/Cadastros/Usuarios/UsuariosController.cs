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
    //refazer saporra
    [HttpPost]
    public async Task<IActionResult> CadastraUsuario
            (UsuarioDTO dto)
    {
        await _service.Cadastrar(dto);
        return Ok("Usuário cadastrado!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginUsuarioDTO dto)
    {
        var token = await _service.Login(dto);
        return Ok(token);
    }
}
