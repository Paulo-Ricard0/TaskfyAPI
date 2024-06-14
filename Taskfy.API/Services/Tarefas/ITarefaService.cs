using System.Security.Claims;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Tarefas.Request;

namespace Taskfy.API.Services.Tarefas;

public interface ITarefaService
{
	Task<ResponseDTO> CriaTarefaAsync(TarefaRequestDTO tarefaModel, ClaimsPrincipal user);
	Task<ResponseDTO> BuscaTodasTarefasAsync(ClaimsPrincipal user);
}
