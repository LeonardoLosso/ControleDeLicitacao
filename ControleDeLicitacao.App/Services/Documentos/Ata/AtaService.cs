using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.Domain.Entities.Documentos.Ata;

namespace ControleDeLicitacao.App.Services.Documentos.Ata;

public class AtaService
{

    public async Task<AtaLicitacao> Adicionar(AtaDTO dto)
    {

        //await ValidarNovoCadastro(dto);

        //TrataStrings(dto);

        var entidade = _mapper.Map<Entidade>(dto);

        //await _entidadeRepository.Adicionar(entidade);

        return new AtaLicitacao();
    }

    //---------------------------[CONSULTAS]-------------------------------

    //public async Task<EntidadeDTO?> ObterPorID(int id)
    //{
    //    var entidade = await _entidadeRepository.ObterPorID(id);

    //    if (entidade == null) return null;

    //    return _mapper.Map<EntidadeDTO>(entidade);
    //}

}
