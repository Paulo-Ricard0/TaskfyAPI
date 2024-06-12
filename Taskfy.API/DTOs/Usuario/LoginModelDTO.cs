using System.ComponentModel.DataAnnotations;

namespace Taskfy.API.DTOs.Usuario;

public class LoginModelDTO
{
	[EmailAddress(ErrorMessage = "Email inválido.")]
	[Required(ErrorMessage = "Email é obrigatório.")]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "Senha é obrigatória.")]
	[StringLength(100)]
	public string Password { get; set; } = string.Empty;
}
