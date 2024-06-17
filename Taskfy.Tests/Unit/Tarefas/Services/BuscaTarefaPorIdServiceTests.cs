using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Linq.Expressions;
using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.API.Models;
using Taskfy.API.Services.Tarefas;
using Taskfy.Tests.Unit.Tarefas.Services.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Services
{
	public class BuscaTarefaPorIdServiceTests : BaseServiceSetup
	{
		[Fact]
		public async Task DeveRetornar_200OK_QuandoUsuarioPossuirTarefa()
		{
			// Arrange
			var userId = Guid.NewGuid().ToString();
			var claims = new[]
			{
				new Claim("UserId", userId)
			};
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

			var tarefaId = Guid.NewGuid();

			var tarefa = new Tarefa
			{
				Id = tarefaId,
				Titulo = "Tarefa",
				Descricao = "Descrição da tarefa",
				Data_vencimento = DateTime.Now.AddDays(1),
				Usuario = null,
				Usuario_id = userId,
				Status = false
			};

			var tarefaDTO = new TarefaDTO
			{
				Id = tarefa.Id,
				Titulo = tarefa.Titulo,
				Descricao = tarefa.Descricao,
				Data_vencimento = tarefa.Data_vencimento,
				Status = tarefa.Status,
				Usuario_id = tarefa.Usuario_id
			};

			TarefaRepositoryMock.GetAsync(Arg.Any<Expression<Func<Tarefa, bool>>>())
				.Returns(Task.FromResult<Tarefa?>(tarefa));

			MapperMock.Map<TarefaDTO>(tarefa).Returns(tarefaDTO);

			var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

			// Act
			var resultado = await tarefaService.BuscaTarefaPorIdAsync(claimsPrincipal, tarefaId) as TarefaResponseDTO<TarefaDTO>;

			// Assert
			await TarefaRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Tarefa, bool>>>());
			resultado.Should().NotBeNull();
			resultado.Should().BeOfType<TarefaResponseDTO<TarefaDTO>>();
			resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
			resultado?.Data.Should().BeEquivalentTo(tarefaDTO);
		}

		[Fact]
		public async Task DeveRetornar_401Unauthorized_QuandoUsuarioIdInvalido()
		{
			// Arrange
			string? userId = "";
			var claims = new[]
			{
				new Claim("UserId", userId)
			};
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

			var tarefaId = Guid.NewGuid();

			var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

			// Act
			var resultado = await tarefaService.BuscaTarefaPorIdAsync(claimsPrincipal, tarefaId);

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
			var userId = Guid.NewGuid().ToString();
			var claims = new[]
			{
				new Claim("UserId", userId)
			};
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

			var tarefaId = Guid.NewGuid();

			TarefaRepositoryMock.GetAsync(Arg.Any<Expression<Func<Tarefa, bool>>>())
				.Returns(Task.FromResult<Tarefa?>(null));

			var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

			// Act
			var resultado = await tarefaService.BuscaTarefaPorIdAsync(claimsPrincipal, tarefaId);

			// Assert
			resultado.Should().NotBeNull();
			resultado.Should().BeOfType<ResponseDTO>();
			resultado?.Status.Should().Be("Erro");
			resultado?.Message.Should().Be("Tarefa não encontrada.");
			resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		}
	}
}
