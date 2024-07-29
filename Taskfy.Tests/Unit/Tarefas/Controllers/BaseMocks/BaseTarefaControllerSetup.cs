using NSubstitute;
using Taskfy.API.Controllers;
using Taskfy.API.Services.Tarefas;

namespace Taskfy.Tests.Unit.Tarefas.Controllers.Mocks;

public abstract class BaseTarefaControllerSetup
{
	protected readonly ITarefaService TarefaServiceMock;
	protected readonly TarefaController TarefaControllerMock;

	protected BaseTarefaControllerSetup()
	{
		TarefaServiceMock = Substitute.For<ITarefaService>();
		TarefaControllerMock = new TarefaController(TarefaServiceMock);
	}
}
