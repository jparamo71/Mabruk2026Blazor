using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class UpdateUPCCodeDto
    {

        [Required]
        public string BarCharCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código del paquete es requerido.")]
        public string UPCCode { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingreso un valor mayor que {1}")]
        public int UnitsByPackage { get;set; } = 0;

    }
}
