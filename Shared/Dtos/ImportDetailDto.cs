using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class ImportDetailDto
    {

        public int Id { get; set; }

        public int DocumentId { get; set; }

        public int ProductId { get; set; }

        public string BarcharCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal VerifiedQuantity { get; set; }

        public decimal TotalValue { get; set; }

    }
}
