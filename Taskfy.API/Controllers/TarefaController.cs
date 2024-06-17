using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Request;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.API.Services.Tarefas;

namespace Taskfy.API.Controllers;

[Route("api/tasks/")]
[ApiController]
[Produces("application/json")]
public class TarefaController : ControllerBase
{
	private readonly ITarefaService _tarefaService;

	public TarefaController(ITarefaService tarefaService)
	{
		_tarefaService = tarefaService;
	}

	[Authorize]
	[HttpPost]
	[ProducesResponseType(typeof(TarefaResponseDTO<TarefaDTO>), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(GlobalErrorResponseDTO), StatusCodes.Status500InternalServerError)]
	[ProducesDefaultResponseType]
	public async Task<IActionResult> CriaTarefa([FromBody] TarefaRequestDTO tarefaModel)
	{
		var response = await _tarefaService.CriaTarefaAsync(tarefaModel, User);

		return StatusCode(response.StatusCode, response);
	}

	[Authorize]
	[HttpGet]
	[ProducesResponseType(typeof(TarefaResponseDTO<IEnumerable<TarefaDTO>>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(GlobalErrorResponseDTO), StatusCodes.Status500InternalServerError)]
	[ProducesDefaultResponseType]
	public async Task<IActionResult> BuscaTodasTarefas()
	{
		var response = await _tarefaService.BuscaTodasTarefasAsync(User);

		return StatusCode(response.StatusCode, response);
	}

	[Authorize]
	[HttpGet("{tarefaId}")]
	[ProducesResponseType(typeof(TarefaResponseDTO<TarefaDTO>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(GlobalErrorResponseDTO), StatusCodes.Status500InternalServerError)]
	[ProducesDefaultResponseType]
	public async Task<IActionResult> BuscaTarefaPorId(Guid tarefaId)
	{
		var response = await _tarefaService.BuscaTarefaPorIdAsync(User, tarefaId);

		return StatusCode(response.StatusCode, response);
	}

	[Authorize]
	[HttpPut("{tarefaId}")]
	[ProducesResponseType(typeof(TarefaResponseDTO<TarefaDTO>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(GlobalErrorResponseDTO), StatusCodes.Status500InternalServerError)]
	[ProducesDefaultResponseType]
	public async Task<IActionResult> AtualizaTarefa(Guid tarefaId, [FromBody] TarefaRequestUpdateDTO tarefaModel)
	{
		var response = await _tarefaService.AtualizaTarefa(User, tarefaId, tarefaModel);

		return StatusCode(response.StatusCode, response);
	}

	[Authorize]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(GlobalErrorResponseDTO), StatusCodes.Status500InternalServerError)]
	[ProducesDefaultResponseType]
	[HttpDelete("{tarefaId}")]
	public async Task<IActionResult> DeletaTarefa(Guid tarefaId)
	{
		var response = await _tarefaService.DeletaTarefa(User, tarefaId);

		return StatusCode(response.StatusCode, response);
	}
}
