using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class OrderRequestDto
    {
        public int Id { get; set; } = 0;
        public DateTime OrderDate { get; set; }
        public string OrderDateStr { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsQuoted { get; set; }
        public string Note { get; set; } = string.Empty;
        public string AddressDelivery { get; set; } = string.Empty;        
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerNit { get; set; } = string.Empty;
        public int OrderStateId {get;set; } = 0;
        public string OrderStateName { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public string SellerEmail { get; set; } = string.Empty; 

        public List<OrderRequestDetailDto> Details { get; set; } = new List<OrderRequestDetailDto>();

    }
}
