using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Usuario;

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

	public void PublishUserCreated(string userName, string email)
	{
		var message = new
		{
			UserName = userName,
			ToEmail = email,
			Type = NotificationType.UserCreated
		};

		PublishMessage("taskfy_notification_queue", message);
	}

	public void PublishTaskCreated(UserProjection usuario, TarefaDTO tarefa)
	{
		var message = new
		{
			UserName = usuario.Name,
			ToEmail = usuario.Email,
			Type = NotificationType.TaskCreated,
			TituloTarefa = tarefa.Titulo,
			DescricaoTarefa = tarefa.Descricao,
			DataVencimentoTarefa = tarefa.Data_vencimento
		};

		PublishMessage("taskfy_notification_queue", message);
	}

	public void PublishTaskUpdated(UserProjection usuario, TarefaDTO tarefa)
	{
		throw new NotImplementedException();
	}

	public void PublishTaskDeleted(UserProjection usuario, TarefaDTO tarefa)
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
