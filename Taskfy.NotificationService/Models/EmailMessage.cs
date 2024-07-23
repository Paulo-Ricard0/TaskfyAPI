namespace Taskfy.NotificationService.Models;
public class EmailMessage
{
	public required string To { get; set; }
	public required string Subject { get; set; }
	public required NotificationMessage Body { get; set; }
}