using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Models;

namespace Taskfy.API.Services.Auth;

public class AuthService : IAuthService
{
	private readonly UserManager<Usuario> _userManager;
	private readonly IConfiguration _configuration;

	public AuthService(UserManager<Usuario> userManager, IConfiguration configuration)
	{
		_userManager = userManager;
		_configuration = configuration;
	}

	public async Task<ResponseDTO> RegisterAsync(RegistroModelDTO usuarioModel)
	{
		var usuarioExiste = await _userManager.FindByEmailAsync(usuarioModel.Email!);
		if (usuarioExiste != null)
		{
			return new ResponseDTO { Status = "Erro", Message = "Usuário já registrado!", StatusCode = StatusCodes.Status409Conflict };
		}

		Usuario usuario = new()
		{
			Email = usuarioModel.Email,
			UserName = usuarioModel.UserName,
			SecurityStamp = Guid.NewGuid().ToString()
		};

		var registraUsuario = await _userManager.CreateAsync(usuario, usuarioModel.Password!);

		if (registraUsuario.Succeeded)
		{
			return new ResponseDTO
			{
				Status = "Sucesso",
				Message = "Usuário criado com sucesso!",
				StatusCode = StatusCodes.Status201Created
			};
		}

		return new ResponseDTO
		{
			Status = "Erro",
			Message = "Falha na criação de usuário.",
			StatusCode = StatusCodes.Status500InternalServerError
		};
	}

	public async Task<object> LoginAsync(LoginModelDTO usuarioModel)
	{
		var usuario = await _userManager.FindByEmailAsync(usuarioModel.Email!);
		if (usuario == null || !await _userManager.CheckPasswordAsync(usuario, usuarioModel.Password!))
			return null;

		var authClaims = await GetAuthClaims(usuario);
		var token = GenerateAccessToken(authClaims, _configuration);
		var refreshToken = GenerateRefreshToken();

		if (!int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes))
			throw new InvalidOperationException("Período de validade do refresh token inválido.");

		usuario.RefreshToken = refreshToken;
		usuario.RefreshTokenExpiryTime = ConvertUtcToBrasilTime(DateTime.UtcNow).AddMinutes(refreshTokenValidityInMinutes);

		var atualizaUsuario = await _userManager.UpdateAsync(usuario);

		if (!atualizaUsuario.Succeeded)
			throw new InvalidOperationException("Falha ao atualizar refresh token do usuário.");

		return new
		{
			Token = new JwtSecurityTokenHandler().WriteToken(token),
			RefreshToken = refreshToken,
			Expiration = token.ValidTo
		};
	}

	// Funções auxiliares
	public async Task<List<Claim>> GetAuthClaims(Usuario usuario)
	{
		var userRoles = await _userManager.GetRolesAsync(usuario);

		var authClaims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, usuario.UserName!),
		new Claim(ClaimTypes.Email, usuario.Email!),
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
	};

		authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
		return authClaims;
	}

	public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config)
	{
		var chaveJwt = _config.GetSection("JWT").GetValue<string>("SecretKey") ?? throw new InvalidOperationException("chave secreta inválida.");

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

	public string GenerateRefreshToken()
	{
		var bytesAleatorios = new byte[128];

		using var geradorNumerosAleatorios = RandomNumberGenerator.Create();

		geradorNumerosAleatorios.GetBytes(bytesAleatorios);

		var refreshToken = Convert.ToBase64String(bytesAleatorios);

		return refreshToken;
	}

	public DateTime ConvertUtcToBrasilTime(DateTime utcDateTime)
	{
		var brTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
		return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, brTimeZone);
	}
}
