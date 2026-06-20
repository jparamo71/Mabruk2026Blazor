using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.ViewModel
{
    public class ViewVerificationImport
    {

		public string DocumentTypeId { get; set; } = string.Empty;
		public string DocumentNumber { get; set; } = string.Empty;
		public DateTime DateStart { get; set; }
		public string Status { get; set;  } = string.Empty;
		public string ProviderName { get; set;  }	 = string.Empty;
		public string BarcharCode { get; set; } = string.Empty;	
		public string ProductName { get; set; } = string.Empty;
		public int SystemAvailable { get; set; }
		public int PhysicalQuantity { get; set; }
		public int Difference { get; set; }

    }
}
