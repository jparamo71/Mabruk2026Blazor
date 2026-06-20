using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class PaginationDto
    {

        public int? Page { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public string? Filter { get; set; }

        public int MarkId { get; set; } = 0;

    }
}
