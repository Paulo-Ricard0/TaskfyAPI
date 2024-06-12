using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Logs;
using Taskfy.API.Models;

namespace Taskfy.API.Services.Auth;

public class AuthService : IAuthService
{
	private readonly UserManager<Usuario> _userManager;
	private readonly IConfiguration _configuration;
	private readonly ITokenService _tokenService;
	private readonly ILog _logger;

	public AuthService(UserManager<Usuario> userManager, IConfiguration configuration, ITokenService tokenService, ILog logger)
	{
		_userManager = userManager;
		_configuration = configuration;
		_tokenService = tokenService;
		_logger = logger;
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
			Name = usuarioModel.Name,
			Email = usuarioModel.Email,
			UserName = usuarioModel.Email,
			SecurityStamp = Guid.NewGuid().ToString()
		};

		var registraUsuario = await _userManager.CreateAsync(usuario, usuarioModel.Password);

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

	public async Task<ResponseDTO> LoginAsync(LoginModelDTO usuarioModel)
	{
		var usuario = await _userManager.FindByEmailAsync(usuarioModel.Email);
		if (usuario == null || !await _userManager.CheckPasswordAsync(usuario, usuarioModel.Password))
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Email ou senha incorretos.",
				StatusCode = StatusCodes.Status401Unauthorized
			};
		}

		var authClaims = await GetAuthClaims(usuario);
		var token = _tokenService.GenerateAccessToken(authClaims, _configuration);
		var refreshToken = GenerateRefreshToken();

		if (!int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes))
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Período de validade do refresh token inválido.",
				StatusCode = StatusCodes.Status500InternalServerError
			};
		}

		usuario.RefreshToken = refreshToken;
		usuario.RefreshTokenExpiryTime = _tokenService.ConvertUtcToBrasilTime(DateTime.UtcNow).AddMinutes(refreshTokenValidityInMinutes);

		var atualizaUsuario = await _userManager.UpdateAsync(usuario);

		if (!atualizaUsuario.Succeeded)
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Falha ao atualizar refresh token do usuário.",
				StatusCode = StatusCodes.Status500InternalServerError
			};
		}

		_logger.LogToFile("Login - Sucesso", "Login efetuado com sucesso!");
		return new ResponseLoginTokenDTO
		{
			Token = new JwtSecurityTokenHandler().WriteToken(token),
			RefreshToken = refreshToken,
			Expiration = token.ValidTo,
			StatusCode = StatusCodes.Status200OK
		};
	}

	// Funções auxiliares
	public async Task<List<Claim>> GetAuthClaims(Usuario usuario)
	{
		var userRoles = await _userManager.GetRolesAsync(usuario);

		var authClaims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, usuario.Name),
		new Claim(ClaimTypes.Email, usuario.Email!),
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
	};

		authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
		return authClaims;
	}

	public string GenerateRefreshToken()
	{
		var bytesAleatorios = new byte[128];

		using var geradorNumerosAleatorios = RandomNumberGenerator.Create();

		geradorNumerosAleatorios.GetBytes(bytesAleatorios);

		var refreshToken = Convert.ToBase64String(bytesAleatorios);

		return refreshToken;
	}
}
