using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class Client
    {
        [Key]
        public Guid Id { get; set; } 
        [DisplayName("Наименование")]
        public string Name { get; set; }
        [DisplayName("Адрес")]
        public string Address { get; set; }
        [DisplayName("архив")]
        public bool Archived { get; set; } = false;

        public void Archive() => Archived = true;
    }
}
