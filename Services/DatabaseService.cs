using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using MachineMonitoringApp.Models;

namespace MachineMonitoringApp.Services
{
    /// <summary>
    /// Simple helper to insert serial/telemetry data into SQL Server.
    /// Update ConnectionString below to match your environment.
    /// This file uses Microsoft.Data.SqlClient; if the package is not
    /// referenced, run: dotnet add package Microsoft.Data.SqlClient
    /// </summary>
    public static class DatabaseService
    {
        // TODO: GANTI connection string ini sesuai environment Anda
        public static string ConnectionString = "Server=localhost;Database=MachineDB;Trusted_Connection=True;";

        public static void InsertSerialData(MachineData m, string rawData)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"
INSERT INTO SerialData
    (MachineId, MachineName, LineName, NilaiA0, NilaiTerakhirA2, DurasiTerakhirA4, RataRataTerakhirA4, PartHours, DataCh1, Uptime, P_DataCh1, P_Uptime, RawData, RecordedAt)
VALUES
    (@MachineId, @MachineName, @LineName, @NilaiA0, @NilaiTerakhirA2, @DurasiTerakhirA4, @RataRataTerakhirA4, @PartHours, @DataCh1, @Uptime, @P_DataCh1, @P_Uptime, @RawData, @RecordedAt)
";

                    cmd.Parameters.AddWithValue("@MachineId", m.id);
                    cmd.Parameters.AddWithValue("@MachineName", (object)m.name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LineName", (object)m.line_production ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NilaiA0", m.nilaiA0);
                    cmd.Parameters.AddWithValue("@NilaiTerakhirA2", (object)m.nilaiTerakhirA2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DurasiTerakhirA4", (object)m.durasiTerakhirA4 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RataRataTerakhirA4", (object)m.ratarataTerakhirA4 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PartHours", (object)m.parthours ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DataCh1", (object)m.dataCh1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Uptime", (object)m.uptime ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@P_DataCh1", (object)m.p_datach1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@P_Uptime", (object)m.p_uptime ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RawData", (object)rawData ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RecordedAt", DateTime.Now);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // jangan lempar ke UI; cukup catat untuk debug
                Debug.WriteLine($"Failed to insert serial data: {ex.Message}");
            }
        }
    }
}
