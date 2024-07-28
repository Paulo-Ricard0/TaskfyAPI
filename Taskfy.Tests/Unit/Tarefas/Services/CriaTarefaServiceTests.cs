using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Linq.Expressions;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Request;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Models;
using Taskfy.Tests.Unit.ServicesMocks;
using Taskfy.Tests.Unit.Tarefas.Services.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Services;

public class CriaTarefaServiceTests : BaseTarefaServiceSetup
{
	[Fact]
	public async Task DeveRetornar_201Created_QuandoDadosValidos()
	{
		// Arrange
		var userId = MocksData.User.GetUserId();

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		var tarefaRequestDto = MocksData.Tarefa.GetTarefaRequestDTO();

		var tarefa = MocksData.Tarefa.CreateTarefa(userId, tarefaRequestDto);

		var tarefaResponseDto = MocksData.Tarefa.GetTarefaDTO(tarefa);

		var userProjection = MocksData.User.GetUserProjection();

		MapperMock.Map<Tarefa>(tarefaRequestDto).Returns(tarefa);
		MapperMock.Map<TarefaDTO>(tarefa).Returns(tarefaResponseDto);

		UnitOfWorkMock.TarefaRepository.Create(tarefa).Returns(tarefa);
		UnitOfWorkMock.CommitAsync().Returns(Task.CompletedTask);

		UnitOfWorkMock.UsuarioRepository
			.FilterByIdAsync(Arg.Any<Expression<Func<Usuario, bool>>>(), Arg.Any<Expression<Func<Usuario, UserProjection>>>())
			.Returns(Task.FromResult<UserProjection?>(userProjection));

		// Act
		var resultado = await TarefaServiceMock.CriaTarefaAsync(tarefaRequestDto, claimsPrincipal) as TarefaResponseDTO<TarefaDTO>;

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

		var userId = MocksData.User.GetUserId();

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		// Act
		var resultado = await TarefaServiceMock.CriaTarefaAsync(tarefaRequestDto!, claimsPrincipal);

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Dados de tarefa inválidos.");
	}

	[Fact]
	public async Task DeveRetornar_401Unauthorized_QuandoUserIdInvalido()
	{
		// Arrange
		var tarefaRequestDto = MocksData.Tarefa.GetTarefaRequestDTO();

		const string? userId = "";

		var claimsPrincipal = MocksData.User.GetClaimsPrincipal(userId);

		// Act
		var resultado = await TarefaServiceMock.CriaTarefaAsync(tarefaRequestDto, claimsPrincipal);

		// Assert
		resultado.Should().NotBeNull();
		resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
		resultado?.Status.Should().Be("Erro");
		resultado?.Message.Should().Be("Usuário não autorizado.");
	}
}
