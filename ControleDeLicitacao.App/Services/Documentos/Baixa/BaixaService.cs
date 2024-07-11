using AutoMapper;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services.Documentos.Baixa;

public class BaixaService
{
    private readonly IMapper _mapper;
    private readonly BaixaRepository _baixaRepository;

    public BaixaService(IMapper mapper, BaixaRepository baixaRepository)
    {
        _mapper = mapper;
        _baixaRepository = baixaRepository;
    }

    public async Task<BaixaDTO?> ObterPorID(int id)
    {
        var baixaLicitacao = await _baixaRepository
            .Buscar()
            .FirstOrDefaultAsync();

        if(baixaLicitacao is null) return null;

        return null;
    }
}
