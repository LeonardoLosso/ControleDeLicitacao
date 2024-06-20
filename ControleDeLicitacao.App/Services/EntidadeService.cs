using AutoMapper;
using ControleDeLicitacao.App.DTOs;
using ControleDeLicitacao.App.DTOs.Entidades;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.Common;
using ControleDeLicitacao.Domain.Entities.Cadastros;
using ControleDeLicitacao.Infrastructure.Persistence.Interface;

namespace ControleDeLicitacao.App.Services;

public class EntidadeService
{
    private readonly IRepository<Entidade> _entidadeRepository;
    private readonly IMapper _mapper;

    public EntidadeService(IRepository<Entidade> entidadeRepository, IMapper mapper)
    {
        _entidadeRepository = entidadeRepository ?? throw new ArgumentNullException(nameof(entidadeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    }

    public EntidadeDTO ObterPorID(int id)
    {
        var entidade = _entidadeRepository.ObterPorID(id);

        if (entidade == null) return null;

        return _mapper.Map<EntidadeDTO>(entidade);
    }
    public void Adicionar(EntidadeDTO dto)
    {

        ValidarNovoCadastro(dto);

        var entidade = _mapper.Map<Entidade>(dto);

        _entidadeRepository.Adicionar(entidade);
    }


    public void Editar(EntidadeDTO dto, bool edit = false)
    {
        if (edit) ValidarInativo(dto.Status);

        var entidade = _mapper.Map<Entidade>(dto);

        _entidadeRepository.Editar(entidade);
    }
    public void AlterarStatus(int id)
    {
        var entidade = _entidadeRepository.ObterPorID(id);
        //verificar se a entidade está dentro de algum processo em aberto (para itens tambem)


        entidade.Status = entidade.Status == 1 ? 2 : 1;

        _entidadeRepository.Editar(entidade);
    }
    public ListagemDTO<EntidadeSimplificadaDTO> Listar(int? pagina, int? tipo, int? status, string? cidade, string? search)
    {
        var take = 15;
        var listagemDTO = new ListagemDTO<EntidadeSimplificadaDTO>();
        var query = _entidadeRepository.Buscar();

        //params
        if (tipo.HasValue)
            query = query.Where(w => w.Tipo == tipo);

        if (status.HasValue)
            query = query.Where(w => w.Status == status);

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(w => w.Endereco.Cidade.Contains(cidade.Trim()));

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.BuscarPalavraChave(search);
        }

        listagemDTO.TotalItems = query.Count();
        if (pagina.HasValue)
        {
            listagemDTO.Page = pagina ?? 0;
            listagemDTO.CalcularTotalPage();

            var skip = (pagina.Value - 1) * take;
            query = query.Skip(skip).Take(take);
        }


        var lista = query.Select(s =>
        new EntidadeSimplificadaDTO
        {
            ID = s.ID,
            Status = s.Status,
            Fantasia = s.Fantasia,
            Tipo = s.Tipo,
            CNPJ = s.CNPJ,
            Telefone = s.Telefone,
            Email = s.Email
        }).ToList();

        listagemDTO.Lista = lista;

        return listagemDTO;
    }

    private void ValidarNovoCadastro(EntidadeDTO dto)
    {
        var cnpjDuplicado = _entidadeRepository.Buscar().Where(w => w.CNPJ.Equals(dto.CNPJ)).Any();

        if (cnpjDuplicado) throw new GenericException("CNPJ duplicado no banco de dados", 501);
    }
    private void ValidarInativo(int status)
    {
        if (status == 2) throw new GenericException("Não é possivel editar um cadastro inativo", 501);
    }
}
