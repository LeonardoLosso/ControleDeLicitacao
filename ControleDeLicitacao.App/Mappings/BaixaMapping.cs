using AutoMapper;
using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.DTOs.Baixa.NotasEmpenhos;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa.NotasEmpenho;

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

        CreateMap<BaixaLicitacao, AtaDTO>()
            .ForMember(dest => dest.Empresa, opt => opt.MapFrom(src => src.EmpresaID))
            .ForMember(dest => dest.Orgao, opt => opt.MapFrom(src => src.OrgaoID))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap();

        CreateMap<ItemDeBaixa, ItemDeAtaDTO>()
            .ForMember(dest => dest.QtdeLicitada, opt => opt.MapFrom(src => src.QtdeLicitada))
            .ForMember(dest => dest.ValorLicitado, opt => opt.MapFrom(src => src.ValorLicitado))
            .ForMember(dest => dest.AtaID, opt => opt.MapFrom(src => src.BaixaID))
            .ReverseMap();

        //--------------------------------------------------------------------------------

        CreateMap<Empenho, EmpenhoDTO>()
            .ForMember(dest => dest.Orgao, opt => opt.MapFrom(src => src.OrgaoID))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap();

        CreateMap<ItemDeEmpenho, ItemDeEmpenhoDTO>().ReverseMap();

        CreateMap<Empenho, EmpenhoSimplificadoDTO>()
            .ForMember(dest => dest.Orgao, opt => opt.Ignore())
            .ForMember(dest => dest.Unidade, opt => opt.Ignore());

        //--------------------------------------------------------------------------------

        CreateMap<Nota, NotaDTO>()
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap();

        CreateMap<ItemDeNota, ItemDeNotaDTO>().ReverseMap();

        CreateMap<Nota, NotaSimplificadaDTO>()
            .ForMember(dest => dest.ValorEntregue, opt => opt.MapFrom(src => src.Itens.Sum(i => i.ValorTotal)))
            .ForMember(dest => dest.Unidade, opt => opt.Ignore());

        //--------------------------------------------------------------------------------

        CreateMap<BaixaPolicia, BaixaPoliciaDTO>()
            .ReverseMap();
    }
}
