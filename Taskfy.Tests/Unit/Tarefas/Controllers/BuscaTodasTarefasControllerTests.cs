using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.Tests.Unit.ServicesMocks;
using Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Controllers
{
	public class BuscaTodasTarefasControllerTests : BaseTarefaControllerSetup
	{
		[Fact]
		public async Task DeveRetornar_200OK_AoBuscarTodasTarefas()
		{
			// Arrange
			var userId = MocksData.User.GetUserId();

			var tarefas = MocksData.Tarefa.ListaTarefas(userId);

			var tarefasResponseDto = tarefas.ConvertAll(t => MocksData.Tarefa.GetTarefaDTO(t));

			var responseTarefasEncontradas = new TarefaResponseDTO<IEnumerable<TarefaDTO>>
			{
				Data = tarefasResponseDto,
				StatusCode = StatusCodes.Status200OK,
			};

			TarefaServiceMock.BuscaTodasTarefasAsync(Arg.Any<ClaimsPrincipal>())
				.Returns(Task.FromResult(responseTarefasEncontradas as ResponseDTO));

			// Act
			var resultado = await TarefaControllerMock.BuscaTodasTarefas() as ObjectResult;

			// Assert
			resultado.Should().NotBeNull();
			resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
			resultado?.Value.Should().BeEquivalentTo(responseTarefasEncontradas);
		}

		[Fact]
		public async Task DeveRetornar_401Unauthorized_QuandoUserIdInvalido()
		{
			// Arrange
			var responseTarefaUnauthorized = new ResponseDTO
			{
				Status = "Erro",
				Message = "Usuário não autorizado.",
				StatusCode = StatusCodes.Status401Unauthorized,
			};

			TarefaServiceMock.BuscaTodasTarefasAsync(Arg.Any<ClaimsPrincipal>())
			.Returns(Task.FromResult(responseTarefaUnauthorized));

			// Act
			var resultado = await TarefaControllerMock.BuscaTodasTarefas() as ObjectResult;

			// Assert
			resultado.Should().NotBeNull();
			resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
			resultado?.Value.Should().BeEquivalentTo(responseTarefaUnauthorized);
		}

		[Fact]
		public async Task DeveRetornar_404NotFound_QuandoTarefasNaoEncontradas()
		{
			// Arrange
			var responseTarefasNotFound = new ResponseDTO
			{
				Status = "Erro",
				Message = "Tarefas não encontradas.",
				StatusCode = StatusCodes.Status404NotFound,
			};

			TarefaServiceMock.BuscaTodasTarefasAsync(Arg.Any<ClaimsPrincipal>())
			.Returns(Task.FromResult(responseTarefasNotFound));

			// Act
			var resultado = await TarefaControllerMock.BuscaTodasTarefas() as ObjectResult;

			// Assert
			resultado.Should().NotBeNull();
			resultado?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
			resultado?.Value.Should().BeEquivalentTo(responseTarefasNotFound);
		}
	}
}
