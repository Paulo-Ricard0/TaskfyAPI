using System.ComponentModel.DataAnnotations;

namespace Taskfy.API.DTOs.Usuario;

public class RegistroModelDTO
{
	[Required(ErrorMessage = "Nome de usuário é obrigatório.")]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;

	[EmailAddress(ErrorMessage = "Email inválido.")]
	[Required(ErrorMessage = "Email é obrigatório.")]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "Senha é obrigatória.")]
	[StringLength(100)]
	public string Password { get; set; } = string.Empty;
}
