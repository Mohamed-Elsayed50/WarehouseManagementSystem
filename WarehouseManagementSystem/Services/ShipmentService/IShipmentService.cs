using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.ViewModels;

namespace WarehouseManagementSystem.Services.ShipmentService
{
    public interface IShipmentService
    {
        Task<ResponseVM<ShipmentVM>> AddShipmentAsync(ShipmentVM model);
        Task<ResponseVM<ShipmentVM>> UpdateShipmentAsync(ShipmentVM model);
        Task DeleteShipmentAsync(Guid id);
        Task<List<shipment>> GetFilteredShipmentsAsync(DateTime? from, DateTime? to, int? number, string? resource, string? unit, string? Client);
        Task<shipment> GetByIdAsync(Guid id);
        Task MakeShipmentSigned(Guid id);
    }
}
