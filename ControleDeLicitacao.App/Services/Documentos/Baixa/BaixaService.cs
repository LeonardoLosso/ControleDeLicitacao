using AutoMapper;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Documentos.Ata;
using ControleDeLicitacao.Common;
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

        var baixaLicitacao = await _baixaRepository.ObterBaixaCompletaPorID(id);

        if (baixaLicitacao is null) return null;

        return _mapper.Map<BaixaDTO>(baixaLicitacao);
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

    public async Task<BaixaDTO> Adicionar(int id)
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

        return _mapper.Map<BaixaDTO>(baixa);
    }
    public async Task AlterarStatus(BaixaDTO dto)
    {
        //VALIDAR EMPENHOS ATIVOS

        var possuiEmpenhoAtivo = await _baixaRepository
            .BuscarEmpenho()
            .Where(w => w.BaixaID == dto.ID && w.Status == 1)
            .AnyAsync();
        if (possuiEmpenhoAtivo)
            throw new GenericException("O Documento possui Empenhos ativos!", 501);

        var baixa = _mapper.Map<BaixaLicitacao>(dto);

        if(baixa is not null)
        {
            await _baixaRepository.Editar(baixa);
        }
    }
    public async Task Editar(int id)
    {
        try
        {
            var ata = await _ataService.ObterPorID(id);

            var dto = await ObterPorID(id);

            if (dto is not null && ata is not null)
            {
                dto.DataAta = ata.DataAta;
                dto.DataLicitacao = ata.DataLicitacao;
                dto.Vigencia = ata.Vigencia;
                dto.Edital = ata.Edital;
                dto.Empresa = ata.Empresa;
                dto.Orgao = ata.Orgao;



                dto.Itens = new List<ItemDeBaixaDTO>();
                foreach (var item in ata.Itens)
                {
                    var itemBaixa = new ItemDeBaixaDTO()
                    {
                        ID = item.ID,
                        BaixaID = dto.ID,
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
                    dto.Itens.Add(itemBaixa);
                }
                //else
                //{
                //    foreach(var item in dto.Itens)
                //    {
                //        var itemAta = ata.Itens
                //            .Where(i => i.ID == item.ID && i.AtaID == ata.ID)
                //            .FirstOrDefault();

                //        if(itemAta is not null)
                //        {
                //            item.ValorUnitario = itemAta.ValorUnitario;
                //            item.ValorLicitado = itemAta.ValorTotal;
                //            item.Saldo = itemAta.ValorTotal - item.ValorEmpenhado;
                //        }
                //    }
                //}

                var baixa = _mapper.Map<BaixaLicitacao>(dto);
                await _baixaRepository.Editar(baixa);
            }
        }
        catch (Exception ex)
        {
            throw new GenericException("Erro ao editar Baixa!", 513, ex);
        }
    }
}
