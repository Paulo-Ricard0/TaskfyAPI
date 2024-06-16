using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.Controllers;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Controllers;

public class BuscaTarefaPorIdControllerTests : BaseControllerSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_AoBuscarTarefaPeloId()
	{
		// Arrange
		var userId = Guid.NewGuid().ToString();
		var tarefaId = Guid.NewGuid();

		var tarefaResponseDTO = new TarefaDTO
		{
			Id = tarefaId,
			Titulo = "Tarefa",
			Descricao = "Descrição da tarefa",
			Data_vencimento = DateTime.Now.AddDays(1),
			Status = false,
			Usuario_id = userId
		};

		var responseTarefaEncontrada = new TarefaResponseDTO<TarefaDTO>
		{
			Data = tarefaResponseDTO,
			StatusCode = StatusCodes.Status200OK,
		};

		TarefaServiceMock.BuscaTarefaPorIdAsync(Arg.Any<ClaimsPrincipal>(), tarefaId)
		.Returns(Task.FromResult(responseTarefaEncontrada as ResponseDTO));

		var controller = new TarefaController(TarefaServiceMock);

		// Act
		var resultado = await controller.BuscaTarefaPorId(tarefaId) as ObjectResult;

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

		var tarefaId = Guid.NewGuid();

		TarefaServiceMock.BuscaTarefaPorIdAsync(Arg.Any<ClaimsPrincipal>(), tarefaId)
		.Returns(Task.FromResult(responseTarefaUnauthorized));

		var controller = new TarefaController(TarefaServiceMock);

		// Act
		var resultado = await controller.BuscaTarefaPorId(tarefaId) as ObjectResult;

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

		var tarefaId = Guid.NewGuid();

		TarefaServiceMock.BuscaTarefaPorIdAsync(Arg.Any<ClaimsPrincipal>(), tarefaId)
		.Returns(Task.FromResult(responseTarefaNotFound));

		var controller = new TarefaController(TarefaServiceMock);

		// Act
		var resultado = await controller.BuscaTarefaPorId(tarefaId) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaNotFound);
	}
}
