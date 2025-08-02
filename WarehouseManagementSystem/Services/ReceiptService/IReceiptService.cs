using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.ViewModels;

namespace WarehouseManagementSystem.Services.ReceiptService
{
    public interface IReceiptService
    {
        Task<List<Receipt>> GetAllReceiptsAsync();
        Task<Receipt?> GetByIdAsync(Guid id);
        Task<ResponseVM<ReceiptVM>> AddReceiptAsync(ReceiptVM model);
        Task<ResponseVM<ReceiptVM>> UpdateReceiptAsync(ReceiptVM model);
        Task DeleteReceiptAsync(Guid id);
        Task<List<Receipt>> GetFilteredReceiptsAsync(DateTime? from, DateTime? to, int? number, string? resource, string? unit);
        Task<(List<UnitsOfMeasurement>, List<Resource>)> GetItemsWithCount();
    }
}
