using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class CustomUserStore : UserStore<Usuario, IdentityRole<int>, UsuarioContext, int>
{
    public CustomUserStore(UsuarioContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }
}
