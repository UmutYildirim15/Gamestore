using System.Linq.Expressions;

namespace Northwind.Data.Repositories.Abstract;

public interface IGenericRepository<T>
    where T : class
{
    Task<T> GetOneAsync(Expression<Func<T, bool>> predicate);

    Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);

    Task AddAsync(T entity);

    Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null);

    Task UpdateAsync(T entity);

    Task DeleteAsync(string? id);
}