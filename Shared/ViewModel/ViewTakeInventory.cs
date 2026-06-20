using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.ViewModel
{
    public class ViewTakeInventory
    {

		public DateTime DateStart { get; set;  }
		public string Status { get; set; } = string.Empty;
		public string MarkName { get; set; } = string.Empty;
		public string BarcharCode { get; set; } = string.Empty;
		public string ProductName { get; set; } = string.Empty;
		public int systemAvailable { get; set; } = 0;
		public int PhysicalQuantity { get; set; } = 0;
		public int Difference { get; set;  } = 0;
		public string Notes { get; set;  } = string.Empty;	

    }
}
