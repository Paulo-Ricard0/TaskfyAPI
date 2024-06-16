using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Taskfy.API.Data;

namespace Taskfy.API.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
	protected readonly AppDbContext _context;

	public Repository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
	{
		var query = _context.Set<T>().AsNoTracking();

		if (predicate != null)
		{
			query = query.Where(predicate);
		}

		return await query.ToListAsync();
	}

	public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
	{
		return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
	}

	public async Task<T?> FindAsync(params object[] keyValues)
	{
		return await _context.Set<T>().FindAsync(keyValues);
	}

	public T Create(T entity)
	{
		_context.Set<T>().Add(entity);
		return entity;
	}

	public T Update(T entity)
	{
		_context.Set<T>().Update(entity);
		return entity;
	}

	public T Delete(T entity)
	{
		_context.Set<T>().Remove(entity);
		return entity;
	}
}
