using Azure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repository.Base;
using WarehouseManagementSystem.Services.BalanceService;
using WarehouseManagementSystem.ViewModels;
using LinqKit;

namespace WarehouseManagementSystem.Services.ReceiptService
{
    public class ReceiptService:IReceiptService
    {


        private readonly IBaseRepository<Receipt> _receiptRepo;
        private readonly IBaseRepository<ReceiptItem> _itemRepo;
        private readonly IBalanceService _balanceService;

        public ReceiptService(IBaseRepository<Receipt> receiptRepo, IBaseRepository<ReceiptItem> itemRepo, IBalanceService balanceService)
        {
            _receiptRepo = receiptRepo;
            _itemRepo = itemRepo;
            _balanceService = balanceService;
        }

        public async Task<List<Receipt>> GetAllReceiptsAsync()
        {
            try
            {
                return await _receiptRepo.GetListAsync(x=>!x.IsDeleted,includes: x=>x.Include(x=>x.Items).ThenInclude(x=>x.Resource).Include(x=>x.Items).ThenInclude(x=>x.Unit));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка при получении списка приходов.", ex);
            }
        }
        public async Task<List<Receipt>> GetFilteredReceiptsAsync(DateTime? from, DateTime? to, int? number, string? resource, string? unit)
        {
            var filter =PredicateBuilder.New<Receipt> (x=>!x.IsDeleted);


            if (from.HasValue)
                filter = filter.And(x => x.Date >= from.Value);

            if (to.HasValue)
                filter = filter.And(x => x.Date <= to.Value);

            if (number.HasValue)
                filter = filter.And(x => x.Number == number.Value);

            if (!string.IsNullOrEmpty(resource))
                filter = filter.And(x => x.Items.Any(i => i.Resource.Name == resource));

            if (!string.IsNullOrEmpty(unit))
                filter = filter.And(x => x.Items.Any(i => i.Unit.Name == unit));

            var receipts = await _receiptRepo.GetListAsync(
                where: filter,
                includes: q => q
                    .Include(x => x.Items.Where(i=>( string.IsNullOrEmpty(resource) || i.Resource.Name == resource) && (string.IsNullOrEmpty(unit) || i.Unit.Name == unit)))
                    .ThenInclude(i => i.Resource)
                    .Include(x => x.Items.Where(i=>(string.IsNullOrEmpty(resource)||i.Resource.Name ==resource) && (string.IsNullOrEmpty(unit) || i.Unit.Name == unit)))
                    .ThenInclude(i => i.Unit),
                orderBy: q => q.OrderBy(x => x.Number)
            );


            return receipts;
        }



        public async Task<Receipt?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _receiptRepo.GetByConditionAsync<Receipt>(x => x.Id == id, includes: x => x.Include(r => r.Items).ThenInclude(i => i.Resource)
                                               .Include(r => r.Items).ThenInclude(i => i.Unit));
                 
                                    
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при получении прихода по ID: {id}", ex);
            }
        }

        public async Task<ResponseVM<ReceiptVM>> AddReceiptAsync(ReceiptVM model)
        {
            using var transaction = await _receiptRepo.BeginTransactionAsync();
            try
            {
                var NumberIsExist = await _receiptRepo.GetByConditionAsync<Receipt>(x=>x.Number== model.Number);
                if (NumberIsExist != null)
                {
                    return new ResponseVM<ReceiptVM>
                    {
                        Data = model,
                        Status = ResponseStatus.Error,
                        Message = "В системе уже зарегистрирована накладная с таким номером"

                    };
                }
                var receipt = new Receipt
                {
                    Id = Guid.NewGuid(),
                    Number = model.Number,
                    Date = model.Date,
                    Items = model.Items.Select(i => new ReceiptItem
                    {
                        Id = Guid.NewGuid(),
                        ResourceId = i.ResourceId,
                        UnitId = i.UnitId,
                        Quantity = i.Quantity
                    }).ToList()
                };
                await _receiptRepo.AddAsync(receipt);
                await _receiptRepo.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseVM<ReceiptVM>
                {
                    Data = model,
                    Status = ResponseStatus.Success,
                    Message = "Поступление успешно добавлено."

                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Ошибка при добавлении прихода.", ex);
            }
        }

        public async Task<ResponseVM<ReceiptVM>> UpdateReceiptAsync(ReceiptVM model)
        {
            using var transaction = await _receiptRepo.BeginTransactionAsync();
            try
            {

                var existing = await _receiptRepo.GetByConditionAsync<Receipt>(
                    x => x.Id == model.Id,
                    includes: x => x.Include(r => r.Items)
                );

                if (existing == null)
                    throw new Exception("Приход не найден.");


                existing.Number = model.Number;
                existing.Date = model.Date;

                await _itemRepo.DeleteRangeAsync(existing.Items);
      

                var newItems = model.Items.Select(i => new ReceiptItem
                {
                    Id = Guid.NewGuid(),
                    ReceiptId = existing.Id,
                    ResourceId = i.ResourceId,
                    UnitId = i.UnitId,
                    Quantity = i.Quantity
                }).ToList();

                await _itemRepo.AddRangeAsync(newItems);
                await _receiptRepo.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ResponseVM<ReceiptVM>
                {
                    Data = model,
                    Status = ResponseStatus.Success,
                    Message = "Поступление успешно добавлено."

                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task DeleteReceiptAsync(Guid id)
        {
            try
            {
                var receipt = await _receiptRepo.GetFirstByConditionAsync(x => x.Id == id);
                if (receipt == null) 
                    throw new Exception("Приход не найден.");

                receipt.IsDeleted = true;
                await _receiptRepo.UpdateAsync(receipt);
                await _receiptRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка при архивировании прихода.", ex);
            }
        }




    }
}
