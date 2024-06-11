using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Models;

namespace Taskfy.API.Services.Auth
{
	public interface IAuthService
	{
		Task<ResponseDTO> RegisterAsync(RegistroModelDTO usuarioModel);
		Task<object> LoginAsync(LoginModelDTO usuarioModel);
		Task<List<Claim>> GetAuthClaims(Usuario usuario);
		JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config);
		string GenerateRefreshToken();
		DateTime ConvertUtcToBrasilTime(DateTime utcDateTime);
	}
}
