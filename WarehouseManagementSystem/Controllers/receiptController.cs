using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseManagementSystem.Services.ResourceService;
using WarehouseManagementSystem.Services.ReceiptService;
using WarehouseManagementSystem.Services.UnitsOfMeasurementService;
using WarehouseManagementSystem.ViewModels;
using WarehouseManagementSystem.Enums;

namespace WarehouseManagementSystem.Controllers
{
    public class receiptController : Controller
    {
        private readonly IReceiptService _receiptService;
        private readonly IResourceService _resourceService;
        private readonly IUnitsOfMeasurementService _unitsOfMeasurementService;
        private readonly ILogger<receiptController> _logger;

        public receiptController(IReceiptService receiptService, ILogger<receiptController> logger,IResourceService resourceService, IUnitsOfMeasurementService unitsOfMeasurementService)
        {
            _receiptService = receiptService;
            _logger = logger;
            _resourceService = resourceService;
            _unitsOfMeasurementService = unitsOfMeasurementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, int? number, string? resource, string? unit)
        {
            try
            {
                var receipts = await _receiptService.GetFilteredReceiptsAsync(from, to, number, resource, unit);

                // AJAX Request? => Return Partial View
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_FilterReceptsPartial", receipts);
                }

                return View(receipts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка приходов.");
                return StatusCode(500, "Произошла ошибка при загрузке приходов.");
            }
        }


        public async Task<IActionResult> Add()
        {
            var model = new ReceiptVM
            {
                ResourceList = new SelectList(await _resourceService.GetAllResourcesAsync(0), "Id", "Name"),
                UnitList = new SelectList(await _unitsOfMeasurementService.GetAllUnitsOfMeasurementsAsync(0), "Id", "Name"),
                Date = DateTime.Now
            };
            return PartialView("_AddEditPartial", model);
        }

        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guid))
                    return BadRequest("Неверный идентификатор.");

                var receipt = await _receiptService.GetByIdAsync(guid);
                if (receipt == null) 
                    return NotFound("Приход не найден.");

                var vm = new ReceiptVM(receipt);
                vm.UnitList = new SelectList(await _unitsOfMeasurementService.GetAllUnitsOfMeasurementsAsync(0), "Id", "Name");
                vm.ResourceList = new SelectList(await _resourceService.GetAllResourcesAsync(0), "Id", "Name");

                return PartialView("_AddEditPartial", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении прихода для редактирования.");
                return StatusCode(500, "Произошла ошибка при получении данных.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAdd(ReceiptVM model)
        {
            try
            {
                ModelState.Remove("UnitList");
                ModelState.Remove("ResourceList");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, errors });
                }

                var result = await _receiptService.AddReceiptAsync(model);
                if (result.Status == ResponseStatus.Success)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, errors = new[] { result.Message } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении прихода.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при добавлении прихода." } });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitUpdate(ReceiptVM model)
        {

            try
            {
                ModelState.Remove("UnitList");
                ModelState.Remove("ResourceList");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, errors });
                }

                var result = await _receiptService.UpdateReceiptAsync(model);
                if (result.Status == ResponseStatus.Success)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, errors = new[] { result.Message } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении прихода.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при добавлении прихода." } });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guid)) 
                    return BadRequest("Неверный идентификатор.");

                await _receiptService.DeleteReceiptAsync(guid);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании/восстановлении прихода.");
                return StatusCode(500, "Произошла ошибка при обработке архивации.");
            }
        }
    }
}
