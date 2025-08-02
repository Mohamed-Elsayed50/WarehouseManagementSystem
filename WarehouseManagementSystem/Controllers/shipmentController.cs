using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Services.ClientService;
using WarehouseManagementSystem.Services.ReceiptService;
using WarehouseManagementSystem.Services.ResourceService;
using WarehouseManagementSystem.Services.ShipmentService;
using WarehouseManagementSystem.Services.UnitsOfMeasurementService;
using WarehouseManagementSystem.ViewModels;

namespace WarehouseManagementSystem.Controllers
{
    public class shipmentController : Controller
    {
        private readonly IShipmentService _shipmentService;
        private readonly IClientService _clientService;
        private readonly IResourceService _resourceService;
        private readonly IReceiptService _receiptService;
        private readonly IUnitsOfMeasurementService _unitsService;
        private readonly ILogger<shipmentController> _logger;

        public shipmentController(IShipmentService shipmentService,
                                   IClientService clientService,
                                   IResourceService resourceService,
                                   IUnitsOfMeasurementService unitsService,
                                   ILogger<shipmentController> logger,
                                   IReceiptService receiptService)
        {
            _shipmentService = shipmentService;
            _clientService = clientService;
            _resourceService = resourceService;
            _unitsService = unitsService;
            _logger = logger;
            _receiptService = receiptService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, int? number, string? resource, string? unit,string? Client)
        {
            try
            {
                var shipments = await _shipmentService.GetFilteredShipmentsAsync(from, to, number, resource, unit, Client);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_FiltershipmentPartial", shipments);

                return View(shipments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка отгрузок.");
                return StatusCode(500, "Произошла ошибка при загрузке отгрузок.");
            }
        }

        public async Task<IActionResult> Add()
        {
            var Items = await _receiptService.GetItemsWithCount();
            var model = new ShipmentVM
            {
                UnitList = new SelectList(Items.Item1, "Id", "Name"),
                ResourceList = new SelectList(Items.Item2, "Id", "Name"),
                ClientList = new SelectList(await _clientService.GetAllClientsAsync(0), "Id", "Name"),
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

                var shipment = await _shipmentService.GetByIdAsync(guid);
                if (shipment == null)
                    return NotFound("Отгрузка не найдена.");

                var vm = new ShipmentVM(shipment)
                {
                    ResourceList = new SelectList(await _resourceService.GetAllResourcesAsync(0), "Id", "Name"),
                    UnitList = new SelectList(await _unitsService.GetAllUnitsOfMeasurementsAsync(0), "Id", "Name"),
                    ClientList = new SelectList(await _clientService.GetAllClientsAsync(0), "Id", "Name")
                };

                return PartialView("_AddEditPartial", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении отгрузки для редактирования.");
                return StatusCode(500, "Произошла ошибка при получении данных.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAdd(ShipmentVM model)
        {
            try
            {
                ModelState.Remove("UnitList");
                ModelState.Remove("ResourceList");
                ModelState.Remove("ClientList");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, errors });
                }

                var result = await _shipmentService.AddShipmentAsync(model);
                if (result.Status == ResponseStatus.Success)
                    return Json(new { success = true });

                return Json(new { success = false, errors = new[] { result.Message } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении отгрузки.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при добавлении отгрузки." } });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitUpdate(ShipmentVM model)
        {
            try
            {
                ModelState.Remove("UnitList");
                ModelState.Remove("ResourceList");
                ModelState.Remove("ClientList");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, errors });
                }

                var result = await _shipmentService.UpdateShipmentAsync(model);
                if (result.Status == ResponseStatus.Success)
                    return Json(new { success = true });

                return Json(new { success = false, errors = new[] { result.Message } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении отгрузки.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при обновлении отгрузки." } });
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

                await _shipmentService.DeleteShipmentAsync(guid);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании отгрузки.");
                return StatusCode(500, "Произошла ошибка при архивировании.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeShipmentsigned(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guid))
                    return BadRequest("Неверный идентификатор.");

                await _shipmentService.MakeShipmentSigned(guid);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании отгрузки.");
                return StatusCode(500, "Произошла ошибка при архивировании.");
            }
        }

    }
}