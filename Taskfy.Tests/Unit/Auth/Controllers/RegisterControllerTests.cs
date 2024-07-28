using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Taskfy.API.DTOs;
using Taskfy.Tests.Unit.Auth.Controllers.BaseMocks;
using Taskfy.Tests.Unit.ServicesMocks;

namespace Taskfy.Tests.Unit.Auth.Controllers;

public class RegisterControllerTests : BaseAuthControllerSetup
{
	[Fact]
	public async Task DeveRetornar_201Created_QuandoUsuarioCriado()
	{
		// Arrange
		var registroRequestDto = MocksData.User.GetRegistroRequestDTO();

		var response = new ResponseDTO
		{
			Status = "Sucesso",
			Message = "Usuário criado com sucesso!",
			StatusCode = StatusCodes.Status201Created
		};

		AuthServiceMock.RegisterAsync(registroRequestDto).Returns(Task.FromResult(response));

		// Act
		var resultado = await AuthControllerMock.Register(registroRequestDto) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status201Created);
		resultado?.Value.Should().BeEquivalentTo(response);
	}

	[Fact]
	public async Task DeveRetornar_409Conflict_QuandoUsuarioExistir()
	{
		// Arrange
		var registroRequestDto = MocksData.User.GetRegistroRequestDTO();

		var response = new ResponseDTO
		{
			Status = "Erro",
			Message = "Usuário já registrado!",
			StatusCode = StatusCodes.Status409Conflict
		};

		AuthServiceMock.RegisterAsync(registroRequestDto).Returns(Task.FromResult(response));

		// Act
		var resultado = await AuthControllerMock.Register(registroRequestDto) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status409Conflict);
		resultado?.Value.Should().BeEquivalentTo(response);
	}

	[Fact]
	public async Task DeveRetornar_Erro500_QuandoErroInternoOcorrer()
	{
		// Arrange
		var registroRequestDto = MocksData.User.GetRegistroRequestDTO();

		var response = new ResponseDTO
		{
			Status = "Erro",
			Message = "Falha na criação de usuário.",
			StatusCode = StatusCodes.Status500InternalServerError
		};

		AuthServiceMock.RegisterAsync(registroRequestDto).Returns(Task.FromResult(response));

		// Act
		var resultado = await AuthControllerMock.Register(registroRequestDto) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		resultado?.Value.Should().BeEquivalentTo(response);
	}
}
