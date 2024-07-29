using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.API.Models;
using Taskfy.Tests.Unit.ServicesMocks;
using Taskfy.Tests.Unit.Tarefas.Services.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Services;

public class AtualizaTarefaServiceTests : BaseTarefaServiceSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_QuandoAtualizarTarefa()
	{
		// Arrange
		var userId = MocksData.User.GetUserId();
		var tarefaId = MocksData.Tarefa.GetTarefaId();

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		var tarefaRequestUpdate = MocksData.Tarefa.GetTarefaRequestUpdateDTO();

		var tarefaExistente = MocksData.Tarefa.GetTarefa(tarefaId, userId);

		var tarefaAtualizada = MocksData.Tarefa.GetTarefaAtualizada(tarefaId, tarefaRequestUpdate, userId);

		var tarefaResponseDTO = MocksData.Tarefa.GetTarefaDTO(tarefaAtualizada);

		MapperMock.Map<TarefaDTO>(tarefaAtualizada).Returns(tarefaResponseDTO);

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(tarefaExistente);
		UnitOfWorkMock.TarefaRepository.Update(tarefaExistente).Returns(tarefaAtualizada);
		UnitOfWorkMock.CommitAsync().Returns(Task.CompletedTask);

		// Act
		var resultado = await TarefaServiceMock.AtualizaTarefa(claimsPrincipal, tarefaId, tarefaRequestUpdate) as TarefaResponseDTO<TarefaDTO>;

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Data.Should().BeEquivalentTo(tarefaResponseDTO);

		await UnitOfWorkMock.Received(1).CommitAsync();
		UnitOfWorkMock.TarefaRepository.Received(1).Update(tarefaExistente);
	}

	[Fact]
	public async Task DeveRetornar_404NotFound_QuandoTarefaNaoEncontrada()
	{
		// Arrange
		var userId = MocksData.User.GetUserId();
		var tarefaId = MocksData.Tarefa.GetTarefaId();

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		var tarefaRequestUpdate = MocksData.Tarefa.GetTarefaRequestUpdateDTO();

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(Task.FromResult<Tarefa?>(null));

		// Act
		var resultado = await TarefaServiceMock.AtualizaTarefa(claimsPrincipal, tarefaId, tarefaRequestUpdate);

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

		var tarefaRequestUpdate = MocksData.Tarefa.GetTarefaRequestUpdateDTO();

		var tarefaExistente = MocksData.Tarefa.GetTarefa(tarefaId, Guid.NewGuid().ToString());

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(tarefaExistente);

		// Act
		var resultado = await TarefaServiceMock.AtualizaTarefa(claimsPrincipal, tarefaId, tarefaRequestUpdate);

		// Assert
		resultado.Should().NotBeNull();
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Você não tem permissão para atualizar essa tarefa.");
		resultado?.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
	}
}
