using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class ProductsFilterDto
    {

        public string? TextToSearch { get; set; }
        public int SelectedMarkId { get; set; } = 0;
        public bool IsActive { get; set; } = false;
        public bool OnlyWithStock { get; set; } = false;
        public int CustomerId { get; set; }

    }
}
