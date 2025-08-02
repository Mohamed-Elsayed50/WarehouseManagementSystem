using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class shipmentItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("shipment")]
        public Guid shipmentId { get; set; }
        public virtual shipment shipment { get; set; }

        [ForeignKey("Resource")]
        public Guid ResourceId { get; set; }
        public virtual Resource Resource { get; set; }

        [ForeignKey("Unit")]
        public Guid UnitId { get; set; }
        public virtual UnitsOfMeasurement Unit { get; set; }

        public decimal Quantity { get; set; }
    }
}
