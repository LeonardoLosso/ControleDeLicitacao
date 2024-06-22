using AutoMapper;
using ControleDeLicitacao.App.DTOs.User;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using Microsoft.AspNetCore.Identity;

namespace ControleDeLicitacao.App.Services.Cadastros.User;

public class UsuarioService
{
    private IMapper _mapper;
    private UserManager<Usuario> _userManager;
    private SignInManager<Usuario> _signInManager;
    private TokenService _tokenService;

    public UsuarioService
        (IMapper mapper,
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        TokenService tokenService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task Cadastrar(UsuarioDTO dto)
    {
        Usuario usuario = _mapper.Map<Usuario>(dto);

        IdentityResult resultado = await _userManager.CreateAsync(usuario, dto.Password);


        if (!resultado.Succeeded)
            throw new GenericException("Falha ao cadastrar usuário!", 501);

    }

    public async Task<string> Login(LoginUsuarioDTO dto)
    {
        var resultado = await _signInManager.PasswordSignInAsync
            (dto.Username, dto.Password, false, false);

        if (!resultado.Succeeded)
            throw new GenericException("Usuário não autenticado!", 501);

        var usuario = _signInManager
            .UserManager
            .Users
            .FirstOrDefault(user => user.NormalizedUserName == dto.Username.ToUpper());

        var token = _tokenService.GenerateToken(usuario);

        return token;
    }
}
