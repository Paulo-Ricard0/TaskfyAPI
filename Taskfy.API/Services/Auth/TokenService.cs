using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Taskfy.API.Services.Auth;

public class TokenService : ITokenService
{
	public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config)
	{
		var chaveJwt = _config["JWT:SecretKey"] ?? throw new InvalidOperationException("chave secreta inválida.");

		var chaveSecreta = Encoding.UTF8.GetBytes(chaveJwt);

		var credencialAssinada = new SigningCredentials(new SymmetricSecurityKey(chaveSecreta), SecurityAlgorithms.HmacSha256Signature);

		var TokenValidityInMinutes = _config.GetSection("JWT").GetValue<double>("TokenValidityInMinutes");

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = ConvertUtcToBrasilTime(DateTime.UtcNow).AddMinutes(TokenValidityInMinutes),
			NotBefore = ConvertUtcToBrasilTime(DateTime.UtcNow),
			Audience = _config.GetSection("JWT").GetValue<string>("ValidAudience"),
			Issuer = _config.GetSection("JWT").GetValue<string>("ValidIssuer"),
			SigningCredentials = credencialAssinada
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
		return token;
	}

	public DateTime ConvertUtcToBrasilTime(DateTime utcDateTime)
	{
		var brTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
		return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, brTimeZone);
	}
}
