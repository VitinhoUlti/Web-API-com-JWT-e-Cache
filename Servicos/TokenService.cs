using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Testezin.Entidades;

namespace Testezin.Servicos
{
    public static class TokenService
    {
        public static string GerarToken(Usuarios usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Chave.chaveSecreta);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GerarClaims(usuario),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static ClaimsIdentity GerarClaims(Usuarios usuario){
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, usuario.Nome));
            return claimsIdentity;
        }
    }
}