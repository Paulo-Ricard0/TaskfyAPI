using System.ComponentModel.DataAnnotations;

namespace Taskfy.API.DTOs.Tarefas.Request;

public class TarefaRequestDTO
{
	[Required(ErrorMessage = "O titulo da tarefa é obrigatório")]
	[StringLength(100)]
	public required string Titulo { get; set; }

	[Required(ErrorMessage = "A descrição é obrigatória")]
	[StringLength(255)]
	public required string Descricao { get; set; }

	[Required(ErrorMessage = "A data de vencimento é obrigatória")]
	public DateTime Data_vencimento { get; set; }
}
