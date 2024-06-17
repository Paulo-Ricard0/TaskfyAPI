using Taskfy.API.Repositories.Tarefas;
using Taskfy.API.Repositories.Usuarios;

namespace Taskfy.API.UnitOfWork;

public interface IUnitOfWork
{
	IUsuarioRepository UsuarioRepository { get; }
	ITarefaRepository TarefaRepository { get; }
	Task CommitAsync();
}
