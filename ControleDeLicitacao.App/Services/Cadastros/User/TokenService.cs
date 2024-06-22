using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ControleDeLicitacao.App.Services.Cadastros.User;

public class TokenService
{
    public string GenerateToken(Usuario usuario)
    {
        Claim[] claims = new Claim[]
        {
                new Claim("username", usuario.UserName)
        };


        var chave = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes
            ("S0UF0D4"));

        var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
            (expires: DateTime.Now.AddMinutes(10),
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
