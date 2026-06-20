using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.ViewModel
{
    public class ViewImportDoc
    {

		public int Id { get; set; }
		public string DocumentTypeId { get; set; } = string.Empty;
		public string DocumentTypeName { get; set;  } = string.Empty;
		public string DocumentNumber { get; set; } = string.Empty;
		public int ProviderId { get; set; }
		public string ProviderName { get; set; } = string.Empty;
		public DateTime DocumentDate { get; set;  }
		public string Status {get; set; } = string.Empty;
		public int RowNumber { get; set; }

    }
}
