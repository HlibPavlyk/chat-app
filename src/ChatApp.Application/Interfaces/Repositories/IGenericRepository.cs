namespace ChatApp.Application.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>?> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task Remove(int id);
    void Update(TEntity entity);
}