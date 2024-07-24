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
			NotificationType.TaskCreated => $"Nova Tarefa Criada: {tituloTarefa}",
			NotificationType.TaskUpdated => $"Tarefa Atualizada: {tituloTarefa}",
			NotificationType.TaskDeleted => $"Tarefa Excluída: {tituloTarefa}",
			_ => "Notificação Taskfy"
		};
	}
}
