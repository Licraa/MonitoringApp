// File: Models/LineSummary.cs

namespace MachineMonitoringApp.Models
{
    public class LineSummary
    {
        public string LineName { get; }
        public int Active { get; }
        public int Inactive { get; }
        public int Total { get; }
        public int Count { get; set; } // Jumlah total mesin
        public double Cycle { get; set; } // Waktu siklus rata-rata
        public double PartPerHour { get; set; } // Bagian per jam
        public double AvgCycle { get; set; } // Siklus rata-rata
        public double Downtime { get; set; } // Waktu tidak aktif
        public double DowntimePercentage { get; set; } // Persentase waktu tidak aktif
        public double Uptime { get; set; } // Waktu aktif
        public double UptimePercentage { get; set; } // Persentase waktu aktif

        public LineSummary(string lineName, int active, int inactive, int total)
        {
            this.LineName = lineName;
            this.Active = active;
            this.Inactive = inactive;
            this.Total = total;
        }
    }
}