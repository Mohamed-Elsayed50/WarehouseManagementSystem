using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagementSystem.Models
{
    public class balance
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("resource")]
        public Guid ResourceId { get; set; }
        [ForeignKey("UnitOfMeasurement")]
        public Guid UnitOfMeasurementId { get; set; }
        public virtual Resource resource { get; set; }
        public virtual UnitsOfMeasurement UnitOfMeasurement { get; set; }
        public decimal Quantity { get; set; }
    }
}
