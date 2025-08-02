using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagementSystem.Models
{
    public class shipment
    {
        [Key]

        public Guid Id { get; set; } = Guid.NewGuid();

        public int Number { get; set; }
        [ForeignKey("client")]
        public Guid ClientId { get; set; }

        public bool IsSigned { get; set; }
        public Client client { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        public virtual List<shipmentItems> Items { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
