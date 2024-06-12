using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Taskfy.API.Models;

public class Tarefa
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	[Required]
	[StringLength(100)]
	public string Titulo { get; set; } = string.Empty;

	[Required]
	[StringLength(255)]
	public string Descricao { get; set; } = string.Empty;

	[Required]
	public DateTime Data_vencimento { get; set; }

	public bool Status { get; set; } = false;

	[Required]
	public string? Usuario_id { get; set; }

	[ForeignKey("Usuario_id")]
	[JsonIgnore]
	public Usuario? Usuario { get; set; }
}
