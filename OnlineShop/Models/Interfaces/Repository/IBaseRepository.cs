namespace OnlineShop.Models.Interfaces.Repository
{
    public interface IBaseRepository<T>
    {
        IQueryable<T> GetAll();
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task<T> Update(T entity);
        Task Delete(T entity);
        Task SaveChangesAsync();
        void Attach(T entity);
    }
}
