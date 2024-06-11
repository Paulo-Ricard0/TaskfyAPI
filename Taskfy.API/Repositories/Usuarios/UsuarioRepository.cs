using Taskfy.API.Data;
using Taskfy.API.Models;

namespace Taskfy.API.Repositories.Usuarios;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
	public UsuarioRepository(AppDbContext context) : base(context)
	{
	}
}
