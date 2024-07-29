using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.Tests.Unit.Auth.Controllers.BaseMocks;
using Taskfy.Tests.Unit.ServicesMocks;

namespace Taskfy.Tests.Unit.Auth.Controllers;

public class LoginControllerTests : BaseAuthControllerSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_QuandoLoginComSucesso()
	{
		// Arrange
		var loginRequestDto = MocksData.User.GetLoginRequestDTO();

		var responseToken = new ResponseLoginTokenDTO
		{
			StatusCode = StatusCodes.Status200OK,
			Token = "mocked.jwt.token",
			RefreshToken = "mocked.refresh.token",
			Expiration = DateTime.UtcNow.AddMinutes(30)
		};

		AuthServiceMock.LoginAsync(loginRequestDto).Returns(Task.FromResult(responseToken as ResponseDTO));

		// Act
		var resultado = await AuthControllerMock.Login(loginRequestDto) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Value.Should().BeEquivalentTo(responseToken);
	}

	[Fact]
	public async Task DeveRetornar_401Unauthorized_QuandoCredenciaisInvalidas()
	{
		// Arrange
		var loginRequestDto = MocksData.User.GetLoginRequestDTO();

		var response = new ResponseDTO
		{
			Status = "Erro",
			Message = "Email ou senha incorretos.",
			StatusCode = StatusCodes.Status401Unauthorized
		};

		AuthServiceMock.LoginAsync(loginRequestDto).Returns(Task.FromResult(response));

		// Act
		var resultado = await AuthControllerMock.Login(loginRequestDto) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
		resultado?.Value.Should().BeEquivalentTo(response);
	}

	[Fact]
	public async Task DeveRetornar_Erro500_QuandoFalhaAoAtualizarRefreshToken()
	{
		// Arrange
		var loginRequestDto = MocksData.User.GetLoginRequestDTO();

		var response = new ResponseDTO
		{
			Status = "Erro",
			Message = "Falha ao atualizar refresh token do usuário.",
			StatusCode = StatusCodes.Status500InternalServerError
		};

		AuthServiceMock.LoginAsync(loginRequestDto).Returns(Task.FromResult(response));

		// Act
		var resultado = await AuthControllerMock.Login(loginRequestDto) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		resultado?.Value.Should().BeEquivalentTo(response);
	}
}
