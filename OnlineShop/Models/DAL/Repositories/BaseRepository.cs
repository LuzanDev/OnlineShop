using OnlineShop.Models.Entity;
using OnlineShop.Models.Interfaces.Repository;

namespace OnlineShop.Models.DAL.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException("Entity is null");
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException("Entity is null");
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public async Task<T> Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException("Entity is null");
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Attach(T entity)
        {
            if (entity == null) throw new ArgumentNullException("Entity is null");
            _context.Attach(entity);
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException("Entities collection is null or empty");

            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }
    }
}
