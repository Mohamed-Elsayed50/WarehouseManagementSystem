using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.ViewModels;

namespace WarehouseManagementSystem.Services.ClientService
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync(int status);
        Task<Client?> GetByIdAsync(Guid id);
        Task<ResponseVM<Client>> AddClientAsync(Client client);
        Task<ResponseVM<Client>> UpdateClientAsync(Client client);
        Task ArchiveClientAsync(Guid id);
        Task UnarchiveClientAsync(Guid id);
    }

}
