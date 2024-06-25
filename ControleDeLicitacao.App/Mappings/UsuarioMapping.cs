using AutoMapper;
using ControleDeLicitacao.App.DTOs.User;
using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;

namespace ControleDeLicitacao.App.Mappings;

public class UsuarioMapping : Profile
{
    public UsuarioMapping()
    {
        CreateMap<UsuarioDTO, Usuario>()
            .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
            .ForMember(dest => dest.Permissoes, opt => opt.Ignore());

        CreateMap<Usuario, UsuarioDTO>()
            .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
            .ForMember(dest => dest.Permissoes, opt => opt.Ignore());
    }

}