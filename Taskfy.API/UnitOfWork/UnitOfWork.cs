using Taskfy.API.Data;
using Taskfy.API.Repositories.Tarefas;
using Taskfy.API.Repositories.Usuarios;

namespace Taskfy.API.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
	private IUsuarioRepository? _usuarioRepository;
	private ITarefaRepository? _tarefaRepository;
	public AppDbContext _context;

	public UnitOfWork(AppDbContext context)
	{
		_context = context;
	}

	public IUsuarioRepository UsuarioRepository
	{
		get
		{
			return _usuarioRepository = _usuarioRepository ?? new UsuarioRepository(_context);
		}
	}

	public ITarefaRepository TarefaRepository
	{
		get
		{
			return _tarefaRepository = _tarefaRepository ?? new TarefaRepository(_context);
		}
	}

	public async Task CommitAsync()
	{
		await _context.SaveChangesAsync();
	}
}
