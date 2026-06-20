using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Dtos
{
    public class ResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public dynamic? Content { get; set; }
    
        
        /*public ResponseDto(int statusCode, dynamic content, string Message) { 
            this.StatusCode = statusCode;
            this.Content = content;
            this.Message = Message;
        }*/

    }
}
