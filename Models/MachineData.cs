using System;

namespace MachineMonitoringApp.Models
{
    public class MachineData
    {
        // === Identitas & Status Utama ===
        public int id { get; set; }
        public string name { get; set; }
        public string line_production { get; set; }
        public int nilaiA0 { get; set; } = 0; // Status (1=Active, 0=Inactive)
        public DateTime LastUpdated { get; set; } = DateTime.MinValue;
        public string process { get; set; } = "Packing";
        public string remark { get; set; } = "-";

        // === Data Utama (10 Item Serial Port) ===
        public string parthours { get; set; } = "0";
        public string durasiTerakhirA4 { get; set; } = "0";
        public string ratarataTerakhirA4 { get; set; } = "0";
        public string nilaiTerakhirA2 { get; set; } = "0"; // Total Count
        public string dataCh1 { get; set; } = "00:00:00"; // Total Downtime Time
        public string uptime { get; set; } = "00:00:00";  // Total Uptime Time
        public string p_datach1 { get; set; } = "0";      // Total Downtime %
        public string p_uptime { get; set; } = "0";       // Total Uptime %

        // === Data Shift (untuk Detail Card) - Diisi dari API/DB, tidak dari serial port 10 item ===
        // Shift 1st
        public string s1_nilaiTerakhirA2 { get; set; } = "0";
        public string s1_dataCh1 { get; set; } = "00:00:00";
        public string s1_p_datach1 { get; set; } = "0";
        public string s1_uptime { get; set; } = "00:00:00";
        public string s1_p_uptime { get; set; } = "0";

        // Shift 2nd
        public string s2_nilaiTerakhirA2 { get; set; } = "0";
        public string s2_dataCh1 { get; set; } = "00:00:00";
        public string s2_p_datach1 { get; set; } = "0";
        public string s2_uptime { get; set; } = "00:00:00";
        public string s2_p_uptime { get; set; } = "0";

        // Shift 3rd
        public string s3_nilaiTerakhirA2 { get; set; } = "0";
        public string s3_dataCh1 { get; set; } = "00:00:00";
        public string s3_p_datach1 { get; set; } = "0";
        public string s3_uptime { get; set; } = "00:00:00";
        public string s3_p_uptime { get; set; } = "0";
    }

    // public class LineSummary
    // {
    //     public string LineName { get; set; }
    //     public int Active { get; set; }
    //     public int Inactive { get; set; }
    //     public int Total { get; set; }

    //     // public LineSummary(string name, int active, int inactive, int total)
    //     // {
    //     //     LineName = name;
    //     //     Active = active;
    //     //     Inactive = inactive;
    //     //     Total = total;
    //     // }
    // }

    public class MachineIdentity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Line { get; set; }
    }
}