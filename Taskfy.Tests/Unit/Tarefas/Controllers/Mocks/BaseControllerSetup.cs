using NSubstitute;
using Taskfy.API.Services.Tarefas;

namespace Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

public abstract class BaseControllerSetup
{
	protected readonly ITarefaService TarefaServiceMock;

	protected BaseControllerSetup()
	{
		TarefaServiceMock = Substitute.For<ITarefaService>();
	}
}
