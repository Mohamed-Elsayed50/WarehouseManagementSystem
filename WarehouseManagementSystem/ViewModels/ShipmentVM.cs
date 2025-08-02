using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.ViewModels
{
    public class ShipmentVM
    {
        public Guid? Id { get; set; }

        [Required]
        public int Number { get; set; }
        public bool IsSigned { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        public Guid ClientId { get; set; }

        public List<ShipmentItemVM> Items { get; set; } = new();

        // Dropdown lists
        public SelectList ResourceList { get; set; }
        public SelectList UnitList { get; set; }
        public SelectList ClientList { get; set; }

        public ShipmentVM() { }

        public ShipmentVM(shipment shipment)
        {
            Id = shipment.Id;
            Number = shipment.Number;
            Date = shipment.Date;
            IsSigned = shipment.IsSigned;
            ClientId = shipment.ClientId;
            Items = shipment.Items.Select(i => new ShipmentItemVM
            {
                ResourceId = i.ResourceId,
                UnitId = i.UnitId,
                Quantity = i.Quantity
            }).ToList();
        }

        public shipment ToModel()
        {
            return new shipment
            {
                Id = Id ?? Guid.NewGuid(),
                Number = Number,
                Date = Date,
                IsSigned = IsSigned,
                ClientId = ClientId,
                Items = Items.Select(i => new shipmentItems
                {
                    Id = Guid.NewGuid(),
                    ResourceId = i.ResourceId,
                    UnitId = i.UnitId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}
