using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Services.ClientService;

namespace WarehouseManagementSystem.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientService clientService, ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }
        [Route("Client/Index/{status}")]
        public async Task<IActionResult> Index([FromRoute]int status)
        {
            try
            {
                var clients = await _clientService.GetAllClientsAsync(status);
                ViewBag.status = status;
                return View(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка клиентов.");
                return StatusCode(500, "Произошла ошибка при загрузке клиентов.");
            }
        }

        public IActionResult Add()
        {
            return PartialView("_AddEditPartial", new Client());
        }

        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guid))
                    return BadRequest("Недопустимый идентификатор клиента.");

                var client = await _clientService.GetByIdAsync(guid);
                if (client == null)
                    return NotFound("Клиент не найден.");

                return PartialView("_AddEditPartial", client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении клиента для редактирования.");
                return StatusCode(500, "Произошла ошибка при получении клиента.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAdd(Client client)
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

                var result = await _clientService.AddClientAsync(client);
                if (result.Status == ResponseStatus.Success)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, errors = new[] { result.Message } });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении клиента.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при добавлении прихода." } });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitUpdate(Client client)
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

                var result = await _clientService.UpdateClientAsync(client);
                if (result.Status == ResponseStatus.Success)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, errors = new[] { result.Message } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении клиента.");
                return Json(new { success = false, errors = new[] { "Произошла ошибка при добавлении прихода." } });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Client/Archive/{id}/{status}")]
        public async Task<IActionResult> Archive(string id,int status)
        {
            try
            {
                if (!Guid.TryParse(id, out var guid))
                    return BadRequest("Недопустимый идентификатор клиента.");
                if(status == 0)
                    await _clientService.ArchiveClientAsync(guid);
                else
                    await _clientService.UnarchiveClientAsync(guid);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании клиента.");
                return StatusCode(500, "Произошла ошибка при архивировании клиента.");
            }
        }

    }
}
