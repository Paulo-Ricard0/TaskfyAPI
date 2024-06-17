using System.Text.Json.Serialization;

namespace Taskfy.API.DTOs.Tarefas.Response;

public class TarefaResponseDTO<T> : ResponseDTO
{
	public T? Data { get; set; }

	[JsonIgnore]
	public override string Status { get; set; } = string.Empty;
	[JsonIgnore]
	public override string Message { get; set; } = string.Empty;
}
