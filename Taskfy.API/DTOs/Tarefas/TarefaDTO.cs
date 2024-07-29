using System.ComponentModel.DataAnnotations;

namespace Taskfy.API.DTOs.Tarefas;

public class TarefaDTO
{
	[Required]
	public Guid Id { get; set; }

	[Required]
	public required string Titulo { get; set; }

	[Required]
	public required string Descricao { get; set; }

	[Required]
	public DateTime Data_vencimento { get; set; }

	[Required]
	public bool Status { get; set; } = false;

	[Required]
	public string? Usuario_id { get; set; }
}
