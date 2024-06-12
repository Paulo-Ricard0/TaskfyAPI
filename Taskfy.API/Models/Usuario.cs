using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Taskfy.API.Models;

public class Usuario : IdentityUser
{
	[Required]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;
	public string? RefreshToken { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }

	[JsonIgnore]
	public ICollection<Tarefa>? Tarefas { get; set; }
}
