using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Models;

namespace Taskfy.API.Services.Auth
{
	public interface IAuthService
	{
		Task<ResponseDTO> RegisterAsync(RegistroModelDTO usuarioModel);
		Task<ResponseDTO> LoginAsync(LoginModelDTO usuarioModel);
		Task<List<Claim>> GetAuthClaims(Usuario usuario);
		string GenerateRefreshToken();
		DateTime ConvertUtcToBrasilTime(DateTime utcDateTime);
	}
}
