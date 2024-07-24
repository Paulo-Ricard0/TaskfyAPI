using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Taskfy.NotificationService.Models;
using Taskfy.NotificationService.Services.Interfaces;

namespace Taskfy.NotificationService.Services;
public class NotificationHandler : INotificationHandler
{
	private readonly IEmailService _emailService;
	private readonly IModel _channel;
	private readonly ILogger<NotificationHandler> _logger;

	public NotificationHandler(IEmailService emailService, IModel channel, ILogger<NotificationHandler> logger)
	{
		_emailService = emailService;
		_channel = channel;
		_logger = logger;
	}

	public void HandleMessage(NotificationMessage notificationMessage)
	{
		_logger.LogInformation("[x] - Received notification: {notification.Type} - {DateTime}", notificationMessage.Type, DateTime.Now);

		var emailMessage = new EmailMessage
		{
			To = notificationMessage.ToEmail,
			Subject = _emailService.GetEmailSubject(notificationMessage.Type, notificationMessage.TituloTarefa),
			Body = notificationMessage
		};

		var serializedMessage = JsonConvert.SerializeObject(emailMessage);

		_logger.LogInformation("[x] - Email message: {emailMessage} - {DateTime}", serializedMessage, DateTime.Now);

		var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(emailMessage));

		_channel.BasicPublish(exchange: "",
							routingKey: "taskfy_email_queue",
							basicProperties: null,
							body: body);
	}
}
