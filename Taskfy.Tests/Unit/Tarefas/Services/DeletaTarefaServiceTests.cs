using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.Models;
using Taskfy.API.Services.Tarefas;
using Taskfy.Tests.Unit.Tarefas.Services.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Services;

public class DeletaTarefaServiceTests : BaseServiceSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_QuandoDeletarTarefa()
	{
		// Arrange
		var userId = Guid.NewGuid().ToString();
		var tarefaId = Guid.NewGuid();

		var claims = new[]
		{
			new Claim("UserId", userId)
		};

		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		var tarefaExistente = new Tarefa
		{
			Id = tarefaId,
			Titulo = "Tarefa Existente",
			Descricao = "Descrição da tarefa existente",
			Data_vencimento = DateTime.Now.AddDays(2),
			Usuario = null,
			Status = false,
			Usuario_id = userId
		};

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(tarefaExistente);
		UnitOfWorkMock.TarefaRepository.Delete(tarefaExistente).Returns(tarefaExistente);
		UnitOfWorkMock.CommitAsync().Returns(Task.CompletedTask);

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.DeletaTarefa(claimsPrincipal, tarefaId);

		// Assert
		resultado.Should().NotBeNull();
		resultado?.Status.Should().Be("Sucesso");
		resultado?.Message.Should().Be("Tarefa deletada com sucesso.");
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);

		await UnitOfWorkMock.Received(1).CommitAsync();
		UnitOfWorkMock.TarefaRepository.Received(1).Delete(tarefaExistente);
	}

	[Fact]
	public async Task DeveRetornar_404NotFound_QuandoTarefaNaoEncontrada()
	{
		// Arrange
		var userId = Guid.NewGuid().ToString();
		var tarefaId = Guid.NewGuid();

		var claims = new[]
		{
			new Claim("UserId", userId)
		};

		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(Task.FromResult<Tarefa?>(null));

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.DeletaTarefa(claimsPrincipal, tarefaId);

		// Assert
		resultado.Should().NotBeNull();
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Tarefa não encontrada.");
		resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
	}

	[Fact]
	public async Task DeveRetornar_403Forbidden_QuandoUserIdInvalido()
	{
		// Arrange
		var userId = Guid.NewGuid().ToString();
		var tarefaId = Guid.NewGuid();

		var claims = new[]
		{
			new Claim("UserId", userId)
		};

		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		var tarefaExistente = new Tarefa
		{
			Id = tarefaId,
			Titulo = "Tarefa Existente",
			Descricao = "Descrição da tarefa existente",
			Data_vencimento = DateTime.Now.AddDays(2),
			Usuario = null,
			Status = false,
			Usuario_id = Guid.NewGuid().ToString()
		};

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(tarefaExistente);

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.DeletaTarefa(claimsPrincipal, tarefaId);

		// Assert
		resultado.Should().NotBeNull();
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Você não tem permissão para deletar essa tarefa.");
		resultado?.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
	}
}
