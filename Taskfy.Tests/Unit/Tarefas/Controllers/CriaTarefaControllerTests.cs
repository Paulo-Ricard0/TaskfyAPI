using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Request;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.Tests.Unit.ServicesMocks;
using Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Controllers;

public class CriaTarefaControllerTests : BaseTarefaControllerSetup
{
	[Fact]
	public async Task DeveRetornar_201Created_AoCriarTarefa()
	{
		var userId = MocksData.User.GetUserId();

		var tarefaRequestDto = MocksData.Tarefa.GetTarefaRequestDTO();

		var tarefa = MocksData.Tarefa.CreateTarefa(userId, tarefaRequestDto);

		var tarefaResponseDTO = MocksData.Tarefa.GetTarefaDTO(tarefa);

		var responseTarefaCriada = new TarefaResponseDTO<TarefaDTO>
		{
			Data = tarefaResponseDTO,
			StatusCode = StatusCodes.Status201Created,
		};

		TarefaServiceMock.CriaTarefaAsync(tarefaRequestDto, Arg.Any<ClaimsPrincipal>())
			.Returns(Task.FromResult(responseTarefaCriada as ResponseDTO));

		// Act
		var resultado = await TarefaControllerMock.CriaTarefa(tarefaRequestDto) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status201Created);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaCriada);
	}

	[Fact]
	public async Task DeveRetornar_400BadRequest_QuandoDadosTarefaInvalidos()
	{
		TarefaRequestDTO? tarefaRequestDto = null;

		var responseTarefaInvalida = new ResponseDTO
		{
			Status = "Erro",
			Message = "Dados de tarefa inválidos.",
			StatusCode = StatusCodes.Status400BadRequest,
		};

		TarefaServiceMock.CriaTarefaAsync(tarefaRequestDto!, Arg.Any<ClaimsPrincipal>())
			.Returns(Task.FromResult(responseTarefaInvalida));

		// Act
		var resultado = await TarefaControllerMock.CriaTarefa(tarefaRequestDto!) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaInvalida);
	}

	[Fact]
	public async Task DeveRetornar_401Unauthorized_QuandoUserIdInvalido()
	{
		var tarefaRequestDto = MocksData.Tarefa.GetTarefaRequestDTO();

		var responseTarefaUnauthorized = new ResponseDTO
		{
			Status = "Erro",
			Message = "Usuário não autorizado.",
			StatusCode = StatusCodes.Status401Unauthorized,
		};

		TarefaServiceMock.CriaTarefaAsync(tarefaRequestDto, Arg.Any<ClaimsPrincipal>())
			.Returns(Task.FromResult(responseTarefaUnauthorized));

		// Act
		var resultado = await TarefaControllerMock.CriaTarefa(tarefaRequestDto) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaUnauthorized);
	}
}
