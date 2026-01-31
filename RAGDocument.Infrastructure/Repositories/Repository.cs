using Microsoft.EntityFrameworkCore;
using RAGDocument.Domain.Interfaces;
using RAGDocument.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RAGDocument.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _dbSet;
        public Repository(AppDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IQueryable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task UpdateAsync(T entity)
        {
            var trackedEntity = _dbSet.FindAsync(entity);
            if (trackedEntity == null)
            {
                throw new KeyNotFoundException($"{typeof(T).Name} Entity not found for update");
            }
            _db.Entry(trackedEntity).CurrentValues.SetValues(entity);
        }
    }
}
