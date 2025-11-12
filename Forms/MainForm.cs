using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Reflection;
using MachineMonitoringApp.Models;
using MachineMonitoringApp.Services;
using MachineMonitoringApp.Utils;

namespace MachineMonitoringApp.Forms
{
    // Asumsi: Kontrol utama Anda di Designer bernama flowLayoutPanel1
    public partial class MainForm : Form
    {
        // === PROPERTI UTAMA ===
        private SerialDataLogger _dataLogger;
        private System.Windows.Forms.Timer _uiUpdateTimer;
        private List<MachineData> _allMachineData = new List<MachineData>();

        // Asumsi: flowLayoutPanel1 adalah FlowLayoutPanel di designer Anda

        public MainForm()
        {
            InitializeComponent();

            this.Text = "Machine Monitoring Dashboard";

            // 🚨 1. AKTIVASI DOUBLE BUFFERING (Penting untuk mengatasi BLINKING)
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            // Mengatur Double Buffering pada FlowLayoutPanel secara Reflection
            if (flowLayoutPanel1 != null)
            {
                flowLayoutPanel1.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(flowLayoutPanel1, true, null);
            }

            // 2. Setup Data dan Logger
            _allMachineData = GetInitialMachineData();
            InitializeLogger();

            // 3. Setup Timer untuk update Dashboard
            InitializeUpdateTimer();

            // 4. Render awal (Ini akan membuat card pertama kali)
            RenderDashboard(GenerateSummary(_allMachineData));
        }

        // --------------------------------------------------------------------------
        // === LOGIKA REAL-TIME SERIAL PORT ===
        // --------------------------------------------------------------------------

        private void InitializeLogger()
        {
            const string portToUse = "COM4";
            const int baudRate = 115200;

            try
            {
                _dataLogger = new SerialDataLogger(portToUse, baudRate);
                _dataLogger.DataReceived += DataLogger_DataReceived;
                _dataLogger.Open();
                System.Diagnostics.Debug.WriteLine($"Berhasil membuka port: {portToUse} @ {baudRate}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Koneksi Serial Gagal. Pastikan perangkat terpasang di {portToUse}. Error: {ex.Message}",
                                "Error Koneksi Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _dataLogger = null;
            }
        }

        private void InitializeUpdateTimer()
        {
            _uiUpdateTimer = new System.Windows.Forms.Timer();
            _uiUpdateTimer.Interval = 1000; // Update every second
            _uiUpdateTimer.Tick += (s, e) =>
            {
                footerLabel.Text = DateTime.Now.ToString("dddd, MMMU dd, yyyy HH:mm:ss");
            };
            _uiUpdateTimer.Start();
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            // 🚨 JANTUNG REAL-TIME: Memastikan UI di-update secara berkala dan stabil
            RenderDashboard(GenerateSummary(_allMachineData));
        }

        private void DataLogger_DataReceived(string rawData)
        {
            // Wajib menggunakan Invoke untuk memproses data di thread UI (Thread Safety)
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ProcessDataForUI(rawData)));
            }
            else
            {
                ProcessDataForUI(rawData);
            }
        }

        private void ProcessDataForUI(string rawData)
        {
            // Logika parsing 10 item data serial
            if (rawData.Length < 10 || rawData.Trim().Equals("Reset", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var parts = rawData.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(p => p.Trim())
                               .ToList();

            if (parts.Count < 10) return;

            MachineData updatedMachine = ParseMachineGroup(parts.Take(10).ToList());

            if (updatedMachine != null)
            {
                var existingMachine = _allMachineData.FirstOrDefault(m => m.id == updatedMachine.id);

                if (existingMachine != null)
                {
                    // Update data yang datang dari Serial Port (10 item)
                    existingMachine.nilaiA0 = updatedMachine.nilaiA0;
                    existingMachine.parthours = updatedMachine.parthours;
                    existingMachine.nilaiTerakhirA2 = updatedMachine.nilaiTerakhirA2;
                    existingMachine.durasiTerakhirA4 = updatedMachine.durasiTerakhirA4;
                    existingMachine.ratarataTerakhirA4 = updatedMachine.ratarataTerakhirA4;
                    existingMachine.dataCh1 = updatedMachine.dataCh1;
                    existingMachine.uptime = updatedMachine.uptime;
                    existingMachine.p_datach1 = updatedMachine.p_datach1;
                    existingMachine.p_uptime = updatedMachine.p_uptime;
                    existingMachine.LastUpdated = DateTime.Now;
                }
            }
        }

        private MachineData ParseMachineGroup(List<string> parts)
        {
            // ... (Kode parsing tetap sama, menggunakan MachineMapper) ...
            if (parts.Count < 10) return null;

            try
            {
                int id = int.Parse(parts[0]);
                var identity = MachineMapper.GetIdentityById(id);

                if (identity == null) return null;

                return new MachineData
                {
                    id = id,
                    name = identity.Name,
                    line_production = identity.Line,

                    nilaiA0 = int.Parse(parts[1]),
                    nilaiTerakhirA2 = parts[2],
                    durasiTerakhirA4 = parts[3],
                    ratarataTerakhirA4 = parts[4],
                    parthours = parts[5],
                    dataCh1 = parts[6],
                    uptime = parts[7],
                    p_datach1 = parts[8],
                    p_uptime = parts[9],
                    LastUpdated = DateTime.Now
                };
            }
            catch { return null; }
        }

        // --------------------------------------------------------------------------
        // === LOGIKA RENDERING & KLIK CARD (Real-Time Optimized) ===
        // --------------------------------------------------------------------------

        private void RenderDashboard(Dictionary<string, LineSummary> data)
        {
            flowLayoutPanel1.Controls.Clear();

            foreach (var entry in data)
            {
                var line = entry.Value;
                var card = CreateLineCard(line); // Create a card for each item
                flowLayoutPanel1.Controls.Add(card); // Add the card to flowLayoutPanel1
            }

            flowLayoutPanel1.ResumeLayout();
        }

        private Panel CreateLineCard(LineSummary line)
        {
            // ========================================
            // === 1. KARTU UTAMA (CARD) ===
            // ========================================
            var card = new Panel
            {
                Width = 350,  // Ukuran kartu yang lebih wajar
                Height = 450, // Tinggi kartu
                BackColor = Color.White,
                BorderStyle = BorderStyle.None, // Menghilangkan border kuno
                Margin = new Padding(15),       // Jarak antar kartu
                Padding = new Padding(10)       // Jarak dalam kartu
            };
            // Tambahkan event handler sederhana untuk efek "hover"
            card.MouseEnter += (s, e) => { card.BackColor = Color.FromArgb(248, 249, 250); };
            card.MouseLeave += (s, e) => { card.BackColor = Color.White; };

            // ========================================
            // === 2. JUDUL KARTU (TITLE) ===
            // ========================================
            var title = new Label
            {
                Text = line.LineName ?? "NULL",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80), // Warna Dark Slate
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ========================================
            // === 3. PANEL STATUS (CONNECTED/NOT CONNECTED) ===
            // ========================================
            var statusPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(5)
            };

            var connectedLabel = new Label
            {
                Text = $"{line.Active}\nConnected",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(39, 174, 96), // Warna Hijau "Flat"
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Left,
                Width = (card.Width - card.Padding.Horizontal - statusPanel.Padding.Horizontal - 10) / 2, // Setengah lebar
                TextAlign = ContentAlignment.MiddleCenter
            };

            var notConnectedLabel = new Label
            {
                Text = $"{line.Inactive}\nNot Connected",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(231, 76, 60), // Warna Merah "Flat"
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Right,
                Width = (card.Width - card.Padding.Horizontal - statusPanel.Padding.Horizontal - 10) / 2, // Setengah lebar
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Panel pemisah kecil (opsional)
            var separator = new Panel { Dock = DockStyle.Left, Width = 10, BackColor = Color.White };

            statusPanel.Controls.Add(connectedLabel);
            statusPanel.Controls.Add(separator);
            statusPanel.Controls.Add(notConnectedLabel);


            // ========================================
            // === 4. LABEL STATISTIK UTAMA ===
            // ========================================
            // Menggunakan string yang sama dari kode Anda
            var statsText = $"COUNT\n{line.Count}\n\n" +
                            $"CYCLE\n{line.Cycle}s\n\n" +
                            $"PART/HR\n{line.PartPerHour}\n\n" +
                            $"AVG CYCLE\n{line.AvgCycle}s\n\n" +
                            $"DOWNTIME\n{line.Downtime} ({line.DowntimePercentage}%)\n\n" +
                            $"UPTIME\n{line.Uptime} ({line.UptimePercentage}%)";

            var statsLabel = new Label
            {
                Text = statsText,
                Font = new Font("Segoe UI", 11, FontStyle.Regular), // Font Regular untuk data
                ForeColor = Color.FromArgb(82, 82, 82), // Abu-abu gelap (bukan hitam)
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // ========================================
            // === 5. TOTAL LABEL (FOOTER KARTU) ===
            // ========================================
            var totalLabel = new Label
            {
                Text = $"{line.Total} Total Machine",
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(241, 243, 244) // Footer kartu yang senada
            };

            // ========================================
            // === 6. SUSUN KONTROL PADA KARTU ===
            // ========================================
            card.Controls.Add(statsLabel);      // Diisi dulu (Dock.Fill)
            card.Controls.Add(totalLabel);
            card.Controls.Add(statusPanel);
            card.Controls.Add(title);

            return card;
        }

        // --------------------------------------------------------------------------
        // === MEMBERSIHKAN SUMBER DAYA ===
        // --------------------------------------------------------------------------

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dataLogger?.Close();
            _uiUpdateTimer?.Stop();
            base.OnFormClosing(e);
        }

        // Metode Pembantu Lainnya
        // ... (GetInitialMachineData, GenerateSummary tetap sama) ...

        private List<MachineData> GetInitialMachineData()
        {
            var initialList = new List<MachineData>();
            foreach (var identity in MachineMapper.GetAllIdentities())
            {
                initialList.Add(new MachineData
                {
                    id = identity.Id,
                    name = identity.Name,
                    line_production = identity.Line,
                });
            }
            return initialList;
        }

        private Dictionary<string, LineSummary> GenerateSummary(List<MachineData> machines)
        {
            return machines.GroupBy(m => m.line_production)
                           .ToDictionary(
                               g => g.Key,
                               g => new LineSummary(
                                   g.Key,
                                   g.Count(m => m.nilaiA0 == 1),
                                   g.Count(m => m.nilaiA0 == 0),
                                   g.Count()
                               )
                           );
        }
    }
}