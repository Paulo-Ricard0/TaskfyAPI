using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Models;
using Taskfy.Tests.Unit.Auth.Services.Mocks;
using Taskfy.Tests.Unit.ServicesMocks;

namespace Taskfy.Tests.Unit.Auth.Services
{
	public class LoginServiceTests : BaseUserServiceSetup
	{
		[Fact]
		public async Task DeveRetornar_200OK_QuandoUsuarioEsenhaCorretos()
		{
			// Arrange
			var loginRequestModel = MocksData.User.GetLoginRequestDTO();

			var usuario = MocksData.User.GetUsuario();

			UserManagerMock.FindByEmailAsync(loginRequestModel.Email).Returns(Task.FromResult<Usuario?>(usuario));
			UserManagerMock.CheckPasswordAsync(usuario, loginRequestModel.Password).Returns(Task.FromResult(true));
			UserManagerMock.GetRolesAsync(usuario).Returns(Task.FromResult<IList<string>>([]));
			UserManagerMock.UpdateAsync(usuario).Returns(Task.FromResult(IdentityResult.Success));

			ConfigurationMock["JWT:SecretKey"].Returns("7b4093b95b3c848b3d3b961da2203c848a09093d");
			ConfigurationMock["JWT:ValidAudience"].Returns("http://localhost:7066");
			ConfigurationMock["JWT:ValidIssuer"].Returns("http://localhost:5066");
			ConfigurationMock["JWT:TokenValidityInMinutes"].Returns("30");
			ConfigurationMock["JWT:RefreshTokenValidityInMinutes"].Returns("60");

			TokenServiceMock.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>(), Arg.Any<IConfiguration>())
				.Returns(MocksData.Auth.GetJwtToken(usuario.Id));

			// Act
			var resultado = await AuthServiceMock.LoginAsync(loginRequestModel) as ResponseLoginTokenDTO;

			// Assert
			resultado.Should().NotBeNull();
			resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
			resultado?.Token.Should().NotBeNullOrEmpty();
			resultado?.RefreshToken.Should().NotBeNullOrEmpty();

			const int maxTokenValidityInMinutes = 35;
			var maxExpiration = DateTime.UtcNow.AddMinutes(maxTokenValidityInMinutes);
			resultado?.Expiration.Should().BeAfter(DateTime.UtcNow).And.BeBefore(maxExpiration);
		}

		[Fact]
		public async Task DeveRetornar_401Unauthorized_QuandoCredenciaisInvalidas()
		{
			// Arrange
			var loginRequestModel = MocksData.User.GetLoginRequestDTO();

			var usuario = MocksData.User.GetUsuario();

			UserManagerMock.FindByEmailAsync(loginRequestModel.Email).Returns(Task.FromResult<Usuario?>(null));
			UserManagerMock.CheckPasswordAsync(usuario, loginRequestModel.Password).Returns(Task.FromResult(false));

			// Act
			var resultado = await AuthServiceMock.LoginAsync(loginRequestModel) as ResponseDTO;

			// Assert
			resultado.Should().NotBeNull();
			resultado.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Email ou senha incorretos.");
		}

		[Fact]
		public async Task DeveRetornar_Erro500_EmFalha_AoAtualizarRefreshToken()
		{
			// Arrange
			var loginRequestModel = MocksData.User.GetLoginRequestDTO();

			var usuario = MocksData.User.GetUsuario();

			UserManagerMock.FindByEmailAsync(loginRequestModel.Email).Returns(Task.FromResult<Usuario?>(usuario));
			UserManagerMock.CheckPasswordAsync(usuario, loginRequestModel.Password).Returns(Task.FromResult(true));
			UserManagerMock.UpdateAsync(usuario).Returns(Task.FromResult(IdentityResult.Failed()));

			ConfigurationMock["JWT:RefreshTokenValidityInMinutes"].Returns("60");

			// Act
			var resultado = await AuthServiceMock.LoginAsync(loginRequestModel) as ResponseDTO;

			// Assert
			resultado.Should().NotBeNull();
			resultado.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Falha ao atualizar refresh token do usuário.");
		}

		[Fact]
		public async Task DeveRetornar_Erro500_EmFalha_AoParsearRefreshTokenValidityInMinutes()
		{
			// Arrange
			var loginRequestModel = MocksData.User.GetLoginRequestDTO();

			var usuario = MocksData.User.GetUsuario();

			UserManagerMock.FindByEmailAsync(loginRequestModel.Email).Returns(Task.FromResult<Usuario?>(usuario));
			UserManagerMock.CheckPasswordAsync(usuario, loginRequestModel.Password).Returns(Task.FromResult(true));

			ConfigurationMock["JWT:RefreshTokenValidityInMinutes"].Returns("invalid_value");

			// Act
			var resultado = await AuthServiceMock.LoginAsync(loginRequestModel) as ResponseDTO;

			// Assert
			resultado.Should().NotBeNull();
			resultado.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Período de validade do refresh token inválido.");
		}
	}
}
