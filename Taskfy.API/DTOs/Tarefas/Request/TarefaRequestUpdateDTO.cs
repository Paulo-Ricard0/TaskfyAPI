using System.ComponentModel.DataAnnotations;

namespace Taskfy.API.DTOs.Tarefas.Request;

public class TarefaRequestUpdateDTO : TarefaRequestDTO
{
	[Required(ErrorMessage = "O Status é obrigatório")]
	public bool Status { get; set; } = false;
}
