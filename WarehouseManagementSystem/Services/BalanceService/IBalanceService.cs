using System.Linq.Expressions;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Services.BalanceService
{
    public interface IBalanceService
    {
        Task<List<balance>> GetAllBalancesAsync(Guid? resourceId, Guid? unitId);
        Task UpdateBalance();
        Task<decimal> GetAvailableQuantity(Guid resourceId, Guid unitId);

       Task<TResult> GetBalanceByConditionAsync<TResult>(
              Expression<Func<balance, bool>> where = null,
              Expression<Func<balance, TResult>> selector = null,
              bool asNoTracking = false,
              Func<IQueryable<balance>, IQueryable<balance>> includes = null);

        Task<(List<UnitsOfMeasurement>, List<Resource>)> GetItemsWithCount();
    }
}
