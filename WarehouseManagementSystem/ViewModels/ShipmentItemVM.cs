using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.ViewModels
{
    public class ShipmentItemVM
    {
        [Required]
        public Guid ResourceId { get; set; }

        [Required]
        public Guid UnitId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Введите количество")]
        public decimal Quantity { get; set; }

    }
}
