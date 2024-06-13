using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Request;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.API.Models;
using Taskfy.API.Services.Tarefas;
using Taskfy.Tests.Unit.Tarefas.Services.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Services;

public class CriaTarefaServiceTests : BaseServiceSetup
{
	[Fact]
	public async Task DeveRetornar_201Created_QuandoDadosValidos()
	{
		// Arrange
		var tarefaRequestDto = new TarefaRequestDTO
		{
			Titulo = "Nova Tarefa",
			Descricao = "Descrição da nova tarefa",
			Data_vencimento = DateTime.Now.AddDays(1)
		};

		var userId = Guid.NewGuid().ToString();
		var claims = new[]
		{
			new Claim("UserId", userId)
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		var tarefa = new Tarefa
		{
			Id = Guid.NewGuid(),
			Titulo = tarefaRequestDto.Titulo,
			Descricao = tarefaRequestDto.Descricao,
			Data_vencimento = tarefaRequestDto.Data_vencimento,
			Usuario = null,
			Usuario_id = userId,
			Status = false
		};

		var tarefaResponseDto = new TarefaDTO
		{
			Id = tarefa.Id,
			Titulo = tarefa.Titulo,
			Descricao = tarefa.Descricao,
			Data_vencimento = tarefa.Data_vencimento,
			Status = tarefa.Status,
			Usuario_id = tarefa.Usuario_id
		};

		MapperMock.Map<Tarefa>(tarefaRequestDto).Returns(tarefa);
		TarefaRepositoryMock.Create(tarefa).Returns(tarefa);
		UnitOfWorkMock.CommitAsync().Returns(Task.CompletedTask);
		MapperMock.Map<TarefaDTO>(tarefa).Returns(tarefaResponseDto);

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.CriaTarefaAsync(tarefaRequestDto, claimsPrincipal) as TarefaResponseDTO;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status201Created);
		resultado?.Data.Should().BeEquivalentTo(tarefaResponseDto);

		await UnitOfWorkMock.Received(1).CommitAsync();
		LoggerMock.Received(1).LogToFile("Tarefa", "Tarefa criada com sucesso!");
	}

	[Fact]
	public async Task DeveRetornar_400BadRequest_QuandoDadosTarefaInvalidos()
	{
		// Arrange
		TarefaRequestDTO? tarefaRequestDto = null;

		var userId = Guid.NewGuid().ToString();
		var claims = new[]
		{
			new Claim("UserId", userId)
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.CriaTarefaAsync(tarefaRequestDto!, claimsPrincipal);

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Dados de tarefa inválidos.");
	}


}
