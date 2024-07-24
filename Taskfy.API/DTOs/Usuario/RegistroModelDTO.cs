using System.ComponentModel.DataAnnotations;

namespace Taskfy.API.DTOs.Usuario;

public class RegistroModelDTO
{
	[Required(ErrorMessage = "Nome de usuário é obrigatório.")]
	[StringLength(100)]
	public required string Name { get; set; }

	[EmailAddress(ErrorMessage = "Email inválido.")]
	[Required(ErrorMessage = "Email é obrigatório.")]
	public required string Email { get; set; }

	[Required(ErrorMessage = "Senha é obrigatória.")]
	[StringLength(100)]
	public required string Password { get; set; }
}
