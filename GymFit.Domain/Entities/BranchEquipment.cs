using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Entities
{

    public class BranchEquipment
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string EquipmentName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Cardio, Strength, etc.
        public int Quantity { get; set; }
        public string? Brand { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? MaintenanceSchedule { get; set; }
        public bool IsWorking { get; set; } = true;

        public virtual Branch Branch { get; set; } = null!;
    }
}
