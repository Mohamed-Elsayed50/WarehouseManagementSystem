using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Services.BalanceService
{
    public interface IBalanceService
    {
        Task<List<balance>> GetAllBalancesAsync(Guid? resourceId, Guid? unitId);
        Task AddToBalance(Guid ResourceId, Guid UnitId, decimal Quantity);
        Task DeleteFromBalance(Guid ResourceId, Guid UnitId, decimal Quantity);
        Task UpdateBalance();
    }
}
