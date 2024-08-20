using AutoMapper;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa.NotasEmpenho;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Documentos.Baixa;

public class BaixaPoliciaService
{
    private readonly IMapper _mapper;
    private readonly BaixaRepository _baixaRepository;
    public BaixaPoliciaService(IMapper mapper,
                               BaixaRepository baixaRepository)
    {
        _mapper = mapper;
        _baixaRepository = baixaRepository;
    }
    public async Task<BaixaPoliciaDTO?> ObterPorID(int id)
    {

        var baixaLicitacao = await _baixaRepository.ObterBaixaCompletaPorID(id);

        if (baixaLicitacao is null) return null;

        var baixaPolicia = _mapper.Map<BaixaPoliciaDTO>(baixaLicitacao);
        baixaPolicia.ValorLicitado = baixaLicitacao.Itens.Sum(i => i.ValorLicitado);


        baixaPolicia.Empenhos = await ObterEmpenhos(baixaLicitacao.ID);
        baixaPolicia.ValorEmpenhado = baixaPolicia.Empenhos.Sum(e => e.Valor);

        baixaPolicia.ValorEntregue = await _baixaRepository
            .BuscarNota()
            .AsNoTracking()
            .Where(n => n.BaixaID == id)
            .Select(n => n.Itens.Sum(i => i.ValorTotal))
            .FirstOrDefaultAsync();

        return baixaPolicia;
    }
    public async Task<List<EmpenhoPoliciaDTO>> ObterEmpenhos(int id)
    {
        var empenhos = await _baixaRepository.BuscarEmpenhoPolicia()
            .AsNoTracking()
            .Where(e => e.BaixaID == id)
            .ToListAsync();

        var empenhoRetorno = new List<EmpenhoPoliciaDTO>();
        foreach (var emp in empenhos)
        {
            var aux = _mapper.Map<EmpenhoPoliciaDTO>(emp);
            if (aux is not null)
                empenhoRetorno.Add(aux);
        }
        return empenhoRetorno;
    }

    public async Task Salvar(List<EmpenhoPoliciaDTO> dto, int id)
    {
        ValidarInativo(id);

        var empenhos = new List<EmpenhoPolicia>();
        foreach (var emp in dto)
        {
            var aux = _mapper.Map<EmpenhoPolicia>(emp);
            if (aux is not null)
                empenhos.Add(aux);
        }

        if (!empenhos.Any()) return;

        await _baixaRepository.SalvarBaixaPolicia(empenhos, id);
    }
    private void ValidarInativo(int id)
    {
        var status = _baixaRepository
            .Buscar()
            .AsNoTracking()
            .Where(b => b.ID == id)
            .Select(b => b.Status)
            .FirstOrDefault();

        if (status == 2) throw new GenericException("Não é possivel editar um documento inativo", 501);
    }
}
