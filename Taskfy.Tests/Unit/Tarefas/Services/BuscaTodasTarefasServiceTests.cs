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

namespace Taskfy.Tests.Unit.Tarefas.Services;

public class BuscaTodasTarefasServiceTests : BaseTarefaServiceSetup
{
	[Fact]
	public async Task DeveRetornar_200OK_QuandoUsuarioPossuirTarefas()
	{
		// Arrange
		var userId = MocksData.User.GetUserId();

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		var tarefas = MocksData.Tarefa.ListaTarefas(userId);

		var tarefasDTO = tarefas.ConvertAll(t => MocksData.Tarefa.GetTarefaDTO(t));

		UnitOfWorkMock.TarefaRepository.GetAllAsync(Arg.Any<Expression<Func<Tarefa, bool>>>())
			.Returns(Task.FromResult<IEnumerable<Tarefa>>(tarefas));

		MapperMock.Map<IEnumerable<TarefaDTO>>(tarefas).Returns(tarefasDTO);

		// Act
		var resultado = await TarefaServiceMock.BuscaTodasTarefasAsync(claimsPrincipal) as TarefaResponseDTO<IEnumerable<TarefaDTO>>;

		// Assert
		await UnitOfWorkMock.TarefaRepository.Received(1).GetAllAsync(Arg.Any<Expression<Func<Tarefa, bool>>>());
		resultado.Should().NotBeNull();
		resultado.Should().BeOfType<TarefaResponseDTO<IEnumerable<TarefaDTO>>>();
		resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
		resultado?.Data.Should().BeEquivalentTo(tarefasDTO);
	}

	[Fact]
	public async Task DeveRetornar_401Unauthorized_QuandoUsuarioIdInvalido()
	{
		// Arrange
		const string? userId = "";

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		// Act
		var resultado = await TarefaServiceMock.BuscaTodasTarefasAsync(claimsPrincipal);

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
		var userId = MocksData.User.GetUserId();
		;
		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		UnitOfWorkMock.TarefaRepository.GetAllAsync(Arg.Any<Expression<Func<Tarefa, bool>>>())
			.Returns(Task.FromResult<IEnumerable<Tarefa>>(null!));

		// Act
		var resultado = await TarefaServiceMock.BuscaTodasTarefasAsync(claimsPrincipal);

		// Assert
		resultado.Should().NotBeNull();
		resultado.Should().BeOfType<ResponseDTO>();
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Tarefas não encontradas.");
		resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
	}
}
