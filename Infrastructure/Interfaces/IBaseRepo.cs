namespace Infrastructure.Interfaces
{
    public interface IBaseRepo<T, TKey> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(TKey id);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
