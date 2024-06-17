using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.Controllers;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Request;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Controllers;

public class AtualizaTarefaControllerTests : BaseControllerSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_AoAtualizarTarefa()
	{
		// Arrange
		var tarefaId = Guid.NewGuid();
		var userId = Guid.NewGuid().ToString();

		var tarefaRequestUpdate = new TarefaRequestUpdateDTO
		{
			Titulo = "Tarefa Atualizada",
			Descricao = "Descrição atualizada",
			Data_vencimento = DateTime.Now.AddDays(3),
			Status = true
		};

		var tarefaResponseDto = new TarefaDTO
		{
			Id = tarefaId,
			Titulo = tarefaRequestUpdate.Titulo,
			Descricao = tarefaRequestUpdate.Descricao,
			Data_vencimento = tarefaRequestUpdate.Data_vencimento,
			Status = tarefaRequestUpdate.Status,
			Usuario_id = userId
		};

		var responseTarefaAtualizada = new TarefaResponseDTO<TarefaDTO>
		{
			Data = tarefaResponseDto,
			StatusCode = StatusCodes.Status200OK,
		};

		TarefaServiceMock.AtualizaTarefa(Arg.Any<ClaimsPrincipal>(), tarefaId, tarefaRequestUpdate)
			.Returns(Task.FromResult(responseTarefaAtualizada as ResponseDTO));

		var controller = new TarefaController(TarefaServiceMock);

		// Act
		var resultado = await controller.AtualizaTarefa(tarefaId, tarefaRequestUpdate) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaAtualizada);
	}

}
