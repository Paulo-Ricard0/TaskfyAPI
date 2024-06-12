using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Logs;
using Taskfy.API.Models;
using Taskfy.API.Services.Auth;

namespace Taskfy.Tests.Unit.Auth.Services
{
	public class RegisterServiceTests
	{
		[Fact]
		public async Task QuandoUsuarioNaoExiste_CriaUsuarioComSucesso201Created()
		{
			// Arrange
			var usuarioModel = new RegistroModelDTO
			{
				Name = "testuser test",
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			userManager.FindByEmailAsync(usuarioModel.Email).Returns(Task.FromResult<Usuario?>(null));
			userManager.CreateAsync(Arg.Any<Usuario>(), usuarioModel.Password).Returns(Task.FromResult(IdentityResult.Success));

			var configuration = Substitute.For<IConfiguration>();
			var mockTokenService = Substitute.For<ITokenService>();
			var mockLogger = Substitute.For<ILog>();

			var authService = new AuthService(userManager, configuration, mockTokenService, mockLogger);

			// Act
			var resultado = await authService.RegisterAsync(usuarioModel);

			// Assert
			resultado.StatusCode.Should().Be(StatusCodes.Status201Created);
			resultado.Status.Should().Be("Sucesso");
			resultado.Message.Should().Be("Usuário criado com sucesso!");
		}

		[Fact]
		public async Task QuandoUsuarioExistir_Retorna_409Conflict()
		{
			// Arrange
			var UsuarioExistente = new Usuario { Email = "test@gmail.com" };

			var usuarioModel = new RegistroModelDTO
			{
				Name = "testuser test",
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			userManager.FindByEmailAsync(usuarioModel.Email).Returns(Task.FromResult<Usuario?>(UsuarioExistente));

			var configuration = Substitute.For<IConfiguration>();
			var mockTokenService = Substitute.For<ITokenService>();
			var mockLogger = Substitute.For<ILog>();

			var authService = new AuthService(userManager, configuration, mockTokenService, mockLogger);

			// Act
			var resultado = await authService.RegisterAsync(usuarioModel);

			// Assert
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Usuário já registrado!");
			resultado.StatusCode.Should().Be(StatusCodes.Status409Conflict);
		}

		[Fact]
		public async Task QuandoFalhaNoRegistro_Retorna_500InternalServerError()
		{
			// Arrange
			var usuarioModel = new RegistroModelDTO
			{
				Name = "testuser test",
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			var identityResult = IdentityResult.Failed(new IdentityError { Description = "Erro na criação do usuário." });
			userManager.CreateAsync(Arg.Any<Usuario>(), usuarioModel.Password).Returns(Task.FromResult(identityResult));

			var configuration = Substitute.For<IConfiguration>();
			var mockTokenService = Substitute.For<ITokenService>();
			var mockLogger = Substitute.For<ILog>();

			var authService = new AuthService(userManager, configuration, mockTokenService, mockLogger);

			// Act
			var resultado = await authService.RegisterAsync(usuarioModel);

			// Assert
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Falha na criação de usuário.");
			resultado.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		}
	}
}
