using Taskfy.NotificationService.Models;

namespace Taskfy.NotificationService.Services.Interfaces;
public interface INotificationHandler
{
	void HandleMessage(NotificationMessage notificationMessage);
}
