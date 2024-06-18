//using ControleDeLicitacao.API.User;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace ControleDeLicitacao.API.Services
//{
//    public class TokenService
//    {
//        public string GenerateToken(Usuario usuario)
//        {
//            Claim[] claims = new Claim[]
//            {
//                new Claim("username", usuario.UserName),
//                new Claim("id", usuario.Id)
//            };

//            //as credenciais precisam de uma chave que pode ou não ter uma lógica de geração

//            var chave = new SymmetricSecurityKey
//                (Encoding.UTF8.GetBytes
//                ("S0UF0D4"));

//            var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken
//                (expires: DateTime.Now.AddMinutes(10),
//                claims: claims,
//                signingCredentials: signingCredentials);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}
