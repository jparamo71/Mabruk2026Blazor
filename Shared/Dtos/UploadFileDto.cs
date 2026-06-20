using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class UploadFileDto
    {
        public int ProductId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FileContent { get; set; } = string.Empty;
    }
}
