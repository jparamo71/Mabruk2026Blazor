using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class UpdateNoteDto
    {
        public int Id { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
