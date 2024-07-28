using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.Tests.Unit.ServicesMocks;
using Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Controllers;

public class DeletaTarefaControllerTests : BaseTarefaControllerSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_AoDeletarTarefa()
	{
		// Arrange
		var tarefaId = MocksData.Tarefa.GetTarefaId();

		var responseTarefaDeletada = new ResponseDTO
		{
			Status = "Sucesso",
			Message = "Tarefa deletada com sucesso.",
			StatusCode = StatusCodes.Status200OK,
		};

		TarefaServiceMock.DeletaTarefa(Arg.Any<ClaimsPrincipal>(), tarefaId)
			.Returns(Task.FromResult(responseTarefaDeletada));

		// Act
		var resultado = await TarefaControllerMock.DeletaTarefa(tarefaId) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaDeletada);
	}

	[Fact]
	public async Task DeveRetornar_404NotFound_QuandoTarefaNaoEncontrada()
	{
		// Arrange
		var tarefaId = MocksData.Tarefa.GetTarefaId();

		var responseTarefaNotFound = new ResponseDTO
		{
			Status = "Erro",
			Message = "Tarefa não encontrada.",
			StatusCode = StatusCodes.Status404NotFound,
		};

		TarefaServiceMock.DeletaTarefa(Arg.Any<ClaimsPrincipal>(), tarefaId)
			.Returns(Task.FromResult(responseTarefaNotFound));

		// Act
		var resultado = await TarefaControllerMock.DeletaTarefa(tarefaId) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaNotFound);
	}
}
