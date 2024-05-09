namespace OnlineShop.Models.Interfaces.Repository
{
    public interface IBaseRepository<T>
    {
        IQueryable<T> GetAll();
        Task<T> AddAsync(T entity);
        Task<T> Update(T entity);
        void Delete(T entity);
    }
}
