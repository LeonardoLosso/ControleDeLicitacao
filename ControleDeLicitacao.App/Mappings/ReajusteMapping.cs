using AutoMapper;
using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.Domain.Entities.Documentos.Ata.Reajuste;

namespace ControleDeLicitacao.App.Mappings;

public class ReajusteMapping : Profile
{
    public ReajusteMapping()
    {
        CreateMap<Reajuste, ReajusteDTO>()
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens)).ReverseMap();

        CreateMap<ItemDeReajuste, ItemDeReajusteDTO>().ReverseMap();
    }
}
