using System.Text.Json.Serialization;

namespace Taskfy.API.DTOs.Tarefas.Response;

public class TarefaResponseDTO : ResponseDTO
{
	public TarefaDTO? Data { get; set; }

	[JsonIgnore]
	public override string Status { get; set; } = string.Empty;
	[JsonIgnore]
	public override string Message { get; set; } = string.Empty;
}
