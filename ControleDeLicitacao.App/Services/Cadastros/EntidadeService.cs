using AutoMapper;
using ControleDeLicitacao.App.DTOs;
using ControleDeLicitacao.App.DTOs.Entidades;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.Common;
using ControleDeLicitacao.Domain.Entities.Cadastros;
using ControleDeLicitacao.Infrastructure.Persistence.Interface;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Cadastros;

public class EntidadeService
{
    private readonly IRepository<Entidade> _entidadeRepository;
    private readonly IMapper _mapper;

    public EntidadeService(IRepository<Entidade> entidadeRepository, IMapper mapper)
    {
        _entidadeRepository = entidadeRepository ?? throw new ArgumentNullException(nameof(entidadeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    }

    public async Task<EntidadeDTO?> Adicionar(EntidadeDTO dto)
    {
        TrataStrings(dto);

        await ValidarNovoCadastro(dto);


        var entidade = _mapper.Map<Entidade>(dto);

        if (entidade is null) return null;

        await _entidadeRepository.Adicionar(entidade);

        return _mapper.Map<EntidadeDTO>(entidade);
    }


    public async Task Editar(EntidadeDTO dto, bool validaStatus = true)
    {
        if (validaStatus)
        {
            ValidarInativo(dto.Status);
            TrataStrings(dto);
        }

        var entidade = _mapper.Map<Entidade>(dto);

        await _entidadeRepository.Editar(entidade);
    }

    //---------------------------[CONSULTAS]-------------------------------
    public async Task<ListagemDTO<EntidadeSimplificadaDTO>> Listar(int? pagina, int? tipo, int? status, string? cidade, string? search)
    {
        var take = 15;
        var listagemDTO = new ListagemDTO<EntidadeSimplificadaDTO>();
        var query = _entidadeRepository.Buscar();

        //params
        if (tipo.HasValue)
        {
            if (tipo.Value != 0)
                query = query.Where(w => w.Tipo == tipo);
            else
                query = query.Where(w => w.Tipo != 1);

        }

        if (status.HasValue)
            query = query.Where(w => w.Status == status);

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(w => w.Endereco.Cidade.Contains(cidade.Trim()));

        if (!string.IsNullOrWhiteSpace(search))
            query = query.BuscarPalavraChave(search);

        query = query.OrderByDescending(o => o.Status == 1);

        listagemDTO.TotalItems = query.Count();

        if (pagina.HasValue)
        {
            listagemDTO.Page = pagina ?? 0;
            listagemDTO.CalcularTotalPage();

            var skip = (pagina.Value - 1) * take;
            query = query.Skip(skip).Take(take);
        }


        var lista = await query.Select(s =>
        new EntidadeSimplificadaDTO
        {
            ID = s.ID,
            Status = s.Status,
            Fantasia = s.Fantasia,
            Tipo = s.Tipo,
            CNPJ = s.CNPJ,
            Telefone = s.Telefone,
            Email = s.Email
        }).ToListAsync();

        listagemDTO.Lista = lista;

        return listagemDTO;
    }

    public async Task<EntidadeDTO?> ObterPorID(int id)
    {
        var entidade = await _entidadeRepository.ObterPorID(id);

        if (entidade is null) return null;

        return _mapper.Map<EntidadeDTO>(entidade);
    }

    public async Task<EntidadeDTO?> ObterPorIDParaEdicao(int id)
    {
        var entidade = await _entidadeRepository.ObterPorID(id);

        if (entidade is null) return null;

        ValidarInativo(entidade.Status);

        return _mapper.Map<EntidadeDTO>(entidade);
    }

    public string ObterNome(int id)
    {
        if (id == 0) return "";

        var entidade = _entidadeRepository.ObterPorID(id).Result.Fantasia;
        return entidade;
    }

    public async Task<EntidadeDTO?> BuscaEntidadesPorCNPJ(EntidadeDTO entidade)
    {
        string cnpj = entidade.CNPJ.RemoveNonNumeric();

        var retorno = _entidadeRepository.Buscar()
            .AsNoTracking()
            .Where(e => e.CNPJ == cnpj)
            .FirstOrDefault();

        if (retorno is not null)
        {
            if (retorno.Status == 1)
                return _mapper.Map<EntidadeDTO>(retorno);

            return entidade;
        }
        return null;
    }
    public List<string> BuscarEmpresas()
    {
        var cnpjs = _entidadeRepository
            .Buscar()
            .AsNoTracking()
            .Where(e => e.Tipo == 1)
            .Select(e => e.CNPJ)
            .ToList();

        if (!cnpjs.Any()) throw new GenericException("Nenhuma empresa cadastrada", 501);
        return cnpjs.Select(cnpj => AplicarMascaraCNPJ(cnpj)).ToList();

    }
    private string AplicarMascaraCNPJ(string cnpj)
    {
        return Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
    }
    //---------------------------------------------------------------------

    private async Task ValidarNovoCadastro(EntidadeDTO dto)
    {
        var cnpjDuplicado = await _entidadeRepository.Buscar().Where(w => w.CNPJ.Equals(dto.CNPJ)).AnyAsync();

        if (cnpjDuplicado) throw new GenericException("CNPJ duplicado no banco de dados", 501);
    }
    private void ValidarInativo(int status)
    {
        if (status == 2) throw new GenericException("Não é possivel editar um cadastro inativo", 501);
    }

    private void TrataStrings(EntidadeDTO dto)
    {
        dto.CNPJ = dto.CNPJ.RemoveNonNumeric();
        dto.Telefone = dto.Telefone.RemoveNonNumeric();
        dto.Endereco.CEP = dto.Endereco.CEP.RemoveNonNumeric();
    }
}
