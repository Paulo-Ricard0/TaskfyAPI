using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Taskfy.API.Services.MessagesQueue;

public enum NotificationType
{
	UserCreated,
	TaskCreated,
	TaskUpdated,
	TaskDeleted
}

public class MessageQueueService : IMessageQueueService
{
	private readonly IConnection _connection;
	private readonly IModel _channel;

	public MessageQueueService(IConnection connection, IModel channel)
	{
		_connection = connection;
		_channel = channel;
	}

	public void PublishUserCreatedMessage(string userName, string email)
	{
		var message = new
		{
			UserName = userName,
			ToEmail = email,
			Type = NotificationType.UserCreated
		};

		PublishMessage("taskfy_notification_queue", message);
	}

	public void PublishTaskCreatedMessage(string userName, string email, string tituloTarefa, string descricaoTarefa, string dataVencimentoTarefa)
	{
		throw new NotImplementedException();
	}

	public void PublishTaskDeletedMessage(string userName, string email, string tituloTarefa)
	{
		throw new NotImplementedException();
	}

	public void PublishTaskUpdatedMessage(string userName, string email, string tituloTarefa, string descricaoTarefa, string dataVencimentoTarefa)
	{
		throw new NotImplementedException();
	}

	private void PublishMessage(string queueName, object message)
	{
		var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

		_channel.BasicPublish(exchange: "",
							routingKey: queueName,
							basicProperties: null,
							body: body);
	}

	public void Dispose()
	{
		_channel.Close();
		_connection.Close();
	}
}
