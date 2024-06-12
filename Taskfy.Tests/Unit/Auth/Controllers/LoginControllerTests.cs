using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Taskfy.API.Controllers;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Services.Auth;

namespace Taskfy.Tests.Unit.Auth.Controllers;

public class LoginControllerTests
{
	[Fact]
	public async Task Verifica_LoginComSucesso_Retorna201CreatedStatusCode()
	{
		// Arrange
		var usuarioModel = new LoginModelDTO
		{
			Email = "test@gmail.com",
			Password = "Test123@"
		};

		var responseToken = new ResponseLoginTokenDTO
		{
			StatusCode = StatusCodes.Status200OK,
			Token = "mocked.jwt.token",
			RefreshToken = "mocked.refresh.token",
			Expiration = DateTime.UtcNow.AddMinutes(30)
		};

		var authService = Substitute.For<IAuthService>();
		authService.LoginAsync(usuarioModel).Returns(Task.FromResult(responseToken as ResponseDTO));

		var controller = new AuthController(authService);

		// Act
		var resultado = await controller.Login(usuarioModel) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Value.Should().BeEquivalentTo(responseToken);
	}

	[Fact]
	public async Task Verifica_401Unauthorized_QuandoCredenciaisInvalidas()
	{
		// Arrange
		var usuarioModel = new LoginModelDTO
		{
			Email = "test@gmail.com",
			Password = "Test123@"
		};

		var response = new ResponseDTO
		{
			Status = "Erro",
			Message = "Email ou senha incorretos.",
			StatusCode = StatusCodes.Status401Unauthorized
		};

		var authService = Substitute.For<IAuthService>();
		authService.LoginAsync(usuarioModel).Returns(Task.FromResult(response));

		var controller = new AuthController(authService);

		// Act
		var resultado = await controller.Login(usuarioModel) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
		resultado?.Value.Should().BeEquivalentTo(response);
	}
	}
}
