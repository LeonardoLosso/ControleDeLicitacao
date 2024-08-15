using AutoMapper;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Services.Documentos.Ata;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;

namespace ControleDeLicitacao.App.Services.Documentos.Baixa;

public class BaixaPoliciaService
{
    private readonly IMapper _mapper;
    private readonly BaixaRepository _baixaRepository;
    private readonly AtaService _ataService;
    public BaixaPoliciaService(IMapper mapper,
                               BaixaRepository baixaRepository,
                               AtaService ataService)
    {
        _mapper = mapper;
        _baixaRepository = baixaRepository;
        _ataService = ataService;
    }
    public async Task<BaixaPoliciaDTO?> ObterPorID(int id)
    {

        var baixaLicitacao = await _baixaRepository.ObterBaixaPoliciaCompletaPorID(id);

        if (baixaLicitacao is null) return null;

        return _mapper.Map<BaixaPoliciaDTO>(baixaLicitacao);
    }

}
