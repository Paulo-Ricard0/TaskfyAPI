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

public class AtualizaTarefaControllerTests : BaseTarefaControllerSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_AoAtualizarTarefa()
	{
		// Arrange
		var tarefaId = MocksData.Tarefa.GetTarefaId();
		var userId = MocksData.User.GetUserId();

		var tarefaRequestUpdate = MocksData.Tarefa.GetTarefaRequestUpdateDTO();

		var tarefa = MocksData.Tarefa.GetTarefa(tarefaId, userId);

		var tarefaResponseDto = MocksData.Tarefa.GetTarefaDTO(tarefa);

		var responseTarefaAtualizada = new TarefaResponseDTO<TarefaDTO>
		{
			Data = tarefaResponseDto,
			StatusCode = StatusCodes.Status200OK,
		};

		TarefaServiceMock.AtualizaTarefa(Arg.Any<ClaimsPrincipal>(), tarefaId, tarefaRequestUpdate)
			.Returns(Task.FromResult(responseTarefaAtualizada as ResponseDTO));

		// Act
		var resultado = await TarefaControllerMock.AtualizaTarefa(tarefaId, tarefaRequestUpdate) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaAtualizada);
	}

	[Fact]
	public async Task DeveRetornar_404NotFound_QuandoTarefaNaoEncontrada()
	{
		// Arrange
		var tarefaId = MocksData.Tarefa.GetTarefaId();

		var tarefaRequestUpdate = MocksData.Tarefa.GetTarefaRequestUpdateDTO();

		var responseTarefaNotFound = new ResponseDTO
		{
			Status = "Erro",
			Message = "Tarefa não encontrada.",
			StatusCode = StatusCodes.Status404NotFound,
		};

		TarefaServiceMock.AtualizaTarefa(Arg.Any<ClaimsPrincipal>(), tarefaId, tarefaRequestUpdate)
			.Returns(Task.FromResult(responseTarefaNotFound));

		// Act
		var resultado = await TarefaControllerMock.AtualizaTarefa(tarefaId, tarefaRequestUpdate) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaNotFound);
	}
}
