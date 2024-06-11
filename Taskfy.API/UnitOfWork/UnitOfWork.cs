using Taskfy.API.Data;
using Taskfy.API.Repositories.Usuarios;

namespace Taskfy.API.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
	private IUsuarioRepository? _usuarioRepository;
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

	public async Task CommitAsync()
	{
		await _context.SaveChangesAsync();
	}
}
