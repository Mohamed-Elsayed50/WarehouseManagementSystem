using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repository.Base;
using WarehouseManagementSystem.Services.UnitsOfMeasurementService;
using WarehouseManagementSystem.ViewModels;

public class UnitsOfMeasurementService : IUnitsOfMeasurementService
{
    private readonly IBaseRepository<UnitsOfMeasurement> _UnitRepo;

    public UnitsOfMeasurementService(IBaseRepository<UnitsOfMeasurement> unitRepo)
    {
        _UnitRepo = unitRepo;
    }

    public async Task<List<UnitsOfMeasurement>> GetAllUnitsOfMeasurementsAsync(int status)
    {
        try
        {
            if(status==0)
                return await _UnitRepo.GetAll(x => !x.Archived);
            else
                return await _UnitRepo.GetAll(x => x.Archived);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Произошла ошибка при загрузке списка Единицы измерения.", ex);
        }
    }

    public async Task<UnitsOfMeasurement?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _UnitRepo.GetFirstByConditionAsync(x => x.Id == id);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Ошибка при получении Единицa измерения по ID: {id}", ex);
        }
    }

    public async Task<ResponseVM<UnitsOfMeasurement>> AddUnitsOfMeasurementAsync(UnitsOfMeasurement unitsOfMeasurement)
    {
        try
        {
            var CheakName = await _UnitRepo.GetByConditionAsync<UnitsOfMeasurement>(x => x.Name == unitsOfMeasurement.Name);
            if (CheakName == null)
            {

                await _UnitRepo.AddAsync(unitsOfMeasurement);
                await _UnitRepo.SaveChangesAsync();

                return new ResponseVM<UnitsOfMeasurement>
                {
                    Status = ResponseStatus.Success,
                    Data = unitsOfMeasurement,
                    Message = "Единицa измерения добавленa"
                };
            }
            else
            {
                return new ResponseVM<UnitsOfMeasurement>
                {
                    Status = ResponseStatus.Error,
                    Data = unitsOfMeasurement,
                    Message = "В системе уже зарегистрирован Единицa измерения с таким названием"
                };
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ошибка при добавлении Единицa измерения.", ex);
        }
    }

    public async Task<ResponseVM<UnitsOfMeasurement>> UpdateUnitsOfMeasurementAsync(UnitsOfMeasurement unitsOfMeasurement)
    {
        try
        {
            var CheakName = await _UnitRepo.GetByConditionAsync<UnitsOfMeasurement>(x => x.Name == unitsOfMeasurement.Name && x.Id != unitsOfMeasurement.Id);
            if (CheakName == null)
            {
                await _UnitRepo.UpdateAsync(unitsOfMeasurement);
                await _UnitRepo.SaveChangesAsync();

                return new ResponseVM<UnitsOfMeasurement>
                {
                    Status = ResponseStatus.Success,
                    Data = unitsOfMeasurement,
                    Message = "Единицa измерения обновленa"
                };
            }
            else
            {
                return new ResponseVM<UnitsOfMeasurement>
                {
                    Status = ResponseStatus.Error,
                    Data = unitsOfMeasurement,
                    Message = "В системе уже зарегистрирован Единицa измерения с таким названием"
                };
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ошибка при обновлении Единицa измерения.", ex);
        }
    }

    public async Task ArchiveUnitsOfMeasurementAsync(Guid id)
    {
        try
        {
            var unitsOfMeasurement = await _UnitRepo.GetFirstByConditionAsync(x => x.Id == id);
            if (unitsOfMeasurement == null)
                throw new KeyNotFoundException("Единицa измерения не найден.");

            unitsOfMeasurement.Archived = true;
            await _UnitRepo.UpdateAsync(unitsOfMeasurement);
            await _UnitRepo.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Произошла ошибка при архивировании Единицa измерения.", ex);
        }
    }

    public async Task UnarchiveUnitsOfMeasurementAsync(Guid id)
    {
        var unitsOfMeasurement = await _UnitRepo.GetFirstByConditionAsync(x => x.Id == id);
        if (unitsOfMeasurement == null)
            throw new Exception("Единицa измерения не найденa.");

        unitsOfMeasurement.Archived = false;
        await _UnitRepo.UpdateAsync(unitsOfMeasurement);
        await _UnitRepo.SaveChangesAsync();
    }
}