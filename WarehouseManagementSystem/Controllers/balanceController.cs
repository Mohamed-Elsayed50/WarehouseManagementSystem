using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Services.BalanceService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WarehouseManagementSystem.Controllers
{
    public class balanceController : Controller
    {
        private readonly ILogger<balanceController> _logger;
        private readonly IBalanceService _balanceService;
        public balanceController(ILogger<balanceController> logger, IBalanceService balanceService)
        {
            _logger = logger;
            _balanceService = balanceService;
        }


        public async Task<IActionResult> Index(Guid? resource, Guid? unit)
        {
            try
            {
                var Balances = await _balanceService.GetAllBalancesAsync(resource, unit);

                // AJAX Request? => Return Partial View
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_FilterBalancesPartial", Balances);
                }

                return View(Balances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка приходов.");
                return StatusCode(500, "Произошла ошибка при загрузке приходов.");
            }
        }
    }
}
