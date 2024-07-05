using AutoMapper;
using ControleDeLicitacao.App.DTOs;
using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.Common;
using ControleDeLicitacao.Domain.Entities.Documentos.Ata;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Documentos.Ata;

public class AtaService
{
    private readonly IMapper _mapper;
    private readonly AtaRepository _ataRepository;
    public AtaService(IMapper mapper, AtaRepository ataRepository)
    {
        _mapper = mapper;
        _ataRepository = ataRepository;
    }

    public async Task<AtaLicitacao> Adicionar(AtaDTO dto)
    {

        //await ValidarNovoCadastro(dto);

        //TrataStrings(dto);

        var ataLicitacao = _mapper.Map<AtaLicitacao>(dto);

        return await _ataRepository.Adicionar(ataLicitacao);
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
            Empresa = s.EmpresaID,
            Orgao = s.OrgaoID,
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
            .Where(w => w.ID == id)
            .Include(i => i.Itens)
            .FirstOrDefaultAsync();

        if (ataLicitacao == null) return null;

        return _mapper.Map<AtaDTO>(ataLicitacao);
    }

}
