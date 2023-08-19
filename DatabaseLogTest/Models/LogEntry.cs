using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLogTest.Models
{
    public class LogEntry
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string LogLevel { get; set; }
        public string? Source { get; set; }
        public string? Message { get; set; }
        public string? Exception { get; set; }
    }
}
