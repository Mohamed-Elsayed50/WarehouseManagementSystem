using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class UnitsOfMeasurement
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Archived { get; set; }
    }
}
