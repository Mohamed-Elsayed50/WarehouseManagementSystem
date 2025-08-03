using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repository.Base;
using WarehouseManagementSystem.Services.BalanceService;
using WarehouseManagementSystem.Services.ResourceService;
using WarehouseManagementSystem.ViewModels;

public class ResourceService : IResourceService
{
    private readonly IBaseRepository<Resource> _ResourceRepo;
    private readonly IBalanceService _balanceService;
    public ResourceService(IBaseRepository<Resource> ResourceRepo, IBalanceService balanceService)
    {
        _ResourceRepo = ResourceRepo;
        _balanceService = balanceService;
    }

    public async Task<List<Resource>> GetAllResourcesAsync(int status)
    {
        try
        {
            await _balanceService.UpdateBalance();
            if (status == 0) 
                return await _ResourceRepo.GetAll(x => !x.Archived);
            else
                return await _ResourceRepo.GetAll(x => x.Archived);


        }
        catch (Exception ex)
        {
            throw new ApplicationException("Произошла ошибка при загрузке списка Ресурсов.", ex);
        }
    }

    public async Task<Resource?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _ResourceRepo.GetFirstByConditionAsync(x => x.Id == id);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Ошибка при получении Ресурсa по ID: {id}", ex);
        }
    }

    public async Task<ResponseVM<Resource>> AddResourceAsync(Resource resource)
    {
        try
        {
            var CheakName = await _ResourceRepo.GetByConditionAsync<Resource>(x => x.Name == resource.Name);
            if (CheakName == null)
            {

                await _ResourceRepo.AddAsync(resource);
                await _ResourceRepo.SaveChangesAsync();

                return new ResponseVM<Resource>
                {
                    Status = ResponseStatus.Success,
                    Data = resource,
                    Message = "Ресурсa добавленa"
                };
            }
            else
            {
                return new ResponseVM<Resource>
                {
                    Status = ResponseStatus.Error,
                    Data = resource,
                    Message = "В системе уже зарегистрирован Ресурсa с таким названием"
                };
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ошибка при добавлении Ресурсa.", ex);
        }
    }

    public async Task<ResponseVM<Resource>> UpdateResourceAsync(Resource resource)
    {
        try
        {
            var CheakName = await _ResourceRepo.GetByConditionAsync<Resource>(x => x.Name == resource.Name && x.Id != resource.Id);
            if (CheakName == null)
            {
                await _ResourceRepo.UpdateAsync(resource);
                await _ResourceRepo.SaveChangesAsync();

                return new ResponseVM<Resource>
                {
                    Status = ResponseStatus.Success,
                    Data = resource,
                    Message = "Ресурсa обновленa"
                };
            }
            else
            {
                return new ResponseVM<Resource>
                {
                    Status = ResponseStatus.Error,
                    Data = resource,
                    Message = "В системе уже зарегистрирован Ресурсa с таким названием"
                };
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ошибка при обновлении Ресурсa.", ex);
        }
    }

    public async Task ArchiveResourceAsync(Guid id)
    {
        try
        {
            var resource = await _ResourceRepo.GetFirstByConditionAsync(x => x.Id == id);
            if (resource == null)
                throw new KeyNotFoundException("Ресурс не найден.");

            resource.Archived = true;
            await _ResourceRepo.UpdateAsync(resource);
            await _ResourceRepo.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Произошла ошибка при архивировании Ресурсa.", ex);
        }
    }

    public async Task UnarchiveResourceAsync(Guid id)
    {
        var resource = await _ResourceRepo.GetFirstByConditionAsync(x => x.Id == id);
        if (resource == null)
            throw new Exception("Ресурс не найден.");

        resource.Archived = false;
        await _ResourceRepo.UpdateAsync(resource);
        await _ResourceRepo.SaveChangesAsync();
    }
}