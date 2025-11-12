// File: Models/LogData.cs

using System;
using System.Collections.Generic;

namespace MachineMonitoringApp.Models
{
    // Class yang merepresentasikan satu baris data log
    public class LogData
    {
        public DateTime Timestamp { get; set; }
        public string RawData { get; set; }
        public Dictionary<string, string> ParsedFields { get; set; } // Untuk data terpisah
    }

    // Class yang merepresentasikan keseluruhan file log
    public class MachineLog
    {
        public string MachineName { get; set; }
        public List<LogData> Entries { get; set; } = new List<LogData>();
    }
}