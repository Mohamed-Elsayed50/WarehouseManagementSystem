using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class Receipt
    {
        [Key]

        public Guid Id { get; set; } = Guid.NewGuid();

        public int Number { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        public virtual List<ReceiptItem> Items { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
