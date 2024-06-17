using System.ComponentModel.DataAnnotations;

namespace Taskfy.API.DTOs.Tarefas.Request;

public class TarefaRequestDTO
{
	[Required(ErrorMessage = "O titulo da tarefa é obrigatório")]
	[StringLength(100)]
	public string Titulo { get; set; } = string.Empty;

	[Required(ErrorMessage = "A descrição é obrigatória")]
	[StringLength(255)]
	public string Descricao { get; set; } = string.Empty;

	[Required(ErrorMessage = "A data de vencimento é obrigatória")]
	public DateTime Data_vencimento { get; set; }
}
