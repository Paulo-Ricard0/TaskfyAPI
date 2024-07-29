using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Usuario;

namespace Taskfy.API.Services.MessagesQueue;

public interface IMessageQueueService
{
	void PublishUserCreated(string userName, string email);
	void PublishTaskCreated(UserProjection usuario, TarefaDTO tarefa);
	void PublishTaskUpdated(UserProjection usuario, TarefaDTO tarefa);
	void PublishTaskDeleted(UserProjection usuario, TarefaDTO tarefa);
}
