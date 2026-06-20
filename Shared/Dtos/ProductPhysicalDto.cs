using MabrukBlazor2026.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class ProductPhysicalDto
    {

        public int Id { get; set; }

        public int InventoryTakingId { get; set; }

        public int ProductId { get; set; }

        public string BarcharCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public decimal SystemAvailable { get; set; }

        [Required(ErrorMessage = "Ingrese el conteo físico")]
        public decimal PhysicalQuantity { get; set; }

        public decimal Difference { get; set; }

        public bool Justified { get; set; }

        [ConditionalValidation]
        public string? Notes { get; set; }

    }
}
