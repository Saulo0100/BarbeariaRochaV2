using BarbeariaRocha.Modelos.Entidades;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BarbeariaRocha.Infraestrutura;

public class TokenProvider(IConfiguration configuration)
{
    public string CreateToken(Usuario usuario)
    {
        string? secretKey = configuration.GetSection("Jwt:Secret").Value;
        if (string.IsNullOrEmpty(secretKey))
            throw new InvalidOperationException("Jwt:Secret não está configurado.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                new Claim("Id", usuario.Id.ToString()),
                new Claim("Numero", usuario.Numero),
                new Claim("Nome", usuario.Nome),
                new Claim("Perfil", usuario.Perfil.ToString())
                ]),
            Expires = DateTime.UtcNow.AddHours(12),
            SigningCredentials = credential,
            Issuer = configuration.GetSection("Jwt:Issuer").Value,
            Audience = configuration.GetSection("Jwt:Audience").Value
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}
