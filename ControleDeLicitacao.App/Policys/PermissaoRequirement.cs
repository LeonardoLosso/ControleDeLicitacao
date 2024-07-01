using Microsoft.AspNetCore.Authorization;

namespace ControleDeLicitacao.App.Policys;

public class PermissaoRequirement : IAuthorizationRequirement
{
    public int Permissao { get; }
    public PermissaoRequirement(int permissao)
    {
        Permissao = permissao;
    }
}
