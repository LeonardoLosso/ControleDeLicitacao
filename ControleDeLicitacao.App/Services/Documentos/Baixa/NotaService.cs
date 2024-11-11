using AutoMapper;
using ControleDeLicitacao.App.DTOs.Baixa.NotasEmpenhos;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa.NotasEmpenho;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Documentos.Baixa;

public class NotaService
{
    private readonly IMapper _mapper;
    private readonly BaixaRepository _baixaRepository;
    private readonly EntidadeService _entidadeService;

    public NotaService(IMapper mapper, BaixaRepository baixaRepository, EntidadeService entidadeService)
    {
        _mapper = mapper;
        _baixaRepository = baixaRepository;
        _entidadeService = entidadeService;
    }

    public async Task<NotaDTO?> ObterPorID(int id)
    {
        var nota = await _baixaRepository.BuscarNotaPorID(id);

        if (nota is null) return null;

        var dto = _mapper.Map<NotaDTO>(nota);

        return dto;
    }
    public async Task<List<NotaSimplificadaDTO>?> Listar(int id, int? tipo)
    {
        var notas = tipo == 1? await ListarNotasPolicia(id) : await ListarNotasPrefeitura(id);

        if (notas.Count == 0) return null;

        var lista = new List<NotaSimplificadaDTO>();

        foreach (var item in notas)
        {
            var nota = _mapper.Map<NotaSimplificadaDTO>(item);

            if (nota is not null)
            {
                if (item.Unidade != 0)
                    nota.Unidade = $"{item.Unidade} - {_entidadeService.ObterNome(item.Unidade)}";

                lista.Add(nota);
            }
        }

        return lista;
    }
    public async Task<NotaDTO?> Adicionar(NotaDTO dto)
    {
        dto.Itens = AgruparItens(dto);

        await ValidarEmpenhoInativo(dto.EmpenhoID);

        var nota = _mapper.Map<Nota>(dto);

        if (nota is null) return null;

        await _baixaRepository.AdicionarNota(nota);

        return _mapper.Map<NotaDTO>(dto);
    }
    public async Task Editar(NotaDTO dto)
    {
        dto.Itens = AgruparItens(dto);
        await ValidarEmpenhoInativo(dto.EmpenhoID);

        var nota = _mapper.Map<Nota>(dto);

        if (nota is not null)
            await _baixaRepository.EditarNota(nota);
    }
    public async Task Excluir(int id)
    {
        var nota = await _baixaRepository.BuscarNotaPorID(id);

        if (nota is null) throw new GenericException("Não foi possivel excluir", 501);

        await ValidarEmpenhoInativo(nota.EmpenhoID);

        await _baixaRepository.ExcluirNota(nota);
    }
    private async Task ValidarEmpenhoInativo(int empenhoID)
    {
        var empenhoInativo = await _baixaRepository
            .BuscarEmpenho()
            .AsNoTracking()
            .Where(
                e => e.ID == empenhoID &&
                e.Status == 2
            ).AnyAsync();

        if (empenhoInativo) throw new GenericException("O empenho está encerrado", 501);
    }
    private List<ItemDeNotaDTO> AgruparItens(NotaDTO dto)
    {
        var itens = dto.Itens;

        var groupedItens = itens
            .GroupBy(i => i.ID)
            .Select(g => new ItemDeNotaDTO
            {
                ID = g.Key,
                EmpenhoID = dto.EmpenhoID,
                NotaID = dto.ID,
                Nome = g.First().Nome,
                Unidade = g.First().Unidade,
                Quantidade = g.Sum(i => i.Quantidade),
                ValorUnitario = g.OrderBy(i => i.ValorUnitario).First().ValorUnitario,
                ValorTotal = g.OrderBy(i => i.ValorUnitario).First().ValorUnitario * g.Sum(i => i.Quantidade),
                QtdeCaixa = g.First().QtdeCaixa,
                ValorCaixa = g.First().ValorCaixa
            })
            .ToList();

        return groupedItens;
    }

    private async Task<List<Nota>> ListarNotasPrefeitura(int id)
    {
        return await _baixaRepository
            .BuscarNota()
            .AsNoTracking()
        .Include(i => i.Itens)
                .AsNoTracking()
            .Where(w => w.EmpenhoID == id)
            .ToListAsync();
    }
    private async Task<List<Nota>> ListarNotasPolicia(int id)
    {
        return await _baixaRepository
            .BuscarNota()
            .AsNoTracking()
        .Include(i => i.Itens)
                .AsNoTracking()
            .Where(w => w.BaixaID == id)
            .ToListAsync();
    }

    //-----------------------------------------
    public async Task<bool> CreateReport(int id, string path)
    {
        var reportFile = path;

        var freport = new Report();

        await setReportData(freport, id);

        freport.Report.Save(reportFile);

        return true;
    }
    public async Task<byte[]> LoadReport(int id, string path)
    {
        var reportFile = path;

        var freport = new Report();
        freport.Report.Load(reportFile);
        await setReportData(freport, id);
        freport.Prepare();

        var pdfExport = new PDFSimpleExport();

        using MemoryStream ms = new MemoryStream();
        pdfExport.Export(freport, ms);

        ms.Flush();
        return ms.ToArray();
    }

    private async Task setReportData(Report freport, int id)
    {
        var data = await ObterPorID(id);
        var unidade = _entidadeService.ObterNome(data.Unidade);
        var baixa = await _baixaRepository.ObterPorID(data.BaixaID);
        var orgao = _entidadeService.ObterNome(baixa.OrgaoID);
        var empresa = _entidadeService.ObterNome(baixa.EmpresaID);

        freport.SetParameterValue("Edital", data.Edital);
        freport.SetParameterValue("Numero", data.NumNota);
        freport.SetParameterValue("Empenho", data.NumEmpenho);
        freport.SetParameterValue("Orgao", orgao);
        freport.SetParameterValue("Empresa", empresa);
        freport.SetParameterValue("Unidade", unidade);
        freport.SetParameterValue("EhPolicia", data.EhPolicia);
        freport.SetParameterValue("DataEmissao", data.DataEmissao);
        freport.SetParameterValue("DataEntrega", data.DataEntrega);
        freport.SetParameterValue("Observacao", data.Observacao);

        freport.Dictionary.RegisterBusinessObject(data.Itens, "itens", 10, true);
    }
}
