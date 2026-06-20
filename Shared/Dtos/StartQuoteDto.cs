using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class StartQuoteDto
    {
        public string SellerEmail { get; set; } = string.Empty;
        public int CustomerId { get; set; } 
        public bool IsQuote { get; set; } = true;   
    }
}
