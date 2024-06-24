using AutoMapper;
using ControleDeLicitacao.App.DTOs;
using ControleDeLicitacao.App.DTOs.User;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using ControleDeLicitacao.Domain.Ressources;
using Microsoft.AspNetCore.Identity;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using ControleDeLicitacao.Common;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Cadastros.User;

public class UsuarioService
{
    private IMapper _mapper;
    private UserManager<Usuario> _userManager;
    private SignInManager<Usuario> _signInManager;
    private TokenService _tokenService;
    private UsuarioContext _context;

    public UsuarioService
        (IMapper mapper,
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        TokenService tokenService,
        UsuarioContext context)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task Cadastrar(UsuarioDTO dto)
    {
        TrataStrings(dto);
        Usuario usuario = _mapper.Map<Usuario>(dto);

        usuario.Permissoes = new List<Permissao>();
        foreach (var permissao in dto.Permissoes)
        {
            foreach (var recurso in permissao.Recursos)
            {
                usuario.Permissoes.Add(new Permissao
                {
                    PermissaoRecurso = recurso.PermissaoRecurso,
                    RecursoId = recurso.Id
                });
            }
        }

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

    //---------------------------[CONSULTAS]-------------------------------
    public ListagemDTO<UsuarioSimplificadoDTO> Listar(int? pagina, int? status, string? search)
    {
        var take = 15;
        var listagemDTO = new ListagemDTO<UsuarioSimplificadoDTO>();
        var query = _context.Usuarios.AsQueryable();

        //params
        if (status.HasValue)
            query = query.Where(w => w.Status == status);


        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.BuscarPalavraChave(search);
        }

        query = query.OrderByDescending(o => o.Status == 1);

        listagemDTO.TotalItems = query.Count();

        if (pagina.HasValue)
        {
            listagemDTO.Page = pagina ?? 0;
            listagemDTO.CalcularTotalPage();

            var skip = (pagina.Value - 1) * take;
            query = query.Skip(skip).Take(take);
        }


        var lista = query.Select(s =>
        new UsuarioSimplificadoDTO
        {
            Id = s.Id,
            Status = s.Status,
            Nome = s.Nome,
            UserName = s.UserName,
            CPF = s.CPF
        }).ToList();

        listagemDTO.Lista = lista;

        return listagemDTO;
    }

    public UsuarioDTO ObterPorID(int id)
    {
        var usuario = _context.Usuarios.Include(u => u.Permissoes).FirstOrDefault(u => u.Id == id);

        var dto = _mapper.Map<UsuarioDTO>(usuario);

        var permissoes = RetornaPermissoes();

        foreach(var permissao in permissoes)
        {
            foreach(var recurso in permissao.Recursos)
            {
                recurso.PermissaoRecurso = usuario.Permissoes
                    .Where(w => w.RecursoId == recurso.Id)
                    .Select(s => s.PermissaoRecurso).FirstOrDefault();
            }
        }

        dto.Permissoes = permissoes;

        return dto;
    }

    public List<PermissoesDTO> RetornaPermissoes()
    {
        var modulos = PermissoesDeRecursos.Modulos;
        var recursos = PermissoesDeRecursos.Recursos;

        var permissoesDeRecurso = modulos
            .Select(modulo => new PermissoesDTO
            {
                Id = modulo.ID,
                Tela = modulo.Nome,
                Recursos = recursos
                    .Where(recurso => recurso.ModuloID == modulo.ID)
                    .Select(recurso => new RecursosDTO
                    {
                        Id = recurso.ID,
                        NomeRecurso = recurso.Nome,
                        PermissaoRecurso = false
                    }).ToList()
            }).ToList();

        return permissoesDeRecurso;
    }

    private void TrataStrings(UsuarioDTO dto)
    {
        dto.CPF = dto.CPF.RemoveNonNumeric();
        dto.Telefone = dto.Telefone.RemoveNonNumeric();
        dto.Endereco.CEP = dto.Endereco.CEP.RemoveNonNumeric();
    }
}

