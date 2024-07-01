using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ControleDeLicitacao.App.Services.Cadastros.User;

public class TokenService
{
    private const string _key = "F4ca)uNa0FaC4.4T3n74t1VAnAO3xI27E";
    public string GenerateToken(Usuario usuario)
    {
        Claim[] claims = new Claim[]
        {
                new Claim("userName", usuario.UserName),
                new Claim("recursos", 
                    JsonSerializer
                        .Serialize(usuario.Permissoes.Select(p => p.RecursoId)), 
                    JsonClaimValueTypes.JsonArray)
        };

        var chave = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(_key));

        var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
            (expires: DateTime.Now.AddHours(9),
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public JwtSecurityToken DecodeToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken;
    }
}
