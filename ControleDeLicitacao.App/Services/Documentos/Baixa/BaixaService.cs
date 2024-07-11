using AutoMapper;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Services.Documentos.Ata;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Documentos.Baixa;

public class BaixaService
{
    private readonly IMapper _mapper;
    private readonly BaixaRepository _baixaRepository;
    private readonly AtaService _ataService;

    public BaixaService(IMapper mapper, BaixaRepository baixaRepository, AtaService ataService)
    {
        _mapper = mapper;
        _baixaRepository = baixaRepository;
        _ataService = ataService;
    }

    public async Task<BaixaDTO?> ObterPorID(int id)
    {

        var baixaLicitacao = await _baixaRepository
            .Buscar()
            .AsNoTracking()
            .Where(w => w.ID == id)
                .Include(w => w.Itens)
                .AsNoTracking()
                .Include(w => w.Empenhos)
                .AsNoTracking()
            .FirstOrDefaultAsync();

        if (baixaLicitacao is null) baixaLicitacao = await CriarBaixa(id);

        if (baixaLicitacao is null) return null;

        return _mapper.Map<BaixaDTO>(baixaLicitacao);
    }

    private async Task<BaixaLicitacao> CriarBaixa(int id)
    {
        var ata = await _ataService.ObterPorID(id);

        if (ata is null) return null;

        var baixa = new BaixaLicitacao()
        {
            ID = ata.ID,
            DataAta = ata.DataAta,
            DataLicitacao = ata.DataLicitacao,
            Vigencia = ata.Vigencia,
            Edital = ata.Edital,
            EmpresaID = ata.Empresa,
            OrgaoID = ata.Orgao,
            Status = ata.Status,
            Empenhos = new List<Empenho>(),
            Itens = new List<ItemDeBaixa>()
        };
        foreach (var item in ata.Itens)
        {
            var itemBaixa = new ItemDeBaixa()
            {
                ID = item.ID,
                BaixaID = baixa.ID,
                Nome = item.Nome,
                Unidade = item.Unidade,
                QtdeEmpenhada = 0,
                QtdeLicitada = item.Quantidade,
                QtdeAEmpenhar = item.Quantidade,
                ValorEmpenhado = 0,
                ValorLicitado = item.ValorTotal,
                Saldo = item.ValorTotal,
                ValorUnitario = item.ValorUnitario,
            };
            baixa.Itens.Add(itemBaixa);
        }

        await _baixaRepository.Adicionar(baixa);

        return baixa;
    }
}
