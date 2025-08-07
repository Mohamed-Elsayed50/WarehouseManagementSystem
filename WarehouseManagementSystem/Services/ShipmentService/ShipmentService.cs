using LinqKit;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repository.Base;
using WarehouseManagementSystem.Services.BalanceService;
using WarehouseManagementSystem.ViewModels;

namespace WarehouseManagementSystem.Services.ShipmentService
{
    public class ShipmentService : IShipmentService
    {
        private readonly IBaseRepository<shipment> _shipmentRepo;
        private readonly IBaseRepository<shipmentItems> _itemRepo;
        private readonly IBalanceService _balanceService;

        public ShipmentService(IBaseRepository<shipment> shipmentRepo, IBaseRepository<shipmentItems> itemRepo, IBalanceService balanceService)
        {
            _shipmentRepo = shipmentRepo;
            _itemRepo = itemRepo;
            _balanceService = balanceService;
        }

        public async Task<List<shipment>> GetFilteredShipmentsAsync(DateTime? from, DateTime? to, int? number, string? resource, string? unit,string? Client)
        {
            await _balanceService.UpdateBalance();

            var filter = PredicateBuilder.New<shipment>(x => !x.IsDeleted);

            if (from.HasValue)
                filter = filter.And(x => x.Date >= from.Value);

            if (to.HasValue)
                filter = filter.And(x => x.Date <= to.Value);

            if (number.HasValue)
                filter = filter.And(x => x.Number >= number.Value);

            if (!string.IsNullOrEmpty(resource))
                filter = filter.And(x => x.Items.Any(x=>x.Resource.Name == resource));

            if (!string.IsNullOrEmpty(unit))
                filter = filter.And(x => x.Items.Any(x => x.Unit.Name == unit));

            if(!string.IsNullOrEmpty(Client))
                filter = filter.And(x => x.client.Id.ToString()==Client);

            var shipments = await _shipmentRepo.GetListAsync(
              where: filter,
              includes: q => q
                  .Include(s => s.Items.Where(i =>(string.IsNullOrEmpty(resource) || i.Resource.Name == resource) && (string.IsNullOrEmpty(unit) || i.Unit.Name == unit)))
                      .ThenInclude(i => i.Resource)
                      .Include(s => s.Items.Where(i =>(string.IsNullOrEmpty(resource) || i.Resource.Name == resource) && (string.IsNullOrEmpty(unit) || i.Unit.Name == unit)))
                      .ThenInclude(i => i.Unit)
                  .Include(s => s.client),
              orderBy: x => x.OrderBy(s => s.Number)
          );
            return shipments;
        }

        public async Task<shipment?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _shipmentRepo.GetByConditionAsync<shipment>(
                    x => x.Id == id,
                    includes: x => x
                        .Include(s => s.Items).ThenInclude(i => i.Resource)
                        .Include(s => s.Items).ThenInclude(i => i.Unit)
                        .Include(s => s.client)
                );
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при получении отгрузки по ID: {id}", ex);
            }
        }

        public async Task<ResponseVM<ShipmentVM>> AddShipmentAsync(ShipmentVM model)
        {
            using var transaction = await _shipmentRepo.BeginTransactionAsync();
            try
            {


                var numberExists = await _shipmentRepo.GetByConditionAsync<shipment>(x => x.Number == model.Number);
                if (numberExists != null)
                {
                    return new ResponseVM<ShipmentVM>
                    {
                        Data = model,
                        Status = ResponseStatus.Error,
                        Message = "В системе уже зарегистрирована накладная с таким номером"
                    };
                }


                foreach (var item in model.Items)
                {
                    var availableQuantity = await _balanceService.GetBalanceByConditionAsync<balance>(x =>
                        x.ResourceId == item.ResourceId &&
                        x.UnitOfMeasurementId == item.UnitId,includes:x=>x.Include(x=>x.UnitOfMeasurement).Include(x=>x.resource));

                    if (availableQuantity == null || availableQuantity.Quantity < item.Quantity)
                    {
            
                        return new ResponseVM<ShipmentVM>
                        {
                            Data = model,
                            Status = ResponseStatus.Error,
                            Message = $"Недостаточное количество для {availableQuantity?.resource.Name} ({availableQuantity?.UnitOfMeasurement.Name}). " +
                                       $"Доступно: {availableQuantity?.Quantity ?? 0}, Запрошено: {item.Quantity}"
                        };
                    }
                }

                var shipment = new shipment
                {
                    Id = Guid.NewGuid(),
                    Number = model.Number,
                    Date = model.Date,
                    ClientId = model.ClientId,
                    Items = model.Items.Select(i => new shipmentItems
                    {
                        Id = Guid.NewGuid(),
                        ResourceId = i.ResourceId,
                        UnitId = i.UnitId,
                        Quantity = i.Quantity
                    }).ToList()
                };

                await _shipmentRepo.AddAsync(shipment);
                await _shipmentRepo.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ResponseVM<ShipmentVM>
                {
                    Data = model,
                    Status = ResponseStatus.Success,
                    Message = "Отгрузка успешно добавлена."
                };
              
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Ошибка при добавлении отгрузки.", ex);
            }
        }

        public async Task<ResponseVM<ShipmentVM>> UpdateShipmentAsync(ShipmentVM model)
        {
            using var transaction = await _shipmentRepo.BeginTransactionAsync();
            try
            {
                var existing = await _shipmentRepo.GetByConditionAsync<shipment>(
                    x => x.Id == model.Id,
                    includes: x => x.Include(s => s.Items)
                );

                if (existing == null)
                    throw new Exception("Отгрузка не найдена.");

                existing.Number = model.Number;
                existing.Date = model.Date;
                existing.ClientId = model.ClientId;


                await _itemRepo.DeleteRangeAsync(existing.Items);

                foreach (var item in model.Items)
                {
                    var availableQuantity = await _balanceService.GetBalanceByConditionAsync<balance>(x =>
                        x.ResourceId == item.ResourceId &&
                        x.UnitOfMeasurementId == item.UnitId, includes: x => x.Include(x => x.UnitOfMeasurement).Include(x => x.resource));

                    if (availableQuantity == null || availableQuantity.Quantity < item.Quantity)
                    {

                        return new ResponseVM<ShipmentVM>
                        {
                            Data = model,
                            Status = ResponseStatus.Error,
                            Message = $"Недостаточное количество для {availableQuantity?.resource.Name} ({availableQuantity?.UnitOfMeasurement.Name}). " +
                                       $"Доступно: {availableQuantity?.Quantity ?? 0}, Запрошено: {item.Quantity}"
                        };
                    }
                }
                var newItems = model.Items.Select(i => new shipmentItems
                {
                    Id = Guid.NewGuid(),
                    shipmentId = existing.Id,
                    ResourceId = i.ResourceId,
                    UnitId = i.UnitId,
                    Quantity = i.Quantity
                }).ToList();

                await _itemRepo.AddRangeAsync(newItems);
                await _shipmentRepo.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ResponseVM<ShipmentVM>
                {
                    Data = model,
                    Status = ResponseStatus.Success,
                    Message = "Отгрузка успешно обновлена."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteShipmentAsync(Guid id)
        {
            try
            {
                var shipment = await _shipmentRepo.GetFirstByConditionAsync(x => x.Id == id);
                if (shipment == null)
                    throw new Exception("Отгрузка не найдена.");

                shipment.IsDeleted = true;
                await _shipmentRepo.UpdateAsync(shipment);
                await _shipmentRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка при архивировании отгрузки.", ex);
            }
        }

        public async Task MakeShipmentSigned(Guid id)
        {
            try
            {
                var shipment = await _shipmentRepo.GetFirstByConditionAsync(x => x.Id == id);
                if (shipment == null)
                    throw new Exception("Отгрузка не найдена.");

                shipment.IsSigned = !shipment.IsSigned;
                await _shipmentRepo.UpdateAsync(shipment);
                await _shipmentRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка при архивировании отгрузки.", ex);
            }
        }

    }
}