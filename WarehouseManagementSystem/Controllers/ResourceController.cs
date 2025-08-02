using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Services.ResourceService;
using WarehouseManagementSystem.Services.UnitsOfMeasurementService;

namespace WarehouseManagementSystem.Controllers
{
    public class ResourceController : Controller
    {
        private readonly IResourceService _resourceService;
        private readonly ILogger<ResourceController> _logger;

        public ResourceController(IResourceService resourceService, ILogger<ResourceController> logger)
        {
            _resourceService = resourceService;
            _logger = logger;
        }
        [Route("Resource/Index/{status}")]
        public async Task<IActionResult> Index([FromRoute] int status)
        {
            try
            {
                var clients = await _resourceService.GetAllResourcesAsync(status);
                ViewBag.status = status;
                return View(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка Ресурсов.");
                return StatusCode(500, "Произошла ошибка при загрузке Ресурсов.");
            }
        }

        public IActionResult Add()
        {
            return PartialView("_AddEditPartial", new Resource());
        }

        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guid))
                    return BadRequest("Недопустимый идентификатор Ресурсa.");

                var client = await _resourceService.GetByIdAsync(guid);
                if (client == null)
                    return NotFound("Ресурсa не найден.");

                return PartialView("_AddEditPartial", client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении Ресурсa для редактирования.");
                return StatusCode(500, "Произошла ошибка при получении Ресурсa.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAdd(Resource resource)
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

                var result = await _resourceService.AddResourceAsync(resource); 

                if (result.Status == ResponseStatus.Success)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, errors = new[] { result.Message } });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении Ресурсa.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при добавлении Ресурсa." } });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitUpdate(Resource resource)
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

                var result =  await _resourceService.UpdateResourceAsync(resource);
                if (result.Status == ResponseStatus.Success)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, errors = new[] { result.Message } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении Ресурсa.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при обновлении Ресурсa." } });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Resource/Archive/{id}/{status}")]
        public async Task<IActionResult> Archive(string id,int status)
        {
            try
            {
                if (!Guid.TryParse(id, out var guid))
                    return BadRequest("Недопустимый идентификатор Ресурсa.");

                if (status == 0)
                    await _resourceService.ArchiveResourceAsync(guid);
                else
                    await _resourceService.UnarchiveResourceAsync(guid);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании Ресурсa.");
                return StatusCode(500, "Произошла ошибка при архивировании Ресурсa.");
            }
        }

    }
}
