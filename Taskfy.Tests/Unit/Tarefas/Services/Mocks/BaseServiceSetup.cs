using AutoMapper;
using NSubstitute;
using Taskfy.API.Logs;
using Taskfy.API.Repositories.Tarefas;
using Taskfy.API.UnitOfWork;

namespace Taskfy.Tests.Unit.Tarefas.Services.Mocks;

public abstract class BaseServiceSetup
{
	protected readonly IUnitOfWork UnitOfWorkMock;
	protected readonly ILog LoggerMock;
	protected readonly IMapper MapperMock;
	protected readonly ITarefaRepository TarefaRepositoryMock;

	protected BaseServiceSetup()
	{
		UnitOfWorkMock = Substitute.For<IUnitOfWork>();
		LoggerMock = Substitute.For<ILog>();
		MapperMock = Substitute.For<IMapper>();
		TarefaRepositoryMock = Substitute.For<ITarefaRepository>();

		UnitOfWorkMock.TarefaRepository.Returns(TarefaRepositoryMock);
	}
}
