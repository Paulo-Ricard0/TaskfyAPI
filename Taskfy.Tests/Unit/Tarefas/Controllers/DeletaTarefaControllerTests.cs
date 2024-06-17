using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.Controllers;
using Taskfy.API.DTOs;
using Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Controllers;

public class DeletaTarefaControllerTests : BaseControllerSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_AoDeletarTarefa()
	{
		// Arrange
		var tarefaId = Guid.NewGuid();

		var responseTarefaDeletada = new ResponseDTO
		{
			Status = "Sucesso",
			Message = "Tarefa deletada com sucesso.",
			StatusCode = StatusCodes.Status200OK,
		};

		TarefaServiceMock.DeletaTarefa(Arg.Any<ClaimsPrincipal>(), tarefaId)
			.Returns(Task.FromResult(responseTarefaDeletada));

		var controller = new TarefaController(TarefaServiceMock);

		// Act
		var resultado = await controller.DeletaTarefa(tarefaId) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaDeletada);
	}
}
