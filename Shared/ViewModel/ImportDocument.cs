using MabrukBlazor2026.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.ViewModel
{
    public  class ImportDocument
    {
        public int Id { get; set; }

        public string DocumentTypeId { get; set; } = string.Empty;

        public string DocumentTypeName { get; set; } = string.Empty;

        public string DocumentNumber { get; set; } = string.Empty;

        public int ProviderId { get; set; }

        public string ProviderName { get; set; } = string.Empty;

        public DateTime DocumentDate { get; set; }

        public decimal TotalValue { get; set; }

        public string Status { get; set; } = string.Empty;

    }
}
