using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace trabahuso_api.DTOs.Responses
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public required string Message { get; set; }
        public required string Path { get; set; }

    }
}