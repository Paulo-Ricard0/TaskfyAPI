﻿using Microsoft.AspNetCore.Authorization;
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
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[ProducesDefaultResponseType]
	public async Task<IActionResult> BuscaTarefaPorId(Guid tarefaId)
	{
		var response = await _tarefaService.BuscaTarefaPorIdAsync(User, tarefaId);

		return StatusCode(response.StatusCode, response);
	}
}
