using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Models;
using Taskfy.API.Services.Auth;

namespace Taskfy.Tests.Unit.Auth.Services
{
	public class LoginServiceTests
	{
		[Fact]
		public async Task DeveRetornarSucesso_QuandoUsuarioExistirComSenhaCorreta()
		{
			// Arrange
			var usuarioModel = new LoginModelDTO
			{
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var usuario = new Usuario
			{
				Email = "test@gmail.com",
				UserName = "testuser",
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			userManager.FindByEmailAsync(usuarioModel.Email).Returns(Task.FromResult<Usuario?>(usuario));
			userManager.CheckPasswordAsync(usuario, usuarioModel.Password).Returns(Task.FromResult(true));
			userManager.GetRolesAsync(usuario).Returns(Task.FromResult<IList<string>>(new List<string>()));
			userManager.UpdateAsync(usuario).Returns(Task.FromResult(IdentityResult.Success));

			var configuration = Substitute.For<IConfiguration>();
			configuration["JWT:SecretKey"].Returns("7b4093b95b3c848b3d3b961da2203c848a09093d");
			configuration["JWT:ValidAudience"].Returns("http://localhost:7066");
			configuration["JWT:ValidIssuer"].Returns("http://localhost:5066");
			configuration["JWT:TokenValidityInMinutes"].Returns("30");
			configuration["JWT:RefreshTokenValidityInMinutes"].Returns("60");

			var mockTokenService = Substitute.For<ITokenService>();
			mockTokenService.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>(), Arg.Any<IConfiguration>())
				.Returns(CreateMockJwtToken());

			var authService = new AuthService(userManager, configuration, mockTokenService);

			// Act
			var resultado = await authService.LoginAsync(usuarioModel) as ResponseLoginTokenDTO;

			// Assert
			resultado.Should().NotBeNull();
			resultado.StatusCode.Should().Be(StatusCodes.Status200OK);
			resultado.Token.Should().NotBeNullOrEmpty();
			resultado.RefreshToken.Should().NotBeNullOrEmpty();
		}

		private static JwtSecurityToken CreateMockJwtToken()
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, "testuser"),
					new Claim(ClaimTypes.Email, "test@gmail.com")
				}),
				Expires = DateTime.UtcNow.AddMinutes(30),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7b4093b95b3c848a3c3b961da2203c848a09093c")), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
			return token;
		}

		[Fact]
		public async Task DeveRetornar401Unauthorized_QuandoCredenciaisInvalidas()
		{
			// Arrange
			var usuarioModel = new LoginModelDTO
			{
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var usuario = new Usuario
			{
				Email = "test@gmail.com",
				UserName = "testuser",
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			userManager.FindByEmailAsync(usuarioModel.Email).Returns(Task.FromResult<Usuario?>(null));
			userManager.CheckPasswordAsync(usuario, usuarioModel.Password).Returns(Task.FromResult(false));

			var configuration = Substitute.For<IConfiguration>();
			var mockTokenService = Substitute.For<ITokenService>();
			var authService = new AuthService(userManager, configuration, mockTokenService);

			// Act
			var resultado = await authService.LoginAsync(usuarioModel) as ResponseDTO;

			// Assert
			resultado.Should().NotBeNull();
			resultado.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Email ou senha incorretos.");
		}

		[Fact]
		public async Task DeveRetornarErro500_EmFalha_AoAtualizarRefreshToken()
		{
			// Arrange
			var usuarioModel = new LoginModelDTO
			{
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var usuario = new Usuario
			{
				Email = "test@gmail.com",
				UserName = "testuser",
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			userManager.FindByEmailAsync(usuarioModel.Email).Returns(Task.FromResult<Usuario?>(usuario));
			userManager.CheckPasswordAsync(usuario, usuarioModel.Password).Returns(Task.FromResult(true));
			userManager.UpdateAsync(usuario).Returns(Task.FromResult(IdentityResult.Failed()));

			var configuration = Substitute.For<IConfiguration>();
			configuration["JWT:RefreshTokenValidityInMinutes"].Returns("60");

			var mockTokenService = Substitute.For<ITokenService>();
			var authService = new AuthService(userManager, configuration, mockTokenService);

			// Act
			var resultado = await authService.LoginAsync(usuarioModel) as ResponseDTO;

			// Assert
			resultado.Should().NotBeNull();
			resultado.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Falha ao atualizar refresh token do usuário.");
		}
	}
}
