using AutoMapper;
using ControleDeLicitacao.App.DTOs;
using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.Common;
using ControleDeLicitacao.Domain.Entities.Documentos.Ata;
using ControleDeLicitacao.Domain.Entities.Documentos.Ata.Reajuste;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Documentos.Ata;

public class AtaService
{
    private readonly IMapper _mapper;
    private readonly AtaRepository _ataRepository;
    private readonly EntidadeService _entidadeService;
    public AtaService(IMapper mapper, AtaRepository ataRepository, EntidadeService entidadeService)
    {
        _mapper = mapper;
        _ataRepository = ataRepository;
        _entidadeService = entidadeService;
    }

    public async Task<AtaDTO> Adicionar(AtaDTO dto)
    {
        AgruparItens(dto);

        var ataLicitacao = _mapper.Map<AtaLicitacao>(dto);

        await _ataRepository.Adicionar(ataLicitacao);

        return _mapper.Map<AtaDTO>(ataLicitacao);
    }

    public async Task<ReajusteDTO> AdicionarReajuste(ReajusteDTO dto)
    {
        if (dto.Itens.Count == 0) throw new GenericException("Adicione itens antes de criar historico", 512);

        var status = await _ataRepository.Buscar()
            .AsNoTracking()
            .Where(w => w.ID == dto.AtaID)
            .Select(s => s.Status).FirstOrDefaultAsync();


        ValidarInativo(status);


        var reajuste = _mapper.Map<Reajuste>(dto);

        await _ataRepository.AdicionarReajuste(reajuste);

        return _mapper.Map<ReajusteDTO>(reajuste);
    }
    public async Task Editar(AtaDTO dto, bool validaStatus = true)
    {
        dto.Itens = AgruparItens(dto);

        if (validaStatus)
        {
            ValidarInativo(dto.Status);
        }

        var ataLicitacao = _mapper.Map<AtaLicitacao>(dto);

        if (ataLicitacao is null) return;

        await _ataRepository.Editar(ataLicitacao);
    }
    
    public async Task ExcluirReajuste(int id)
    {
        var reajuste = await _ataRepository
            .BuscarReajuste()
            .AsNoTracking()
            .Where(w => w.ID == id)
            .Include(i => i.Itens)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        await _ataRepository.ExcluirReajuste(reajuste);

    }

    //---------------------------[CONSULTAS]-------------------------------
    public async Task<ListagemDTO<AtaSimplificadaDTO>> Listar(int? pagina = null,
                                                              int? tipo = null,
                                                              int? status = null,
                                                              int? unidade = null,
                                                              DateTime? dataInicial = null,
                                                              DateTime? dataFinal = null,
                                                              DateTime? dataAtaInicial = null,
                                                              DateTime? dataAtaFinal = null,
                                                              string? search = null)
    {
        var take = 15;
        var listagemDTO = new ListagemDTO<AtaSimplificadaDTO>();
        var query = _ataRepository.Buscar();

        //params
        if (tipo.HasValue)
            query = query.Where(w => w.Tipo == tipo);

        if (status.HasValue)
            query = query.Where(w => w.Status == status);

        if(unidade.HasValue)
            query = query.Where(w => w.Unidade == unidade);

        if (dataInicial.HasValue)
            query = query.Where(w => w.DataLicitacao >= dataInicial);

        if (dataFinal.HasValue)
            query = query.Where(w => w.DataLicitacao <= dataFinal);

        if (dataAtaInicial.HasValue)
            query = query.Where(w => w.DataAta >= dataAtaInicial);

        if (dataAtaFinal.HasValue)
            query = query.Where(w => w.DataAta <= dataAtaFinal);

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
        new AtaSimplificadaDTO
        {
            ID = s.ID,
            Edital = s.Edital,
            Status = s.Status,
            DataAta = s.DataAta,
            DataLicitacao = s.DataLicitacao,
            Empresa = $"{s.EmpresaID} - {_entidadeService.ObterNome(s.EmpresaID)}",
            Orgao = $"{s.OrgaoID} - {_entidadeService.ObterNome(s.OrgaoID)}",
            TotalLicitado = s.TotalLicitado,
            Unidade = s.Unidade
        }).ToListAsync();

        listagemDTO.Lista = lista;

        return listagemDTO;
    }

    public async Task<AtaDTO?> ObterPorID(int id)
    {
        var ataLicitacao = await _ataRepository
            .Buscar()
            .AsNoTracking()
            .Where(w => w.ID == id)
            .Include(i => i.Itens)
            .FirstOrDefaultAsync();

        if (ataLicitacao is null) return null;

        return _mapper.Map<AtaDTO>(ataLicitacao);
    }

    public async Task<List<ReajusteDTO>?> ListarReajuste(int id)
    {
        var reajustes = await _ataRepository
            .BuscarReajuste()
            .AsNoTracking()
            .Where(w => w.AtaID == id)
            .Include(i => i.Itens)
            .ToListAsync();

        if (reajustes.Count == 0) return null;

        var lista = new List<ReajusteDTO>();

        foreach (var item in reajustes)
        {
            var reajuste = _mapper.Map<ReajusteDTO>(item);

            if (reajuste is not null)
            {
                lista.Add(reajuste);
            }
        }
        return lista;
    }

    private void ValidarInativo(int status)
    {
        if (status == 2) throw new GenericException("Não é possivel editar um documento inativo", 501);
    }
    private List<ItemDeAtaDTO> AgruparItens(AtaDTO dto)
    {
        var itens = dto.Itens;

        var groupedItens = itens
            .GroupBy(i => i.ID)
            .Select(g => new ItemDeAtaDTO
            {
                ID = g.Key,
                AtaID = g.First().AtaID,
                Nome = g.First().Nome,
                Unidade = g.First().Unidade,
                Quantidade = g.Sum(i => i.Quantidade),
                ValorUnitario = g.OrderBy(i => i.ValorUnitario).First().ValorUnitario,
                ValorTotal = g.OrderBy(i => i.ValorUnitario).First().ValorUnitario * g.Sum(i => i.Quantidade),
                Desconto = g.OrderBy(i => i.ValorUnitario).First().Desconto
            })
            .ToList();

        return groupedItens;
    }
}
