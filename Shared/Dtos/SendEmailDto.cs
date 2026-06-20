using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class SendEmailDto
    {
        public int PedidoId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
