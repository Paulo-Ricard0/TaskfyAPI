using System.ComponentModel.DataAnnotations;

namespace Taskfy.API.DTOs.Usuario;

public class RegistroModelDTO
{
	[Required(ErrorMessage = "Nome de usuário é obrigatório.")]
	public string? UserName { get; set; }

	[EmailAddress(ErrorMessage = "Email inválido.")]
	[Required(ErrorMessage = "Email é obrigatório.")]
	public string? Email { get; set; }

	[Required(ErrorMessage = "Senha é obrigatória.")]
	public string? Password { get; set; }
}
