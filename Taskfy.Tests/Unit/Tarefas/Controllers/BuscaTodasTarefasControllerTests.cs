using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using Taskfy.API.Controllers;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

namespace Taskfy.Tests.Unit.Tarefas.Controllers
{
	public class BuscaTodasTarefasControllerTests : BaseControllerSetup
	{
		[Fact]
		public async Task DeveRetornar_200OK_AoBuscarTodasTarefas()
		{
			var userId = Guid.NewGuid().ToString();

			var tarefasResponseDto = new List<TarefaDTO>
			{
				new TarefaDTO
				{
					Id = Guid.NewGuid(),
					Titulo = "Nova Tarefa",
					Descricao = "Descrição da nova tarefa",
					Data_vencimento = DateTime.Now.AddDays(1),
					Status = false,
					Usuario_id = userId
				},
				new TarefaDTO
				{
					Id = Guid.NewGuid(),
					Titulo = "Nova Tarefa 2",
					Descricao = "Descrição da nova tarefa 2",
					Data_vencimento = DateTime.Now.AddDays(2),
					Status = false,
					Usuario_id = userId
				}
			}.AsEnumerable();

			var responseTarefasEncontradas = new TarefaResponseDTO<IEnumerable<TarefaDTO>>
			{
				Data = tarefasResponseDto,
				StatusCode = StatusCodes.Status200OK,
			};

			TarefaServiceMock.BuscaTodasTarefasAsync(Arg.Any<ClaimsPrincipal>())
			.Returns(Task.FromResult(responseTarefasEncontradas as ResponseDTO));

			var controller = new TarefaController(TarefaServiceMock);

			// Act
			var resultado = await controller.BuscaTodasTarefas() as ObjectResult;

			// Assert
			resultado.Should().NotBeNull();
			resultado?.StatusCode.Should().Be(StatusCodes.Status200OK);
			resultado?.Value.Should().BeEquivalentTo(responseTarefasEncontradas);
		}

		[Fact]
		public async Task DeveRetornar_401Unauthorized_QuandoUserIdInvalido()
		{
			var responseTarefaUnauthorized = new ResponseDTO
			{
				Status = "Erro",
				Message = "Usuário não autorizado.",
				StatusCode = StatusCodes.Status401Unauthorized,
			};

			TarefaServiceMock.BuscaTodasTarefasAsync(Arg.Any<ClaimsPrincipal>())
			.Returns(Task.FromResult(responseTarefaUnauthorized as ResponseDTO));

			var controller = new TarefaController(TarefaServiceMock);

			// Act
			var resultado = await controller.BuscaTodasTarefas() as ObjectResult;

			// Assert
			resultado.Should().NotBeNull();
			resultado?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
			resultado?.Value.Should().BeEquivalentTo(responseTarefaUnauthorized);
		}
	}
}
