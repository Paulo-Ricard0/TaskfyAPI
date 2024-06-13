﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskfy.API.DTOs;
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
	[ProducesResponseType(typeof(TarefaResponseDTO), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[ProducesDefaultResponseType]
	public async Task<IActionResult> CriaTarefa([FromBody] TarefaRequestDTO tarefaModel)
	{
		var response = await _tarefaService.CriaTarefaAsync(tarefaModel, User);

		return StatusCode(response.StatusCode, response);
	}
}
