namespace Taskfy.EmailSender.Models;
public class EmailDetails
{
	public required string UserName { get; set; }
	public required string ToEmail { get; set; }
	public EmailType Type { get; set; }
	public string? TituloTarefa { get; set; }
	public string? DescricaoTarefa { get; set; }
	public DateTime DataVencimentoTarefa { get; set; }
	public string? DataVencimentoFormatada { get; set; }
}
