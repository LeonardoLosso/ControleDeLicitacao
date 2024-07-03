using AutoMapper;
using ControleDeLicitacao.App.DTOs.Ata;
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
