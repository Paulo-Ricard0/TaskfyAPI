using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Linq.Expressions;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.API.Models;
using Taskfy.Tests.Unit.ServicesMocks;
using Taskfy.Tests.Unit.Tarefas.Services.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Services
{
    public class BuscaTarefaPorIdServiceTests : BaseTarefaServiceSetup
	{
		[Fact]
		public async Task DeveRetornar_200OK_QuandoUsuarioPossuirTarefa()
		{
			// Arrange
			var userId = MocksData.User.GetUserId();

			var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

			var tarefaId = MocksData.Tarefa.GetTarefaId();

			var tarefa = MocksData.Tarefa.GetTarefa(tarefaId, userId);

			var tarefaDTO = MocksData.Tarefa.GetTarefaDTO(tarefa);

			UnitOfWorkMock.TarefaRepository.GetAsync(Arg.Any<Expression<Func<Tarefa, bool>>>())
				.Returns(Task.FromResult<Tarefa?>(tarefa));

			MapperMock.Map<TarefaDTO>(tarefa).Returns(tarefaDTO);

			// Act
			var resultado = await TarefaServiceMock.BuscaTarefaPorIdAsync(claimsPrincipal, tarefaId) as TarefaResponseDTO<TarefaDTO>;

			// Assert
			await UnitOfWorkMock.TarefaRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Tarefa, bool>>>());
			resultado.Should().NotBeNull();
			resultado.Should().BeOfType<TarefaResponseDTO<TarefaDTO>>();
			resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
			resultado?.Data.Should().BeEquivalentTo(tarefaDTO);
		}

		[Fact]
		public async Task DeveRetornar_401Unauthorized_QuandoUsuarioIdInvalido()
		{
			// Arrange
			const string? userId = "";

			var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

			var tarefaId = MocksData.Tarefa.GetTarefaId();

			// Act
			var resultado = await TarefaServiceMock.BuscaTarefaPorIdAsync(claimsPrincipal, tarefaId);

			// Assert
			resultado.Should().NotBeNull();
			resultado.Should().BeOfType<ResponseDTO>();
			resultado?.Status.Should().Be("Erro");
			resultado?.Message.Should().Be("Usuário não autorizado.");
			resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
		}

		[Fact]
		public async Task DeveRetornar_404NotFound_QuandoTarefaNaoEncontrada()
		{
			// Arrange
			var userId = MocksData.User.GetUserId();

			var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

			var tarefaId = MocksData.Tarefa.GetTarefaId();

			UnitOfWorkMock.TarefaRepository.GetAsync(Arg.Any<Expression<Func<Tarefa, bool>>>())
				.Returns(Task.FromResult<Tarefa?>(null));

			// Act
			var resultado = await TarefaServiceMock.BuscaTarefaPorIdAsync(claimsPrincipal, tarefaId);

			// Assert
			resultado.Should().NotBeNull();
			resultado.Should().BeOfType<ResponseDTO>();
			resultado?.Status.Should().Be("Erro");
			resultado?.Message.Should().Be("Tarefa não encontrada.");
			resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		}
	}
}
