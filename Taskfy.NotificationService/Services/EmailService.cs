using Taskfy.NotificationService.Models;
using Taskfy.NotificationService.Services.Interfaces;

namespace Taskfy.NotificationService.Services;
public class EmailService : IEmailService
{
	public string GetEmailSubject(NotificationType notificationType, string? tituloTarefa)
	{
		return notificationType switch
		{
			NotificationType.UserCreated => "Bem-vindo ao Taskfy!",
			NotificationType.TaskCreated => $"Nova tarefa criada: {tituloTarefa}",
			NotificationType.TaskUpdated => $"Tarefa atualizada: {tituloTarefa}",
			NotificationType.TaskDeleted => $"Tarefa deletada: {tituloTarefa}",
			_ => "Notificação Taskfy"
		};
	}
}
