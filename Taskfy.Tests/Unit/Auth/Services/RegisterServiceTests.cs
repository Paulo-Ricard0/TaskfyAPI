using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Models;
using Taskfy.API.Services.Auth;

namespace Taskfy.Tests.Unit.Auth.Services
{
	public class RegisterServiceTests
	{
		[Fact]
		public async Task Registra_QuandoUsuarioNaoExiste_CriaUsuarioComSucesso()
		{
			// Arrange
			var usuarioModel = new RegistroModelDTO
			{
				UserName = "testuser",
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			userManager.FindByEmailAsync(usuarioModel.Email).Returns(Task.FromResult<Usuario?>(null));
			userManager.CreateAsync(Arg.Any<Usuario>(), Arg.Any<string>()).Returns(Task.FromResult(IdentityResult.Success));

			var configuration = Substitute.For<IConfiguration>();
			var authService = new AuthService(userManager, configuration);

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
				UserName = "testuser",
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			userManager.FindByEmailAsync(usuarioModel.Email).Returns(Task.FromResult<Usuario?>(UsuarioExistente));

			var configuration = Substitute.For<IConfiguration>();
			var authService = new AuthService(userManager, configuration);

			// Act
			var resultado = await authService.RegisterAsync(usuarioModel);

			// Assert
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Usuário já registrado!");
			resultado.StatusCode.Should().Be(StatusCodes.Status409Conflict);
		}

		[Fact]
		public async Task QuandoErroInternoOcorre_Retorna_500InternalServerError()
		{
			// Arrange
			var usuarioModel = new RegistroModelDTO
			{
				UserName = "testuser",
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			var identityResult = IdentityResult.Failed(new IdentityError { Description = "Erro na criação do usuário." });
			userManager.CreateAsync(Arg.Any<Usuario>(), Arg.Any<string>()).Returns(Task.FromResult(identityResult));

			var configuration = Substitute.For<IConfiguration>();
			var authService = new AuthService(userManager, configuration);

			// Act
			var resultado = await authService.RegisterAsync(usuarioModel);

			// Assert
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Falha na criação de usuário.");
			resultado.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		}
	}
}
