using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class OrderRequestDetailDto
    {
        public int DetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal QuantityAvailable { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrize { get; set; }
        public decimal Total { get; set; }
    }
}
