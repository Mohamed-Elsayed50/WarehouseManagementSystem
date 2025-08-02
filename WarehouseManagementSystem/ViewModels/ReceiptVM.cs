using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.ViewModels
{
    public class ReceiptVM
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "Номер обязателен для заполнения")]
        public int Number { get; set; }

        public DateTime Date { get; set; }
        [MinLength(1, ErrorMessage = "Необходимо добавить хотя бы один элемент")]
        public List<ReceiptItemVM> Items { get; set; } = new();

        public SelectList ResourceList { get; set; }
        public SelectList UnitList { get; set; }
        public ReceiptVM(Receipt receipt)
        {
            Id = receipt.Id;
            Number = receipt.Number;
            Date = receipt.Date;

            Items = receipt.Items.Select(i => new ReceiptItemVM
            {
                Id = i.Id,
                ResourceId = i.ResourceId,
                UnitId = i.UnitId,
                Quantity = i.Quantity
            }).ToList();
        }
        public ReceiptVM()
        {
        }

    }
}
