using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Models
{
    public partial class sp_BuscarProductoResult
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Solo_Nombre { get; set; } = string.Empty;
        public string NombreProducto { get; set; } = string.Empty;        
        public decimal? ValorUnitario { get; set; }
        public decimal? ValorLista { get; set; }
        public int? Existencia { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string? RutaImagen { get; set; } = string.Empty;
    }
}
