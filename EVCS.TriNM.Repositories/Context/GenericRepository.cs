using EVCS.TriNM.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EVCS.TriNM.Repositories.Context
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected EVChargingDBContext _context;

        public GenericRepository()
        {
            _context ??= new EVChargingDBContext();
        }

        public GenericRepository(EVChargingDBContext context)
        {
            _context = context;
        }

        public async Task<TKey> CreateReturnKeyAsync<TKey>(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();

            var keyProperty = _context.Entry(entity).Metadata.FindPrimaryKey()?.Properties[0];
            if (keyProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have a primary key defined.");

            var keyValue = _context.Entry(entity).Property(keyProperty.Name).CurrentValue;
            return (TKey)Convert.ChangeType(keyValue, typeof(TKey));
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public IQueryable<T> GetAllQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Set<T>().AsQueryable();
            return await GetPagedResultAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize)
        {
            var query = _context.Set<T>().Where(predicate);
            return await GetPagedResultAsync(query, pageNumber, pageSize);
        }

        protected async Task<PagedResult<T>> GetPagedResultAsync(IQueryable<T> query, int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, Math.Min(100, pageSize));

            var totalCount = await query.CountAsync();
            
        
            var entityType = _context.Model.FindEntityType(typeof(T));
            if (entityType != null)
            {
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey != null && primaryKey.Properties.Count > 0)
                {
                    var keyProperty = primaryKey.Properties[0];
                    var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
                    var property = System.Linq.Expressions.Expression.Property(parameter, keyProperty.Name);
                    var lambda = System.Linq.Expressions.Expression.Lambda(property, parameter);
                    var orderByMethod = typeof(Queryable).GetMethods()
                        .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(T), keyProperty.ClrType);
                    query = (IQueryable<T>)orderByMethod.Invoke(null, new object[] { query, lambda });
                }
            }
            
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public void Create(T entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }

        public async Task<int> CreateAsync(T entity)
        {
            _context.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CreateAsyncWithCheckExist(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var keyProperty = _context.Entry(entity).Metadata.FindPrimaryKey()?.Properties[0];
            if (keyProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have a primary key defined.");

            var keyValue = _context.Entry(entity).Property(keyProperty.Name).CurrentValue;
            var existingEntity = await _context.Set<T>().FindAsync(keyValue);
            if (existingEntity != null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} with key {keyValue} already exists.");

            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public bool Remove(T entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public T GetById(string code)
        {
            return _context.Set<T>().Find(code);
        }

        public async Task<T> GetByIdAsync(string code)
        {
            return await _context.Set<T>().FindAsync(code);
        }

        public T GetById(Guid code)
        {
            return _context.Set<T>().Find(code);
        }

        public async Task<T> GetByIdAsync(Guid code)
        {
            return await _context.Set<T>().FindAsync(code);
        }

        public T GetById(long id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void PrepareCreate(T entity)
        {
            _context.Add(entity);
        }

        public void PrepareUpdate(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
        }

        public void PrepareRemove(T entity)
        {
            _context.Remove(entity);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
    }
}
