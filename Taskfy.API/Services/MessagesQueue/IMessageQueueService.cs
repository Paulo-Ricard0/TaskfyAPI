namespace Taskfy.API.Services.MessagesQueue;

public interface IMessageQueueService
{
	void PublishUserCreatedMessage(string userName, string email);
	void PublishTaskCreatedMessage(string userName, string email, string tituloTarefa, string descricaoTarefa, string dataVencimentoTarefa);
	void PublishTaskUpdatedMessage(string userName, string email, string tituloTarefa, string descricaoTarefa, string dataVencimentoTarefa);
	void PublishTaskDeletedMessage(string userName, string email, string tituloTarefa);
}
