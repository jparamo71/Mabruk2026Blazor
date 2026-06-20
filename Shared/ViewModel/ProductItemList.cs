using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.ViewModel
{
    public class ProductItemList
    {

        public int ProductId { get; set; }

        public string? BarcharCode { get; set; } 

        public string? UPCCode { get; set; }    

        public string Name { get; set; } = string.Empty;

        public int MarkId { get; set; }

        public string? MarkName { get; set; }

        public decimal Available { get; set; } = 0;

    }
}
