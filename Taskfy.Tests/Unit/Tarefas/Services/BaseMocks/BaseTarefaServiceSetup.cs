using AutoMapper;
using NSubstitute;
using Taskfy.API.Logs;
using Taskfy.API.Repositories.Tarefas;
using Taskfy.API.Repositories.Usuarios;
using Taskfy.API.Services.MessagesQueue;
using Taskfy.API.Services.Tarefas;
using Taskfy.API.UnitOfWork;

namespace Taskfy.Tests.Unit.Tarefas.Services.Mocks;

public abstract class BaseTarefaServiceSetup
{
	protected readonly IUnitOfWork UnitOfWorkMock;
	protected readonly ILog LoggerMock;
	protected readonly IMapper MapperMock;
	protected readonly ITarefaRepository TarefaRepositoryMock;
	protected readonly IUsuarioRepository UsuarioRepositoryMock;
	protected readonly IMessageQueueService MessageQueueServiceMock;
	protected readonly ITarefaService TarefaServiceMock;

	protected BaseTarefaServiceSetup()
	{
		UnitOfWorkMock = Substitute.For<IUnitOfWork>();
		LoggerMock = Substitute.For<ILog>();
		MapperMock = Substitute.For<IMapper>();
		TarefaRepositoryMock = Substitute.For<ITarefaRepository>();
		UsuarioRepositoryMock = Substitute.For<IUsuarioRepository>();
		MessageQueueServiceMock = Substitute.For<IMessageQueueService>();

		TarefaServiceMock = new TarefaService(UnitOfWorkMock, LoggerMock, MapperMock, MessageQueueServiceMock);

		UnitOfWorkMock.TarefaRepository.Returns(TarefaRepositoryMock);
		UnitOfWorkMock.UsuarioRepository.Returns(UsuarioRepositoryMock);
	}
}
