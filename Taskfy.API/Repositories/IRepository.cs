﻿using System.Linq.Expressions;

namespace Taskfy.API.Repositories;

public interface IRepository<T> where T : class
{
	Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);
	Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
	Task<T?> FindAsync(params object[] keyValues);
	T Create(T entity);
	T Update(T entity);
	T Delete(T entity);
}
