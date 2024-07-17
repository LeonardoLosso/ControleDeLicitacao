using AutoMapper;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;

namespace ControleDeLicitacao.App.Mappings;

public class BaixaMapping : Profile
{
    public BaixaMapping()
    {
        CreateMap<BaixaLicitacao, BaixaDTO>()
            .ForMember(dest => dest.Empresa, opt => opt.MapFrom(src => src.EmpresaID))
            .ForMember(dest => dest.Orgao, opt => opt.MapFrom(src => src.OrgaoID))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap();

        CreateMap<ItemDeBaixa, ItemDeBaixaDTO>().ReverseMap();

        //--------------------------------------------------------------------------------

        CreateMap<Empenho, EmpenhoDTO>()
            .ForMember(dest => dest.Orgao, opt => opt.MapFrom(src => src.OrgaoID))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap();

        CreateMap<ItemDeEmpenho, ItemDeEmpenhoDTO>().ReverseMap();

        CreateMap<Empenho, EmpenhoSimplificadoDTO>()
            .ForMember(dest => dest.Orgao, opt => opt.MapFrom(src => src.OrgaoID));
    }
}
