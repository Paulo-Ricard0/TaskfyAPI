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
	public required string Titulo { get; set; }

	[Required]
	[StringLength(255)]
	public required string Descricao { get; set; }

	[Required]
	public DateTime Data_vencimento { get; set; }

	public bool Status { get; set; } = false;

	[Required]
	public string? Usuario_id { get; set; }

	[ForeignKey("Usuario_id")]
	[JsonIgnore]
	public Usuario? Usuario { get; set; }
}
