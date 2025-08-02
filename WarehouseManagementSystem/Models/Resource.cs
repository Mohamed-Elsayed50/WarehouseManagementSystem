using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class Resource
    {
        [Key]
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public bool Archived { get; set; }
    }
}
