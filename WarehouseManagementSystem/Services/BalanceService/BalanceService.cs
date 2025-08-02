using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repository.Base;

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
                var balances = await _balanceService.GetListAsync(includes:x=>x.Include(x=>x.UnitOfMeasurement).Include(x=>x.resource));

                if (resourceId.HasValue)
                    balances  = balances.Where(x=>x.ResourceId==resourceId).ToList();
                if(unitId.HasValue)
                    balances = balances.Where(x=>x.UnitOfMeasurementId==unitId).ToList();

                return balances;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Произошла ошибка при загрузке списка баласов.", ex);
            }
        }

  
        public async Task AddToBalance(Guid ResourceId, Guid UnitId,decimal Quantity)
        {
            var existingBalance = await _balanceService.GetByConditionAsync<balance>(x =>x.ResourceId == ResourceId && x.UnitOfMeasurementId == UnitId);

            if (existingBalance != null)
            {
                // Update quantity
                existingBalance.Quantity += Quantity;
                await _balanceService.UpdateAsync(existingBalance);
            }
            else
            {
                // Create new balance entry
                var newBalance = new balance
                {
                    Id = Guid.NewGuid(),
                    ResourceId = ResourceId,
                    UnitOfMeasurementId = UnitId,
                    Quantity = Quantity
                };
                await _balanceService.AddAsync(newBalance);
            }

            await _balanceService.SaveChangesAsync();
        }

        public async Task DeleteFromBalance(Guid ResourceId, Guid UnitId, decimal Quantity)
        {
            var existingBalance = await _balanceService.GetByConditionAsync<balance>(x => x.ResourceId == ResourceId && x.UnitOfMeasurementId == UnitId);

            if (existingBalance != null)
            {
                // Update quantity
                existingBalance.Quantity -= Quantity;
                await _balanceService.UpdateAsync(existingBalance);
                await _balanceService.SaveChangesAsync();
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
    }
}
