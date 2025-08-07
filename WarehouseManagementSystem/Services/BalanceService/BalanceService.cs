using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repositories.BaseRepository;
using WarehouseManagementSystem.Repository.Base;
using LinqKit;

namespace WarehouseManagementSystem.Services.BalanceService
{
    public class BalanceService:IBalanceService
    {
        private readonly IBaseRepository<balance> _balanceService;
        public BalanceService(IBaseRepository<balance> balanceService)
        {
            _balanceService = balanceService;
        }

        public async Task<List<balance>> GetAllBalancesAsync(Guid? resourceId, Guid? unitId)
        {
            try
            {
                await UpdateBalance();

                var filter = PredicateBuilder.New<balance>(true);

                if (resourceId.HasValue)
                    filter = filter.And(x => x.ResourceId == resourceId);

                if(unitId.HasValue)
                    filter = filter.And(x => x.UnitOfMeasurementId == unitId);

                var balances = await _balanceService.GetListAsync(
                    where: filter,
                    includes:x=>x.Include(x=>x.UnitOfMeasurement).Include(x=>x.resource));


                return balances;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Произошла ошибка при загрузке списка баласов.", ex);
            }
        }

        public async Task UpdateBalance()
        {
            try
            {
                await _balanceService.ExecuteSqlAsync("EXEC SyncBalance");
            }
            catch (Exception ex) {
                throw new ApplicationException("Произошла ошибка при загрузке списка баласов.", ex);
            }
        }


        public async Task<decimal> GetAvailableQuantity(Guid resourceId, Guid unitId)
        {
            try
            {
                var balance = await _balanceService.GetByConditionAsync<balance>(b =>
                            b.ResourceId == resourceId && b.UnitOfMeasurementId == unitId);

                return balance?.Quantity ?? 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Произошла ошибка при загрузке списка баласов.", ex);
            }
        }


        public async Task<TResult> GetBalanceByConditionAsync<TResult>(
                      Expression<Func<balance, bool>> where = null,
                      Expression<Func<balance, TResult>> selector = null,
                      bool asNoTracking = false,
                      Func<IQueryable<balance>, IQueryable<balance>> includes = null)
        {
            return await _balanceService.GetByConditionAsync(
                where: where,
                selector: selector,
                includes: includes
            );
        }

        public async Task<(List<UnitsOfMeasurement>, List<Resource>)> GetItemsWithCount()
        {
            try
            {
                var balances = await _balanceService.GetListAsync(includes: x => x
                    .Include(x => x.UnitOfMeasurement)
                    .Include(x => x.resource));

                if (balances == null || balances.Count == 0)
                    throw new Exception("Не найдено ни одной позиции.");

                var uniqueUnits = balances
                    .Select(x => x.UnitOfMeasurement)
                    .Where(u => u != null)
                    .GroupBy(u => u.Id)
                    .Select(g => g.First())
                    .ToList();

                var uniqueResources = balances
                    .Select(x => x.resource)
                    .Where(r => r != null)
                    .GroupBy(r => r.Id)
                    .Select(g => g.First())
                    .ToList();

                return (uniqueUnits, uniqueResources);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка при получении единиц и ресурсов.", ex);
            }
        }
    }
}
