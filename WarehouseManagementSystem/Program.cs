using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Configurations;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repositories.BaseRepository;
using WarehouseManagementSystem.Repository.Base;

namespace WarehouseManagementSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"), b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
            builder.Services.RegisterApplicationServices();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.Migrate();

                var sql = await File.ReadAllTextAsync("SqlScripts/SyncBalance.sql");
                await db.Database.ExecuteSqlRawAsync(sql);


            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
