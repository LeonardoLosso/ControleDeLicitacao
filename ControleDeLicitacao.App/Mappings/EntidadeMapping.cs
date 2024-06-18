using AutoMapper;
using ControleDeLicitacao.App.DTOs.Entidades;
using ControleDeLicitacao.Domain.Entities.Cadastros;
using ControleDeLicitacao.Domain.ValueObjects;

namespace ControleDeLicitacao.App.Mappings;

public class EntidadeMapping : Profile
{
    public EntidadeMapping()
    {
        CreateMap<Entidade, EntidadeDTO>()
            .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
            .ReverseMap();
        
        //CreateMap<EntidadeDTO, Entidade>()
        //    .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco));

        CreateMap<Endereco, EnderecoDTO>().ReverseMap();
    }
}
