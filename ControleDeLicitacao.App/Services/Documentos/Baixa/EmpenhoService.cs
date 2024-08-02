using AutoMapper;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Documentos.Baixa;

public class EmpenhoService
{
    private readonly IMapper _mapper;
    private readonly BaixaRepository _baixaRepository;
    private readonly EntidadeService _entidadeService;


    public EmpenhoService(IMapper mapper, BaixaRepository baixaRepository, EntidadeService entidadeService)
    {
        _mapper = mapper;
        _baixaRepository = baixaRepository;
        _entidadeService = entidadeService;
    }

    public async Task<EmpenhoDTO?> ObterPorID(int id)
    {
        var empenho = await _baixaRepository.BuscarEmpenhoPorID(id);

        if (empenho is null) return null;

        var dto = _mapper.Map<EmpenhoDTO>(empenho);

        return dto;
    }
    public async Task<bool> PossuiEmpenho(int id)
    {
        var possui = await _baixaRepository
            .BuscarEmpenho()
            .Where(w => w.BaixaID == id)
            .AsNoTracking()
            .AnyAsync();

        return possui;
    }

    public async Task<List<EmpenhoSimplificadoDTO>?> Listar(int idBaixa)
    {
        var empenhos = await _baixaRepository
            .BuscarEmpenho()
            .AsNoTracking()
            .Where(w => w.BaixaID == idBaixa)
            .ToListAsync();

        if (empenhos.Count == 0) return null;

        var lista = new List<EmpenhoSimplificadoDTO>();

        foreach (var item in empenhos)
        {
            var empenho = _mapper.Map<EmpenhoSimplificadoDTO>(item);

            if (empenho is not null)
            {
                if (item.OrgaoID != 0)
                    empenho.Orgao = $"{item.OrgaoID} - {_entidadeService.ObterNome(item.OrgaoID)}";

                if (item.Unidade != 0)
                    empenho.Unidade = $"{item.Unidade} - {_entidadeService.ObterNome(item.Unidade)}";

                lista.Add(empenho);
            }
        }

        return lista;
    }

    public async Task<List<ItemDeEmpenhoDTO>?> ListarItens(int id)
    {
        var empenho = await _baixaRepository.BuscarEmpenhoPorID(id);

        if (empenho is null) return null;

        var itens = empenho.Itens.Where(i => i.QtdeAEntregar > 0).ToList();

        var lista = new List<ItemDeEmpenhoDTO>();

        foreach (var item in itens)
        {
            var dto = _mapper.Map<ItemDeEmpenhoDTO>(item);

            if (empenho is not null)
            {
                lista.Add(dto);
            }
        }

        return lista;
    }


    public async Task<EmpenhoSimplificadoDTO> Adicionar(BaixaDTO dto)
    {
        Empenho empenho = new Empenho()
        {
            ID = 0,
            Status = dto.Status,
            BaixaID = dto.ID,
            DataEmpenho = DateTime.Now,
            Edital = dto.Edital,
            OrgaoID = dto.Orgao,
            Unidade = dto.Orgao,
            Saldo = 0,
            Valor = 0,
            Itens = new List<ItemDeEmpenho>()
        };

        await _baixaRepository.AdicionarEmpenho(empenho);

        return _mapper.Map<EmpenhoSimplificadoDTO>(empenho);
    }

    public async Task Excluir(int id)
    {
        var empenho = await _baixaRepository
            .BuscarEmpenho()
            .AsNoTracking()
            .Where(x => x.ID == id)
            .Include(i => i.Itens)
                .AsNoTracking()
            .FirstOrDefaultAsync();

        if (empenho is null) throw new GenericException("Não foi possivel excluir", 501);

        await _baixaRepository.ExcluirEmpenho(empenho);
    }

    public async Task Editar(EmpenhoDTO dto)
    {
        var empenho = _mapper.Map<Empenho>(dto);

        if (empenho is not null)
        {
            if (empenho.Itens.Any())
            {
                var entregue = empenho.Itens.Sum(x => x.ValorEntregue);

                empenho.Valor = empenho.Status == 2 ? entregue : empenho.Valor;
                empenho.Saldo = empenho.Valor - entregue;
            }
            await _baixaRepository.EditarEmpenho(empenho);
        }
    }

}
