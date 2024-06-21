using AutoMapper;
using ControleDeLicitacao.App.DTOs.Itens;
using ControleDeLicitacao.Domain.Entities.Cadastros;
using ControleDeLicitacao.Domain.ValueObjects;

namespace ControleDeLicitacao.App.Mappings;

public class ItemMapping : Profile
{
    public ItemMapping()
    {
        CreateMap<Item, ItemDTO>()
            .ForMember(dest => dest.ListaItens, opt => opt.MapFrom(src => src.ListaItens))
            .ForMember(dest => dest.ListaNomes, opt => opt.MapFrom(src => src.ListaNomes.Select(n => n.Nome)));


        CreateMap<ItemAssociativo, ItemSimplificadoDTO>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ItemFilhoID))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ItemFilho.Status))
            .ForMember(dest => dest.EhCesta, opt => opt.MapFrom(src => src.ItemFilho.EhCesta))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.ItemFilho.Nome))
            .ForMember(dest => dest.UnidadePrimaria, opt => opt.MapFrom(src => src.ItemFilho.UnidadePrimaria))
            .ForMember(dest => dest.UnidadeSecundaria, opt => opt.MapFrom(src => src.ItemFilho.UnidadeSecundaria));


        CreateMap<ItemDTO, Item>()
            .ForMember(dest => dest.ListaItens, opt => opt.Ignore())
            .ForMember(dest => dest.ListaNomes, opt => opt.Ignore());

    }
}
