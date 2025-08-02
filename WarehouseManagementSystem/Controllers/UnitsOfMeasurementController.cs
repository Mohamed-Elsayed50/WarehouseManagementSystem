using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Services.ClientService;
using WarehouseManagementSystem.Services.UnitsOfMeasurementService;

namespace WarehouseManagementSystem.Controllers
{
    public class UnitsOfMeasurementController : Controller
    {
        private readonly IUnitsOfMeasurementService _unitsOfMeasurementService;
        private readonly ILogger<ResourceController> _logger;

        public UnitsOfMeasurementController(IUnitsOfMeasurementService unitsOfMeasurementService, ILogger<ResourceController> logger)
        {
            _unitsOfMeasurementService = unitsOfMeasurementService;
            _logger = logger;
        }
        [Route("UnitsOfMeasurement/Index/{status}")]
        public async Task<IActionResult> Index([FromRoute] int status)
        {
            try
            {
                var clients = await _unitsOfMeasurementService.GetAllUnitsOfMeasurementsAsync(status);
                ViewBag.status = status;
                return View(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка Единицы измерения.");
                return StatusCode(500, "Произошла ошибка при загрузке Единицы измерения.");
            }
        }

        public IActionResult Add()
        {
            return PartialView("_AddEditPartial", new UnitsOfMeasurement());
        }

        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guid))
                    return BadRequest("Недопустимый идентификатор Единицы измерения.");

                var unitsOfMeasurement = await _unitsOfMeasurementService.GetByIdAsync(guid);
                if (unitsOfMeasurement == null)
                    return NotFound("Единицы измерения не найдена.");

                return PartialView("_AddEditPartial", unitsOfMeasurement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении Единицы измерения для редактирования.");
                return StatusCode(500, "Произошла ошибка при получении Единицы измерения.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAdd(UnitsOfMeasurement unitsOfMeasurement)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, errors });
                }

                var result = await _unitsOfMeasurementService.AddUnitsOfMeasurementAsync(unitsOfMeasurement);

                if (result.Status == ResponseStatus.Success)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, errors = new[] { result.Message } });
 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении Единицы измерения.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при добавлении Единицы измерения." } });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitUpdate(UnitsOfMeasurement unitsOfMeasurement)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, errors });
                }

                var result = await _unitsOfMeasurementService.UpdateUnitsOfMeasurementAsync(unitsOfMeasurement);
                if (result.Status == ResponseStatus.Success)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, errors = new[] { result.Message } });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении Единицы измерения.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при обновлении Единицы измерения." } });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UnitsOfMeasurement/Archive/{id}/{status}")]
        public async Task<IActionResult> Archive(string id, int status)
        {
            try
            {
                if (!Guid.TryParse(id, out var guid))
                    return BadRequest("Недопустимый идентификатор Единицы измерения.");
                if (status == 0)
                    await _unitsOfMeasurementService.ArchiveUnitsOfMeasurementAsync(guid);
                else
                    await _unitsOfMeasurementService.UnarchiveUnitsOfMeasurementAsync(guid);

                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании Единицы измерения.");
                return StatusCode(500, "Произошла ошибка при архивировании Единицы измерения.");
            }
        

        }

    }
}
