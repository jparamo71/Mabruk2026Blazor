using MabrukBlazor2026.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.ViewModel
{
    public  class TakingInventoryItemList
    {

        public DateTime DateStart { get; set; }

        public string Status { get; set; } = string.Empty;

        public string MarkName { get; set; } = string.Empty;

        public string BarcharCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public decimal SystemAvailable { get; set; }

        public decimal PhysicalQuantity { get; set; }

        public decimal Difference { get; set; }

        public string? Notes { get; set; }

    }
}
