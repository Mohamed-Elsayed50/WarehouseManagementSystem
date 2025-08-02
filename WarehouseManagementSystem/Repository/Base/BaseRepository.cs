using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repository.Base;

namespace WarehouseManagementSystem.Repositories.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<int> ExecuteSqlAsync(string sql, params object[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<List<T>> GetListAsync(
            Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IQueryable<T>> includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            bool asNoTracking = false)
        {
            IQueryable<T> query = _dbSet;
            if (asNoTracking)
                query = query.AsNoTracking();
            if (where != null)
                query = query.Where(where);
            if (includes != null)
                query = includes(query);
            if (orderBy != null)
                query = orderBy(query);
            return await query.ToListAsync();
        }
        public async Task<List<TResult>> GetListAsync<TResult>(
            Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IQueryable<T>> includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Expression<Func<T, TResult>> selector = null,
            bool asNoTracking = false)
        {
            IQueryable<T> query = _dbSet;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (where != null)
                query = query.Where(where);

            if (includes != null)
                query = includes(query);

            if (orderBy != null)
                query = orderBy(query);

            if (selector != null)
                return await query.Select(selector).ToListAsync();
            else
                throw new ArgumentNullException(nameof(selector), "Selector cannot be null when using GetListAsync with TResult");
        }

        public async Task<IQueryable<T>> GetIQueryable(
            Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IQueryable<T>> includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            bool asNoTracking = false)
        {
            IQueryable<T> query = _dbSet;
            if (asNoTracking)
                query = query.AsNoTracking();
            if (where != null)
                query = query.Where(where);
            if (includes != null)
                query = includes(query);
            if (orderBy != null)
                query = orderBy(query);
            return query;
        }
        public async Task<TResult> GetByConditionAsync<TResult>(
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, TResult>> selector = null,
            bool asNoTracking = false,
            Func<IQueryable<T>, IQueryable<T>> includes = null)
        {
            IQueryable<T> query = _dbSet;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (includes != null)
                query = includes(query);
            if (predicate != null)
                query = query.Where(predicate);

            if (selector != null)
                return await query.Select(selector).FirstOrDefaultAsync();

            return (TResult)(object)(await query.FirstOrDefaultAsync());
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            return predicate == null
                ? await query.AnyAsync()
                : await query.AnyAsync(predicate);
        }
        public async Task<TKey?> MaxAsync<TKey>(Expression<Func<T, TKey>> selector, Expression<Func<T, bool>> predicate = null) where TKey : struct
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
                query = query.Where(predicate);

            if (!await query.AnyAsync())
                return null;

            return await query.MaxAsync(selector);
        }


        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public async Task AddRangeAsync(List<T> entities) => await _dbSet.AddRangeAsync(entities);

        public async Task UpdateAsync(T entity) => _dbSet.Update(entity);
        public async Task UpdateRangeAsync(List<T> entities) => _dbSet.UpdateRange(entities);


        public async Task DeleteAsync(T entity) => _dbSet.Remove(entity);
        public async Task DeleteRangeAsync(List<T> entities) => _dbSet.RemoveRange(entities);


        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();


        public async Task<IDbContextTransaction> BeginTransactionAsync()
            => await _context.Database.BeginTransactionAsync();
        public async Task<TResult?> GetMaxByConditionAsync<TResult>(
    Expression<Func<T, bool>>? wherePredicate,
    Expression<Func<T, TResult>> maxSelector)
    where TResult : struct
        {
            IQueryable<T> query = _context.Set<T>();

            if (wherePredicate != null)
                query = query.Where(wherePredicate);

            return await query.Select(maxSelector).MaxAsync();
        }
        public async Task<List<T>> GetAll(
                    Expression<Func<T, bool>>? filter = null,
                    params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();


            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.ToListAsync();
        }
        public async Task<List<T>> GetAllAsNoTracking(
                   Expression<Func<T, bool>>? filter = null,
                   params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }


            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.ToListAsync();
        }
        public async Task<T?> GetFirstByConditionAsync(
    Expression<Func<T, bool>>? wherePredicate = null,
    params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();


            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (wherePredicate != null)
            {
                return await query.FirstOrDefaultAsync(wherePredicate);
            }


            return await query.FirstOrDefaultAsync();
        }

    }
}