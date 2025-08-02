using Microsoft.Extensions.DependencyInjection;
using WarehouseManagementSystem.Services.ClientService;
using WarehouseManagementSystem.Repository.Base;
using WarehouseManagementSystem.Repositories.BaseRepository;
using WarehouseManagementSystem.Services.ReceiptService;
using WarehouseManagementSystem.Services.UnitsOfMeasurementService;
using WarehouseManagementSystem.Services.ResourceService;
using WarehouseManagementSystem.Services.BalanceService;
using WarehouseManagementSystem.Services.ShipmentService;

namespace WarehouseManagementSystem.Configurations
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitsOfMeasurementService, UnitsOfMeasurementService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<IBalanceService, BalanceService>();
            services.AddScoped<IShipmentService, ShipmentService>();



            return services;
        }
    }
}
