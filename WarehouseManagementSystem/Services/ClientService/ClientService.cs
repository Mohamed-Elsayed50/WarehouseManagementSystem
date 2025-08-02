using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repository.Base;
using WarehouseManagementSystem.Services.ClientService;
using WarehouseManagementSystem.ViewModels;

public class ClientService : IClientService
{
    private readonly IBaseRepository<Client> _clientRepo;

    public ClientService(IBaseRepository<Client> clientRepo)
    {
        _clientRepo = clientRepo;
    }

    public async Task<List<Client>> GetAllClientsAsync(int status)
    {
        try
        {
            if(status == 0) 
                return await _clientRepo.GetAll(x => !x.Archived);
            else    
                return await _clientRepo.GetAll(x => x.Archived);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Произошла ошибка при загрузке списка клиентов.", ex);
        }
    }

    public async Task<Client?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _clientRepo.GetFirstByConditionAsync(x => x.Id == id);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Ошибка при получении клиента по ID: {id}", ex);
        }
    }

    public async Task<ResponseVM<Client>> AddClientAsync(Client client)
    {
        try
        {
            var CheakName = await _clientRepo.GetByConditionAsync<Client>(x => x.Name == client.Name);
            if (CheakName == null)
            {
                await _clientRepo.AddAsync(client);
                await _clientRepo.SaveChangesAsync();

                return new ResponseVM<Client> {
                Status = ResponseStatus.Success,
                Data=client,
                Message= "клиента добавлен"
                };
            }
            else
            {
                return new ResponseVM<Client>
                {
                    Status = ResponseStatus.Error,
                    Data = client,
                    Message = "В системе уже зарегистрирован клиент с таким названием"
                };
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ошибка при добавлении клиента.", ex);
        }
    }

    public async Task<ResponseVM<Client>> UpdateClientAsync(Client client)
    {
        var CheakName = await _clientRepo.GetByConditionAsync<Client>(x => x.Name == client.Name && x.Id!=client.Id);
        if (CheakName == null)
        {
            await _clientRepo.UpdateAsync(client);
            await _clientRepo.SaveChangesAsync();

            return new ResponseVM<Client>
            {
                Status = ResponseStatus.Success,
                Data = client,
                Message = "клиент обновлен"
            };
        }
        else
        {
            return new ResponseVM<Client>
            {
                Status = ResponseStatus.Error,
                Data = client,
                Message = "В системе уже зарегистрирован клиент с таким названием"
            };
        }
    }

    public async Task ArchiveClientAsync(Guid id)
    {
        try
        {
            var client = await _clientRepo.GetFirstByConditionAsync(x => x.Id == id);
            if (client == null)
                throw new KeyNotFoundException("Клиент не найден.");

            client.Archived = true;
            await _clientRepo.UpdateAsync(client);
            await _clientRepo.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Произошла ошибка при архивировании клиента.", ex);
        }
    }

    public async Task UnarchiveClientAsync(Guid id)
    {
        var client = await _clientRepo.GetFirstByConditionAsync(x => x.Id == id);
        if (client == null)
            throw new Exception("Клиент не найден.");

        client.Archived = false;
        await _clientRepo.UpdateAsync(client);
        await _clientRepo.SaveChangesAsync();
    }


}