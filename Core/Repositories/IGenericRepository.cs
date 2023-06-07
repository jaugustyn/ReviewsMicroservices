using System.Linq.Expressions;
using Core.Entities;

namespace Core.Repositories;

public interface IGenericRepository<T> where T : class, IEntityBase
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
    Task<T> GetByIdAsync(Guid id);
    Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);
    Task<T> CreateAsync(T entity);
    Task UpdateAsync(Guid id, T entity);
    Task DeleteAsync(Guid id);
}