using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.Tests.Unit.ServicesMocks;
using Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Controllers;

public class BuscaTarefaPorIdControllerTests : BaseTarefaControllerSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_AoBuscarTarefaPeloId()
	{
		// Arrange
		var userId = MocksData.User.GetUserId();
		var tarefaId = MocksData.Tarefa.GetTarefaId();

		var tarefa = MocksData.Tarefa.GetTarefa(tarefaId, userId);

		var tarefaResponseDto = MocksData.Tarefa.GetTarefaDTO(tarefa);

		var responseTarefaEncontrada = new TarefaResponseDTO<TarefaDTO>
		{
			Data = tarefaResponseDto,
			StatusCode = StatusCodes.Status200OK,
		};

		TarefaServiceMock.BuscaTarefaPorIdAsync(Arg.Any<ClaimsPrincipal>(), tarefaId)
		.Returns(Task.FromResult(responseTarefaEncontrada as ResponseDTO));

		// Act
		var resultado = await TarefaControllerMock.BuscaTarefaPorId(tarefaId) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaEncontrada);
	}

	[Fact]
	public async Task DeveRetornar_401Unauthorized_QuandoUserIdInvalido()
	{
		// Arrange
		var responseTarefaUnauthorized = new ResponseDTO
		{
			Status = "Erro",
			Message = "Usuário não autorizado.",
			StatusCode = StatusCodes.Status401Unauthorized,
		};

		var tarefaId = MocksData.Tarefa.GetTarefaId();

		TarefaServiceMock.BuscaTarefaPorIdAsync(Arg.Any<ClaimsPrincipal>(), tarefaId)
		.Returns(Task.FromResult(responseTarefaUnauthorized));

		// Act
		var resultado = await TarefaControllerMock.BuscaTarefaPorId(tarefaId) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaUnauthorized);
	}

	[Fact]
	public async Task DeveRetornar_404NotFound_QuandoTarefaNaoEncontrada()
	{
		// Arrange
		var responseTarefaNotFound = new ResponseDTO
		{
			Status = "Erro",
			Message = "Tarefa não encontrada.",
			StatusCode = StatusCodes.Status404NotFound,
		};

		var tarefaId = MocksData.Tarefa.GetTarefaId();

		TarefaServiceMock.BuscaTarefaPorIdAsync(Arg.Any<ClaimsPrincipal>(), tarefaId)
		.Returns(Task.FromResult(responseTarefaNotFound));

		// Act
		var resultado = await TarefaControllerMock.BuscaTarefaPorId(tarefaId) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaNotFound);
	}
}
