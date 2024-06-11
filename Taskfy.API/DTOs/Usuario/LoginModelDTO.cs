using System.ComponentModel.DataAnnotations;

namespace Taskfy.API.DTOs.Usuario;

public class LoginModelDTO
{
	[Required(ErrorMessage = "Email é obrigatório.")]
	public string? Email { get; set; }

	[Required(ErrorMessage = "Senha é obrigatória.")]
	public string? Password { get; set; }
}
