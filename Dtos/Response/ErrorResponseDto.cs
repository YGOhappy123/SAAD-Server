using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Dtos.Response
{
    public class ErrorResponseDto
    {
        public string? Message { get; set; }
        public object? Error { get; set; }
    }
}