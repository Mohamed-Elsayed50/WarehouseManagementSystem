using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.ViewModels
{
    public class ReceiptItemVM
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "Пожалуйста, выберите ресурс")]
        public Guid ResourceId { get; set; }
        [Required(ErrorMessage = "Пожалуйста, выберите единицу измерения")]
        public Guid UnitId { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите количество")]

        public decimal Quantity { get; set; }
    }
}
