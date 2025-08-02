using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace WarehouseManagementSystem.Repository.Base
{
    public interface IBaseRepository<T> where T : class
    {
        Task<int> ExecuteSqlAsync(string sql, params object[] parameters);
        Task<List<T>> GetListAsync(
            Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IQueryable<T>> includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            bool asNoTracking = false);
        Task<List<TResult>> GetListAsync<TResult>(
            Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IQueryable<T>> includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Expression<Func<T, TResult>> selector = null,
            bool asNoTracking = false);
        Task<IQueryable<T>> GetIQueryable(
            Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IQueryable<T>> includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            bool asNoTracking = false);
        Task<TResult> GetByConditionAsync<TResult>(
            Expression<Func<T, bool>> where = null,
            Expression<Func<T, TResult>> selector = null,
            bool asNoTracking = false,
            Func<IQueryable<T>, IQueryable<T>> includes = null);
        Task<int> CountAsync(Expression<Func<T, bool>> where = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> where = null);
        Task<TKey?> MaxAsync<TKey>(Expression<Func<T, TKey>> selector, Expression<Func<T, bool>> predicate = null)
            where TKey : struct;
        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(List<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(List<T> entities);
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<TResult?> GetMaxByConditionAsync<TResult>(
    Expression<Func<T, bool>>? wherePredicate,
    Expression<Func<T, TResult>> maxSelector)
            where TResult : struct;
        Task<List<T>> GetAll(
                    Expression<Func<T, bool>>? filter = null,
                    params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetAllAsNoTracking(
                    Expression<Func<T, bool>>? filter = null,
                    params Expression<Func<T, object>>[] includes);
        Task<T?> GetFirstByConditionAsync(Expression<Func<T, bool>> wherePredicate,
                    params Expression<Func<T, object>>[] includes);

    }
}
