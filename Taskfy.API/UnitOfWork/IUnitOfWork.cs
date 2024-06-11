using Taskfy.API.Repositories.Usuarios;

namespace Taskfy.API.UnitOfWork;

public interface IUnitOfWork
{
	IUsuarioRepository UsuarioRepository { get; }
	Task CommitAsync();
}
