using AutoMapper;
using ControleDeLicitacao.App.DTOs;
using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.Common;
using ControleDeLicitacao.Domain.Entities.Documentos.Ata.Reajuste;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Documentos.Baixa;

public class BaixaService
{
    private readonly IMapper _mapper;
    private readonly BaixaRepository _baixaRepository;
    private readonly EntidadeService _entidadeService;


    public BaixaService(IMapper mapper, BaixaRepository baixaRepository, EntidadeService entidadeService)
    {
        _mapper = mapper;
        _baixaRepository = baixaRepository;
        _entidadeService = entidadeService;
    }

    public async Task<BaixaDTO?> ObterPorID(int id)
    {

        var baixaLicitacao = await _baixaRepository.ObterBaixaCompletaPorID(id);

        if (baixaLicitacao is null) return null;

        return _mapper.Map<BaixaDTO>(baixaLicitacao);
    }

    public async Task<AtaDTO?> ObterPorIDMapAta(int id)
    {

        var baixaLicitacao = await _baixaRepository.ObterBaixaCompletaPorID(id);

        if (baixaLicitacao is null) return null;

        return _mapper.Map<AtaDTO>(baixaLicitacao);
    }

    public async Task<List<ItemDeBaixaDTO>> ObterItensPorID(int id, string? search = null)
    {
        var query = _baixaRepository
            .BuscarItens()
            .AsNoTracking()
            .Where(w => w.BaixaID == id && w.QtdeAEmpenhar > 0);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.BuscarPalavraChave(search);
        }
        var itens = await query.Select(s => _mapper.Map<ItemDeBaixaDTO>(s)).ToListAsync();

        return itens;
    }

    public async Task<AtaDTO> Adicionar(AtaDTO dto)
    {
        dto.Itens = AgruparItens(dto);

        var ataLicitacao = _mapper.Map<BaixaLicitacao>(dto);

        foreach (var item in ataLicitacao.Itens)
        {
            item.QtdeAEmpenhar = item.QtdeLicitada;
            item.Saldo = item.ValorLicitado;
        }
        await _baixaRepository.Adicionar(ataLicitacao);

        return _mapper.Map<AtaDTO>(ataLicitacao);
    }

    public async Task Editar(BaixaDTO dto, bool validaStatus = true)
    {
        dto.Itens = AgruparItensBaixa(dto);

        if (validaStatus)
        {
            ValidarInativo(dto.Status);
        }

        var baixa = _mapper.Map<BaixaLicitacao>(dto);

        if (baixa is null) return;

        await _baixaRepository.Editar(baixa);
    }
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
        var query = _baixaRepository.Buscar();

        if (tipo.HasValue)
        {
            if (tipo == 3)
                query = query.Where(w => w.Unidade == tipo);
            else query = query.Where(w => w.Unidade != 3);
        }

        if (status.HasValue)
            query = query.Where(w => w.Status == status);

        if (unidade.HasValue)
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
        {
            var aux = query.BuscarPalavraChave(search);
            if (aux.Any())
                query = aux;
            else
            {
                var entidadeAux = _entidadeService.BuscarQueryable(search);
                query = query
                    .Where(w => entidadeAux.Contains(w.EmpresaID) ||
                    entidadeAux.Contains(w.OrgaoID) ||
                    entidadeAux.Contains(w.Unidade));
            }
        }

        query = query.OrderByDescending(o => o.DataAta);

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
            Saldo = s.Itens.Sum(i => i.Saldo),
            TotalLicitado = s.Itens.Sum(i => i.ValorLicitado),
            Unidade = s.Unidade
        }).ToListAsync();

        listagemDTO.Lista = lista;

        return listagemDTO;
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
                QtdeLicitada = g.Sum(i => i.QtdeLicitada),
                ValorUnitario = g.OrderBy(i => i.ValorUnitario).First().ValorUnitario,
                ValorLicitado = g.OrderBy(i => i.ValorUnitario).First().ValorUnitario * g.Sum(i => i.QtdeLicitada),
                Desconto = g.OrderBy(i => i.ValorUnitario).First().Desconto
            })
            .ToList();

        return groupedItens;
    }
    private List<ItemDeBaixaDTO> AgruparItensBaixa(BaixaDTO dto)
    {
        var itens = dto.Itens;

        var groupedItens = itens
            .GroupBy(i => i.ID)
            .Select(g => new ItemDeBaixaDTO
            {
                ID = g.Key,
                BaixaID = g.First().BaixaID,
                Nome = g.First().Nome,
                Unidade = g.First().Unidade,
                QtdeLicitada = g.Sum(i => i.QtdeLicitada),
                ValorUnitario = g.OrderBy(i => i.ValorUnitario).First().ValorUnitario,
                ValorLicitado = g.OrderBy(i => i.ValorUnitario).First().ValorUnitario * g.Sum(i => i.QtdeLicitada),
                Desconto = g.OrderBy(i => i.ValorUnitario).First().Desconto,
                QtdeEmpenhada = g.Sum(i => i.QtdeEmpenhada),
                QtdeAEmpenhar = g.Sum(i => i.QtdeLicitada) - g.Sum(i => i.QtdeEmpenhada),
                ValorEmpenhado = g.Sum(i => i.ValorEmpenhado),
                Saldo = g.Sum(i => i.ValorLicitado - i.ValorEmpenhado)
            })
            .ToList();

        return groupedItens;
    }
    private void ValidarInativo(int status)
    {
        if (status == 2) throw new GenericException("Não é possivel editar um documento inativo", 501);
    }

    //----------------------------------------------------------
    public async Task<ReajusteDTO> AdicionarReajuste(ReajusteDTO dto)
    {
        if (dto.Itens.Count == 0) throw new GenericException("Adicione itens antes de criar historico", 512);

        var status = await _baixaRepository.Buscar()
            .AsNoTracking()
            .Where(w => w.ID == dto.AtaID)
            .Select(s => s.Status).FirstOrDefaultAsync();


        ValidarInativo(status);


        var reajuste = _mapper.Map<Reajuste>(dto);

        await _baixaRepository.AdicionarReajuste(reajuste);

        return _mapper.Map<ReajusteDTO>(reajuste);
    }
    public async Task ExcluirReajuste(int id)
    {
        var reajuste = await _baixaRepository
            .BuscarReajuste()
            .AsNoTracking()
            .Where(w => w.ID == id)
            .Include(i => i.Itens)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        await _baixaRepository.ExcluirReajuste(reajuste);
    }
    public async Task<List<ReajusteDTO>?> ListarReajuste(int id)
    {
        var reajustes = await _baixaRepository
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
}
