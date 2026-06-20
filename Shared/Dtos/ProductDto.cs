using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class ProductDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? BarcharCode { get; set; }
        public string? UPCCode { get; set; }
        public int MarkId { get; set; } 
        public decimal UnitPrice { get; set; }
        public string? MarkName { get; set; }
        public decimal Available { get; set; }
        public bool IsActive { get; set; }
        public int UnitsByPackage { get; set; }
        public string? ImageUrl { get; set; }
    }
}
