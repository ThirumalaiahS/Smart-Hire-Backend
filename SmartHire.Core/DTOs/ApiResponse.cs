using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.DTOs
{
    public class ApiResponse<T> 
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int StatusCode { get; set; }
        public Object? Meta { get; set; }
        public ApiResponse() { }
        public ApiResponse(bool success, string message, T? data = default, List<string>? errors = default, int statusCode = 200, Object? meta = default)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
            StatusCode = statusCode;
            Meta = meta;
        }

        // Factory helpers for cleaner usage
        public static ApiResponse<T> SuccessResponse(T? data, string message = "Request successful", int statusCode = 200)
            => new ApiResponse<T> { Success = true, Message = message, Data = data, StatusCode = statusCode };

        public static ApiResponse<T> ErrorResponse(List<string> errors, string message = "Request failed", int statusCode = 400)
            => new ApiResponse<T> { Success = false, Message = message, Errors = errors, StatusCode = statusCode };
    }
}
