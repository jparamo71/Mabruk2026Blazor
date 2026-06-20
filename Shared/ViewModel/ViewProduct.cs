using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.ViewModel
{
    public class ViewProduct
    {
        public int ProductId { get; set; }
        public string BarcharCode { get; set; } = string.Empty;
        public string UPCCode { get; set;  } = string.Empty;
        public int MarkId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string MarkName { get; set; } = string.Empty;
        public int Available { get; set; }
        public int RowNumber { get; set; }

    }
}
