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
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new GenericException("Senha em branco", 501);

        Usuario usuario = _mapper.Map<Usuario>(dto);

        usuario.Permissoes = ConverterPermissoes(dto);

        IdentityResult resultado = await _userManager.CreateAsync(usuario, dto.Password);


        if (!resultado.Succeeded)
            throw new GenericException("Falha ao cadastrar usuário!", 501);

    }

    public async Task<string> Login(LoginUsuarioDTO dto)
    {
        var resultado = await _signInManager.PasswordSignInAsync
            (dto.UserName, dto.Password, false, false);

        if (!resultado.Succeeded)
            throw new GenericException("Usuário não autenticado!", 501);

        var usuario = await _signInManager
            .UserManager
            .Users.Include(u => u.Permissoes.Where(p => p.PermissaoRecurso == true))
            .FirstOrDefaultAsync(user => user.NormalizedUserName == dto.UserName.ToUpper());

        var token = _tokenService.GenerateToken(usuario);

        return token;
    }

    public async Task EditarAsync(UsuarioDTO dto)
    {
        TrataStrings(dto);

        var usuario = await _userManager.FindByIdAsync(dto.Id.ToString());

        if (usuario is null)
        {
            throw new GenericException("Usuário não encontrado!", 404);
        }

        await RemoverTodasPermissoes(usuario.Id);

        usuario.Permissoes = ConverterPermissoes(dto);

        _mapper.Map(dto, usuario);

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            if (dto.Password.Equals(dto.RePassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var result = await _userManager.ResetPasswordAsync(usuario, token, dto.Password);

                if (!result.Succeeded)
                {
                    var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new GenericException($"Falha ao alterar senha do usuário: {errorMessage}", 501);
                }
            }
            else
            {
                throw new GenericException("A nova senha e a confirmação da senha não coincidem!", 400);
            }
        }

        // Atualiza o usuário
        var updateResult = await _userManager.UpdateAsync(usuario);

        if (!updateResult.Succeeded)
        {
            var errorMessage = string.Join("; ", updateResult.Errors.Select(e => e.Description));
            throw new GenericException($"Falha ao editar usuário: {errorMessage}", 501);
        }
    }

    //---------------------------[CONSULTAS]-------------------------------
    public async Task<ListagemDTO<UsuarioSimplificadoDTO>> Listar(int? pagina, int? status, string? search)
    {
        var take = 15;
        var listagemDTO = new ListagemDTO<UsuarioSimplificadoDTO>();
        var query = _context.Usuarios
            .AsQueryable()
            .Where(u => u.UserName != "@leo.szychta");

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


        var lista = await query
            .Select(s =>
            new UsuarioSimplificadoDTO
            {
                Id = s.Id,
                Status = s.Status,
                Nome = s.Nome,
                UserName = s.UserName,
                Telefone = s.Telefone,
                Email = s.Email
            }).ToListAsync();

        listagemDTO.Lista = lista;

        return listagemDTO;
    }

    public async Task<UsuarioDTO> ObterPorID(int id)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .Include(u => u.Permissoes)
            .FirstOrDefaultAsync(u => u.Id == id);

        var dto = _mapper.Map<UsuarioDTO>(usuario);

        var permissoes = RetornaPermissoes();

        foreach (var permissao in permissoes)
        {
            foreach (var recurso in permissao.Recursos)
            {
                recurso.PermissaoRecurso = usuario.Permissoes
                    .Where(w => w.RecursoId == recurso.Id)
                    .Select(s => s.PermissaoRecurso)
                    .FirstOrDefault();
            }
        }

        dto.Permissoes = permissoes;

        return dto;
    }

    public async Task<bool> ObterUsuarioExistente(string userName)
    {
        return await _context.Usuarios.AnyAsync(u => u.UserName == userName);
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

    private List<Permissao> ConverterPermissoes(UsuarioDTO dto)
    {
        var userPerm = new List<Permissao>();
        foreach (var permissao in dto.Permissoes)
        {
            foreach (var recurso in permissao.Recursos)
            {
                userPerm.Add(new Permissao
                {
                    PermissaoRecurso = recurso.PermissaoRecurso,
                    RecursoId = recurso.Id
                });
            }
        }

        return userPerm.ToList();
    }
    private async Task RemoverTodasPermissoes(int id)
    {
        var permissoesExistentes = _context.Permissoes.Where(p => p.UsuarioId == id).ToList();
        _context.Permissoes.RemoveRange(permissoesExistentes);
        await _context.SaveChangesAsync();
    }
}

