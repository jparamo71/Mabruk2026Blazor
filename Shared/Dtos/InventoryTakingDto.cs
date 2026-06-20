using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class InventoryTakingDto
    {

        public int Id { get; set; }

        public DateTime? DateStart { get; set; }

        public string Status { get; set; } = string.Empty;

        public int UserId { get; set; }

        public int MarkId { get; set; }

        public string MarkName { get; set; } = string.Empty;


        public List<ProductPhysicalDto>? Details { get; set; }


    }
}
