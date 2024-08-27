using BoardJob.Domain.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace BoardJob.Infrastructure.Repositories
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public TEntity Add(TEntity entity)
        {
            return _context.Set<TEntity>().Add(entity).Entity;
        }

        public TEntity Delete(TEntity entity)
        {
            return _context.Set<TEntity>().Remove(entity).Entity;
        }

        public TEntity Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity).State = EntityState.Modified;
            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetAsyncById(Guid id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
    }
}
