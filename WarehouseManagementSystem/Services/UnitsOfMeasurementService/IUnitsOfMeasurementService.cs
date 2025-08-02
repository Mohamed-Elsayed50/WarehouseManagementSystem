using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.ViewModels;

namespace WarehouseManagementSystem.Services.UnitsOfMeasurementService
{
    public interface IUnitsOfMeasurementService
    {
        Task<List<UnitsOfMeasurement>> GetAllUnitsOfMeasurementsAsync(int status);
        Task<UnitsOfMeasurement?> GetByIdAsync(Guid id);
        Task<ResponseVM<UnitsOfMeasurement>> AddUnitsOfMeasurementAsync(UnitsOfMeasurement unitsOfMeasurement);
        Task<ResponseVM<UnitsOfMeasurement>> UpdateUnitsOfMeasurementAsync(UnitsOfMeasurement unitsOfMeasurement);
        Task ArchiveUnitsOfMeasurementAsync(Guid id);
        Task UnarchiveUnitsOfMeasurementAsync(Guid id);
    }

}
