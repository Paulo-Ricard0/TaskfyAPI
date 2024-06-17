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

namespace Taskfy.Tests.Unit.Tarefas.Services;

public class BuscaTodasTarefasServiceTests : BaseServiceSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_QuandoUsuarioPossuirTarefas()
	{
		// Arrange
		var userId = Guid.NewGuid().ToString();
		var claims = new[]
		{
			new Claim("UserId", userId)
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		var tarefas = new List<Tarefa>
		{
			new Tarefa
			{
				Id = Guid.NewGuid(),
				Titulo = "Tarefa 1",
				Descricao = "Descrição da tarefa 1",
				Data_vencimento = DateTime.Now.AddDays(1),
				Usuario = null,
				Usuario_id = userId,
				Status = false
			},
			new Tarefa
			{
				Id = Guid.NewGuid(),
				Titulo = "Tarefa 2",
				Descricao = "Descrição da tarefa 2",
				Data_vencimento = DateTime.Now.AddDays(2),
				Usuario = null,
				Usuario_id = userId,
				Status = true
			}
		};

		var tarefasDTO = tarefas.ConvertAll(t => new TarefaDTO
		{
			Id = t.Id,
			Titulo = t.Titulo,
			Descricao = t.Descricao,
			Data_vencimento = t.Data_vencimento,
			Status = t.Status,
			Usuario_id = t.Usuario_id
		});

		TarefaRepositoryMock.GetAllAsync(Arg.Any<Expression<Func<Tarefa, bool>>>())
			.Returns(Task.FromResult<IEnumerable<Tarefa>>(tarefas));

		MapperMock.Map<IEnumerable<TarefaDTO>>(tarefas).Returns(tarefasDTO);

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.BuscaTodasTarefasAsync(claimsPrincipal) as TarefaResponseDTO<IEnumerable<TarefaDTO>>;

		// Assert
		await TarefaRepositoryMock.Received(1).GetAllAsync(Arg.Any<Expression<Func<Tarefa, bool>>>());
		resultado.Should().NotBeNull();
		resultado.Should().BeOfType<TarefaResponseDTO<IEnumerable<TarefaDTO>>>();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Data.Should().BeEquivalentTo(tarefasDTO);
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

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.BuscaTodasTarefasAsync(claimsPrincipal);

		// Assert
		resultado.Should().NotBeNull();
		resultado.Should().BeOfType<ResponseDTO>();
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Usuário não autorizado.");
		resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
	}

	[Fact]
	public async Task DeveRetornar_404NotFound_QuandoTarefasNaoEncontradas()
	{
		// Arrange
		var userId = Guid.NewGuid().ToString();
		var claims = new[]
		{
			new Claim("UserId", userId)
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		TarefaRepositoryMock.GetAllAsync(Arg.Any<Expression<Func<Tarefa, bool>>>())
			.Returns(Task.FromResult<IEnumerable<Tarefa>>(null!));

		var tarefaService = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock);

		// Act
		var resultado = await tarefaService.BuscaTodasTarefasAsync(claimsPrincipal);

		// Assert
		resultado.Should().NotBeNull();
		resultado.Should().BeOfType<ResponseDTO>();
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Tarefas não encontradas.");
		resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
	}
}
