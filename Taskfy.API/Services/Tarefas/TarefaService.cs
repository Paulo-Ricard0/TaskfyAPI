﻿using AutoMapper;
using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Request;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.API.Logs;
using Taskfy.API.Models;
using Taskfy.API.UnitOfWork;

namespace Taskfy.API.Services.Tarefas;

public class TarefaService : ITarefaService
{
	private readonly IUnitOfWork _repository;
	private readonly ILog _logger;
	private readonly IMapper _mapper;

	public TarefaService(IUnitOfWork repository, ILog logger, IMapper mapper)
	{
		_repository = repository;
		_logger = logger;
		_mapper = mapper;
	}

	public async Task<ResponseDTO> CriaTarefaAsync(TarefaRequestDTO tarefaModel, ClaimsPrincipal user)
	{
		if (tarefaModel == null)
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Dados de tarefa inválidos.",
				StatusCode = StatusCodes.Status400BadRequest,
			};
		}

		var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
		if (string.IsNullOrEmpty(userId))
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Usuário não autorizado.",
				StatusCode = StatusCodes.Status401Unauthorized,
			};
		}

		var tarefa = _mapper.Map<Tarefa>(tarefaModel);
		tarefa.Usuario_id = userId;

		var novaTarefa = _repository.TarefaRepository.Create(tarefa);

		await _repository.CommitAsync();

		var responseTarefaDTO = _mapper.Map<TarefaDTO>(novaTarefa);

		return new TarefaResponseDTO
		{
			Data = responseTarefaDTO,
			StatusCode = StatusCodes.Status201Created,
		};
	}
}