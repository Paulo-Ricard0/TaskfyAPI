using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Request;
using Taskfy.API.DTOs.Tarefas.Response;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Logs;
using Taskfy.API.Models;
using Taskfy.API.Services.MessagesQueue;
using Taskfy.API.UnitOfWork;

namespace Taskfy.API.Services.Tarefas;

public class TarefaService : ITarefaService
{
	private readonly IUnitOfWork _repository;
	private readonly ILog _logger;
	private readonly IMapper _mapper;
	private readonly IMessageQueueService _messageQueueService;

	public TarefaService(IUnitOfWork repository, ILog logger, IMapper mapper, IMessageQueueService messageQueueService)
	{
		_repository = repository;
		_logger = logger;
		_mapper = mapper;
		_messageQueueService = messageQueueService;
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

		var usuario = await _repository.UsuarioRepository
			.FilterByIdAsync(u => u.Id == userId, u => new UserProjection { Name = u.Name, Email = u.Email! });

		if (usuario != null)
		{
			_messageQueueService.PublishTaskCreated(usuario, responseTarefaDTO);
		}

		_logger.LogToFile("Tarefa", "Tarefa criada com sucesso!");

		return new TarefaResponseDTO<TarefaDTO>
		{
			Data = responseTarefaDTO,
			StatusCode = StatusCodes.Status201Created,
		};
	}

	public async Task<ResponseDTO> BuscaTodasTarefasAsync(ClaimsPrincipal user)
	{
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

		var tarefas = await _repository.TarefaRepository.GetAllAsync(t => t.Usuario_id == userId);
		if (tarefas.IsNullOrEmpty())
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Tarefas não encontradas.",
				StatusCode = StatusCodes.Status404NotFound,
			};
		}

		var responseTarefasDTO = _mapper.Map<IEnumerable<TarefaDTO>>(tarefas);

		return new TarefaResponseDTO<IEnumerable<TarefaDTO>>
		{
			Data = responseTarefasDTO,
			StatusCode = StatusCodes.Status200OK,
		};
	}

	public async Task<ResponseDTO> BuscaTarefaPorIdAsync(ClaimsPrincipal user, Guid tarefaId)
	{
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

		var tarefa = await _repository.TarefaRepository.GetAsync(t => t.Id == tarefaId && t.Usuario_id == userId);
		if (tarefa == null)
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Tarefa não encontrada.",
				StatusCode = StatusCodes.Status404NotFound,
			};
		}

		var responseTarefaDTO = _mapper.Map<TarefaDTO>(tarefa);

		return new TarefaResponseDTO<TarefaDTO>
		{
			Data = responseTarefaDTO,
			StatusCode = StatusCodes.Status200OK,
		};
	}

	public async Task<ResponseDTO> AtualizaTarefa(ClaimsPrincipal user, Guid tarefaId, TarefaRequestUpdateDTO tarefaModel)
	{
		var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

		var tarefaExistente = await _repository.TarefaRepository.FindAsync(tarefaId);
		if (tarefaExistente == null)
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Tarefa não encontrada.",
				StatusCode = StatusCodes.Status404NotFound,
			};
		}

		if (userId != tarefaExistente.Usuario_id)
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Você não tem permissão para atualizar essa tarefa.",
				StatusCode = StatusCodes.Status403Forbidden,
			};
		}

		tarefaExistente.Titulo = tarefaModel.Titulo;
		tarefaExistente.Descricao = tarefaModel.Descricao;
		tarefaExistente.Data_vencimento = tarefaModel.Data_vencimento;
		tarefaExistente.Status = tarefaModel.Status;

		var tarefaAtualizada = _repository.TarefaRepository.Update(tarefaExistente);
		await _repository.CommitAsync();

		var responseTarefaAtualizadaDTO = _mapper.Map<TarefaDTO>(tarefaAtualizada);

		return new TarefaResponseDTO<TarefaDTO>
		{
			Data = responseTarefaAtualizadaDTO,
			StatusCode = StatusCodes.Status200OK,
		};
	}

	public async Task<ResponseDTO> DeletaTarefa(ClaimsPrincipal user, Guid tarefaId)
	{
		var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

		var tarefaExistente = await _repository.TarefaRepository.FindAsync(tarefaId);
		if (tarefaExistente == null)
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Tarefa não encontrada.",
				StatusCode = StatusCodes.Status404NotFound,
			};
		}

		if (userId != tarefaExistente.Usuario_id)
		{
			return new ResponseDTO
			{
				Status = "Erro",
				Message = "Você não tem permissão para deletar essa tarefa.",
				StatusCode = StatusCodes.Status403Forbidden,
			};
		}

		_repository.TarefaRepository.Delete(tarefaExistente);
		await _repository.CommitAsync();

		return new ResponseDTO
		{
			Status = "Sucesso",
			Message = "Tarefa deletada com sucesso.",
			StatusCode = StatusCodes.Status200OK,
		};
	}
}
