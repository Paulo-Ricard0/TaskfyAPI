using Taskfy.API.Data;
using Taskfy.API.Models;

namespace Taskfy.API.Repositories.Tarefas;

public class TarefaRepository : Repository<Tarefa>, ITarefaRepository
{
	public TarefaRepository(AppDbContext context) : base(context)
	{
	}
}
