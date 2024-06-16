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

public class AtualizaTarefaServiceTests : BaseServiceSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_QuandoAtualizarTarefa()
	{
		// Arrange
		var userId = Guid.NewGuid().ToString();
		var tarefaId = Guid.NewGuid();

		var claims = new[]
		{
			new Claim("UserId", userId)
		};

		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		var tarefaRequestUpdate = new TarefaRequestUpdateDTO
		{
			Titulo = "Tarefa Atualizada",
			Descricao = "Descrição atualizada",
			Data_vencimento = DateTime.Now.AddDays(3),
			Status = true
		};

		var tarefaExistente = new Tarefa
		{
			Id = tarefaId,
			Titulo = "Tarefa Existente",
			Descricao = "Descrição da tarefa existente",
			Data_vencimento = DateTime.Now.AddDays(2),
			Status = false,
			Usuario_id = userId
		};

		var tarefaAtualizada = new Tarefa
		{
			Id = tarefaId,
			Titulo = tarefaRequestUpdate.Titulo,
			Descricao = tarefaRequestUpdate.Descricao,
			Data_vencimento = tarefaRequestUpdate.Data_vencimento,
			Status = tarefaRequestUpdate.Status,
			Usuario = null,
			Usuario_id = userId
		};

		var tarefaResponseDTO = new TarefaDTO
		{
			Id = tarefaAtualizada.Id,
			Titulo = tarefaAtualizada.Titulo,
			Descricao = tarefaAtualizada.Descricao,
			Data_vencimento = tarefaAtualizada.Data_vencimento,
			Status = tarefaAtualizada.Status,
			Usuario_id = tarefaAtualizada.Usuario_id
		};

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(tarefaExistente);
		UnitOfWorkMock.TarefaRepository.Update(tarefaExistente).Returns(tarefaAtualizada);
		UnitOfWorkMock.CommitAsync().Returns(Task.CompletedTask);
		MapperMock.Map<TarefaDTO>(tarefaAtualizada).Returns(tarefaResponseDTO);

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.AtualizaTarefa(claimsPrincipal, tarefaId, tarefaRequestUpdate) as TarefaResponseDTO<TarefaDTO>;

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
		var userId = Guid.NewGuid().ToString();
		var tarefaId = Guid.NewGuid();

		var claims = new[]
		{
			new Claim("UserId", userId)
		};

		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		var tarefaRequestUpdate = new TarefaRequestUpdateDTO
		{
			Titulo = "Tarefa Atualizada",
			Descricao = "Descrição atualizada",
			Data_vencimento = DateTime.Now.AddDays(3),
			Status = true
		};

		UnitOfWorkMock.TarefaRepository.FindAsync(tarefaId).Returns(Task.FromResult<Tarefa?>(null));

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.AtualizaTarefa(claimsPrincipal, tarefaId, tarefaRequestUpdate);

		// Assert
		resultado.Should().NotBeNull();
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Tarefa não encontrada.");
		resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
	}
}
