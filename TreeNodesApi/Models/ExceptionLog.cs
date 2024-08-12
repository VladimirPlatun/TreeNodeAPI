using System;

namespace TreeNodesApi.Models
{
    public class ExceptionLog
    {
        public int Id { get; set; }

        public Guid EventId { get; set; } = Guid.NewGuid();

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string QueryParams { get; set; }

        public string BodyParams { get; set; }

        public string StackTrace { get; set; }

        public string ExceptionType { get; set; }

        public string Message { get; set; }
    }
}
