using EVCS.TriNM.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EVCS.TriNM.Repositories.Context
{
    public interface IGenericRepository<T> where T : class
    {
        // Get methods
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        T GetById(string code);
        Task<T> GetByIdAsync(string code);
        T GetById(Guid code);
        Task<T> GetByIdAsync(Guid code);
        T GetById(long id);
        Task<T> GetByIdAsync(long id);
        List<T> GetAll();
        IQueryable<T> GetAllQueryable();
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);

        // Create methods
        void Create(T entity);
        Task<int> CreateAsync(T entity);
        Task<TKey> CreateReturnKeyAsync<TKey>(T entity);
        Task<int> CreateAsyncWithCheckExist(T entity);

        // Update methods
        void Update(T entity);
        Task<int> UpdateAsync(T entity);

        // Delete methods
        bool Remove(T entity);
        Task<bool> RemoveAsync(T entity);
        Task DeleteAsync(T entity);

        // Prepare methods (without saving)
        void PrepareCreate(T entity);
        void PrepareUpdate(T entity);
        void PrepareRemove(T entity);

        // Save methods
        int Save();
        Task<int> SaveAsync();

        // Add method for adding entities
        Task AddAsync(T entity);
    }
}
