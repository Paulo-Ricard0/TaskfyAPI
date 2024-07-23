using Taskfy.NotificationService.Models;

namespace Taskfy.NotificationService.Services.Interfaces;
public interface IEmailService
{
	string GetEmailSubject(NotificationType notificationType, string? tituloTarefa);
}
