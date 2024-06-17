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

public class CriaTarefaControllerTests : BaseControllerSetup
{
	[Fact]
	public async Task DeveRetornar_201Created_AoCriarTarefa()
	{
		var userId = Guid.NewGuid().ToString();

		var tarefaRequestDto = new TarefaRequestDTO
		{
			Titulo = "Nova Tarefa",
			Descricao = "Descrição da nova tarefa",
			Data_vencimento = DateTime.Now.AddDays(1)
		};

		var tarefaResponseDto = new TarefaDTO
		{
			Id = Guid.NewGuid(),
			Titulo = tarefaRequestDto.Titulo,
			Descricao = tarefaRequestDto.Descricao,
			Data_vencimento = tarefaRequestDto.Data_vencimento,
			Status = false,
			Usuario_id = userId
		};

		var responseTarefaCriada = new TarefaResponseDTO<TarefaDTO>
		{
			Data = tarefaResponseDto,
			StatusCode = StatusCodes.Status201Created,
		};

		TarefaServiceMock.CriaTarefaAsync(tarefaRequestDto, Arg.Any<ClaimsPrincipal>())
			.Returns(Task.FromResult(responseTarefaCriada as ResponseDTO));

		var controller = new TarefaController(TarefaServiceMock);

		// Act
		var resultado = await controller.CriaTarefa(tarefaRequestDto) as ObjectResult;

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

		var controller = new TarefaController(TarefaServiceMock);

		// Act
		var resultado = await controller.CriaTarefa(tarefaRequestDto!) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaInvalida);
	}

	[Fact]
	public async Task DeveRetornar_401Unauthorized_QuandoUserIdInvalido()
	{
		var tarefaRequestDto = new TarefaRequestDTO
		{
			Titulo = "Nova Tarefa",
			Descricao = "Descrição da nova tarefa",
			Data_vencimento = DateTime.Now.AddDays(1)
		};

		var responseTarefaUnauthorized = new ResponseDTO
		{
			Status = "Erro",
			Message = "Usuário não autorizado.",
			StatusCode = StatusCodes.Status401Unauthorized,
		};

		TarefaServiceMock.CriaTarefaAsync(tarefaRequestDto, Arg.Any<ClaimsPrincipal>())
			.Returns(Task.FromResult(responseTarefaUnauthorized));

		var controller = new TarefaController(TarefaServiceMock);

		// Act
		var resultado = await controller.CriaTarefa(tarefaRequestDto) as ObjectResult;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
		resultado?.Value.Should().BeEquivalentTo(responseTarefaUnauthorized);
	}
}
