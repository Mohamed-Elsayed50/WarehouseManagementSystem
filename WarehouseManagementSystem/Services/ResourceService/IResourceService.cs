using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.ViewModels;

namespace WarehouseManagementSystem.Services.ResourceService
{
    public interface IResourceService
    {
        Task<List<Resource>> GetAllResourcesAsync(int status);
        Task<Resource?> GetByIdAsync(Guid id);
        Task<ResponseVM<Resource>> AddResourceAsync(Resource resource);
        Task<ResponseVM<Resource>> UpdateResourceAsync(Resource resource);
        Task ArchiveResourceAsync(Guid id);
        Task UnarchiveResourceAsync(Guid id);
    }

}
