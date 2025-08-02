using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace WarehouseManagementSystem.Models
{
    public class AppDbContext :DbContext
    {



        public DbSet<Client> Clients { get; set; }
        public DbSet<UnitsOfMeasurement> UnitsOfMeasurement { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }
        public DbSet<balance> balances { get; set; }
        public DbSet<shipmentItems> shipmentItems { get; set; }
        public DbSet<shipment> shipment { get; set; }





        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReceiptItem>()
                .HasOne(ri => ri.Resource)
                .WithMany()
                .HasForeignKey(ri => ri.ResourceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReceiptItem>()
                .HasOne(ri => ri.Unit)
                .WithMany()
                .HasForeignKey(ri => ri.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReceiptItem>()
                .HasOne(ri => ri.Receipt)
                .WithMany(r => r.Items)
                .HasForeignKey(ri => ri.ReceiptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<balance>()
                .HasOne(b => b.resource)
                .WithMany()
                .HasForeignKey(b => b.ResourceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<balance>()
               .HasOne(b => b.UnitOfMeasurement)
               .WithMany()
               .HasForeignKey(b => b.UnitOfMeasurementId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<shipmentItems>()
               .HasOne(ri => ri.Resource)
               .WithMany()
               .HasForeignKey(ri => ri.ResourceId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<shipmentItems>()
               .HasOne(ri => ri.Resource)
               .WithMany()
               .HasForeignKey(ri => ri.ResourceId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<shipmentItems>()
               .HasOne(ri => ri.Resource)
               .WithMany()
               .HasForeignKey(ri => ri.ResourceId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<shipment>()
               .HasOne(ri => ri.client)
               .WithMany()
               .HasForeignKey(ri => ri.ClientId)
               .OnDelete(DeleteBehavior.Restrict);
        }
        
    }
}
