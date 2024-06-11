using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Taskfy.API.Models;

public class Usuario : IdentityUser
{
	public string? RefreshToken { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }

	[JsonIgnore]
	public ICollection<Tarefa>? Tarefas { get; set; }
}
