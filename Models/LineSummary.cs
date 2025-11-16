// File: Models/LineSummary.cs

namespace MachineMonitoringApp.Models
{
    public class LineSummary
    {
        public string LineName { get; }
        public int Active { get; }
        public int Inactive { get; }
        public int Total { get; }

        // Properti yang bisa ditambahkan (dari data PHP Anda):
        // public string AveragePartPerHour { get; set; }
        // public string TotalUptimePercentage { get; set; }

        public LineSummary(string lineName, int active, int inactive, int total)
        {
            this.LineName = lineName;
            this.Active = active;
            this.Inactive = inactive;
            this.Total = total;
        }
    }
}