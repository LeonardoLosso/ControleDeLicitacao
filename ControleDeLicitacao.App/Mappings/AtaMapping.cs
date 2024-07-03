using AutoMapper;
using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.Domain.Entities.Documentos.Ata;

namespace ControleDeLicitacao.App.Mappings;

public class AtaMapping : Profile
{
    public AtaMapping()
    {
        CreateMap<AtaLicitacao, AtaDTO>()
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap();

        CreateMap<ItemDeAta, ItemDeAtaDTO>().ReverseMap();
    }
}
