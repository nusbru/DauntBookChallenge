namespace Daunt.Core.Repository;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync(int page, int size);
    Task<T> AddAsync(T entity);
    Task Update(T entity);
    Task Delete(Guid id);
}