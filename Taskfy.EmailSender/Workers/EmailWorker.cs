using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Taskfy.EmailSender.Models;
using Taskfy.EmailSender.Services.Interfaces;

namespace Taskfy.EmailSender.Workers;

public class EmailWorker : BackgroundService
{
	private readonly ILogger<EmailWorker> _logger;
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly IEmailService _emailService;


	public EmailWorker(ILogger<EmailWorker> logger, IConnection connection, IModel channel, IEmailService emailService)
	{
		_logger = logger;
		_connection = connection;
		_channel = channel;
		_emailService = emailService;

		_channel.QueueDeclare(queue: "taskfy_email_queue",
							durable: true,
							exclusive: false,
							autoDelete: false,
							arguments: null);
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		stoppingToken.Register(() => _logger.LogInformation("[x] - Worker stopping - {DateTime}", DateTime.Now));

		var consumer = new EventingBasicConsumer(_channel);
		consumer.Received += (model, ea) =>
		{
			try
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);

				_logger.LogInformation("[x] - Received message: {message} - {DateTime}", message, DateTime.Now);

				var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(message);

				if (emailMessage != null)
				{
					_emailService.SendEmailAsync(emailMessage);

					_channel.BasicAck(ea.DeliveryTag, false);
					_logger.LogInformation("[x] - E-mail sent successfully! - {DateTime}", DateTime.Now);
				}
				else
				{
					_logger.LogWarning("[x] - Received null or invalid message - {DateTime}", DateTime.Now);
				}

			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "[x] - Error processing message - {DateTime}", DateTime.Now);
				_channel.BasicNack(ea.DeliveryTag, false, true);
			}
		};

		_channel.BasicConsume(queue: "taskfy_email_queue",
							autoAck: false,
							consumer: consumer);

		return Task.CompletedTask;
	}

	public override void Dispose()
	{
		_channel.Close();
		_connection.Close();
		base.Dispose();
	}
}
