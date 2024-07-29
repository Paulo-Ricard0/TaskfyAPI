using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Taskfy.API.Models;

public class Usuario : IdentityUser
{
	[Required]
	[StringLength(100)]
	public required string Name { get; set; }
	public string? RefreshToken { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }

	[JsonIgnore]
	public ICollection<Tarefa>? Tarefas { get; set; }
}
