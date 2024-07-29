namespace Taskfy.NotificationService.Models;
public class NotificationMessage
{
	public required string UserName { get; set; }
	public required string ToEmail { get; set; }
	public NotificationType Type { get; set; }
	public string? TituloTarefa { get; set; }
	public string? DescricaoTarefa { get; set; }
	public DateTime DataVencimentoTarefa { get; set; }
}