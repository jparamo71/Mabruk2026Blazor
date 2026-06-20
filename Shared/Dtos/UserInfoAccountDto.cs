using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class UserInfoAccountDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;   
        public string FullName { get; set; } = string.Empty;
        public bool AllowUploadImage { get; set; }

    }
}
