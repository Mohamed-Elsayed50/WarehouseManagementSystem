namespace WarehouseManagementSystem.ViewModels
{
    public class AvailableItemVM
    {
        public Guid ResourceId { get; set; }
        public string ResourceName { get; set; }
        public Guid UnitId { get; set; }
        public string UnitName { get; set; }
        public decimal AvailableQuantity { get; set; }
    }
}
