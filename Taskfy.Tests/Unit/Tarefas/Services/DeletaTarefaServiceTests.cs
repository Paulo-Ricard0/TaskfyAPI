﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Taskfy.API.Models;
using Taskfy.Tests.Unit.ServicesMocks;
using Taskfy.Tests.Unit.Tarefas.Services.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Services;

public class DeletaTarefaServiceTests : BaseTarefaServiceSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_QuandoDeletarTarefa()
	{
		// Arrange
		var userId = MocksData.User.GetUserId();
		var tarefaId = MocksData.Tarefa.GetTarefaId();

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		var tarefaExistente = MocksData.Tarefa.GetTarefa(tarefaId, userId);

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(tarefaExistente);
		UnitOfWorkMock.TarefaRepository.Delete(tarefaExistente).Returns(tarefaExistente);
		UnitOfWorkMock.CommitAsync().Returns(Task.CompletedTask);

		// Act
		var resultado = await TarefaServiceMock.DeletaTarefa(claimsPrincipal, tarefaId);

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
		var userId = MocksData.User.GetUserId();
		var tarefaId = MocksData.Tarefa.GetTarefaId();

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(Task.FromResult<Tarefa?>(null));

		// Act
		var resultado = await TarefaServiceMock.DeletaTarefa(claimsPrincipal, tarefaId);

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
		var userId = MocksData.User.GetUserId();
		var tarefaId = MocksData.Tarefa.GetTarefaId();

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		var tarefaExistente = MocksData.Tarefa.GetTarefa(tarefaId, Guid.NewGuid().ToString());

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(tarefaExistente);

		// Act
		var resultado = await TarefaServiceMock.DeletaTarefa(claimsPrincipal, tarefaId);

		// Assert
		resultado.Should().NotBeNull();
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Você não tem permissão para deletar essa tarefa.");
		resultado?.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
	}
}
