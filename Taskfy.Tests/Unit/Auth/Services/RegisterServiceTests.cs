using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Taskfy.API.Models;
using Taskfy.Tests.Unit.Auth.Services.Mocks;
using Taskfy.Tests.Unit.ServicesMocks;

namespace Taskfy.Tests.Unit.Auth.Services
{
	public class RegisterServiceTests : BaseUserServiceSetup
	{
		[Fact]
		public async Task DeveRetornar_200OK_QuandoRegistrarUsuario()
		{
			// Arrange
			var registroRequestDTO = MocksData.User.GetRegistroRequestDTO();

			UserManagerMock.FindByEmailAsync(registroRequestDTO.Email).Returns(Task.FromResult<Usuario?>(null));
			UserManagerMock.CreateAsync(Arg.Any<Usuario>(), registroRequestDTO.Password).Returns(Task.FromResult(IdentityResult.Success));

			// Act
			var resultado = await AuthServiceMock.RegisterAsync(registroRequestDTO);

			// Assert
			resultado.StatusCode.Should().Be(StatusCodes.Status201Created);
			resultado.Status.Should().Be("Sucesso");
			resultado.Message.Should().Be("Usuário criado com sucesso!");
		}

		[Fact]
		public async Task DeveRetornar_409Conflict_QuandoUsuarioExistente()
		{
			// Arrange
			var UsuarioExistente = MocksData.User.GetUsuario();

			var registroRequestDTO = MocksData.User.GetRegistroRequestDTO();

			UserManagerMock.FindByEmailAsync(registroRequestDTO.Email).Returns(Task.FromResult<Usuario?>(UsuarioExistente));

			// Act
			var resultado = await AuthServiceMock.RegisterAsync(registroRequestDTO);

			// Assert
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Usuário já registrado!");
			resultado.StatusCode.Should().Be(StatusCodes.Status409Conflict);
		}

		[Fact]
		public async Task DeveRetornar_Erro500_QuandoFalhaNoRegistro()
		{
			// Arrange
			var registroRequestDTO = MocksData.User.GetRegistroRequestDTO();

			var identityResult = IdentityResult.Failed(new IdentityError { Description = "Erro na criação do usuário." });
			UserManagerMock.CreateAsync(Arg.Any<Usuario>(), registroRequestDTO.Password).Returns(Task.FromResult(identityResult));

			// Act
			var resultado = await AuthServiceMock.RegisterAsync(registroRequestDTO);

			// Assert
			resultado.Status.Should().Be("Erro");
			resultado.Message.Should().Be("Falha na criação de usuário.");
			resultado.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		}
	}
}
