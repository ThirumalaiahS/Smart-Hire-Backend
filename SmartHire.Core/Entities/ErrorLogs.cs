using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.Entities
{
    public class ErrorLogs
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string? Endpoint { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? ExceptionType { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? RequestPayload { get; set; }
        public string? ResponsePayload { get; set; }
        public Guid? CorrelationId { get; set; }
        public string? Source { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Severity { get; set; }
    }
}
