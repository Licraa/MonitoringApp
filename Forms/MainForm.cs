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
            _uiUpdateTimer.Interval = 500; // Update UI setiap 0.5 detik (Real-Time Speed)
            _uiUpdateTimer.Tick += UiUpdateTimer_Tick;
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
            flowLayoutPanel1.SuspendLayout();

            foreach (var entry in data)
            {
                var line = entry.Value;
                // Cari apakah card sudah ada (menggunakan Text/Nama Line sebagai ID unik)
                Panel existingCard = flowLayoutPanel1.Controls.OfType<Panel>()
                                            .FirstOrDefault(c => c.Text == line.LineName);

                if (existingCard != null)
                {
                    // 🚨 UPDATE IN-PLACE: HANYA UPDATE NILAI LABEL
                    if (existingCard.Tag is Label statusLabel)
                    {
                        // Update teks label status
                        statusLabel.Text = $"Active: {line.Active}\nInactive: {line.Inactive}\nTotal: {line.Total}";
                    }
                    // Anda bisa menambahkan logika pewarnaan ulang di sini jika status berubah
                }
                else
                {
                    // CARD BARU: Buat dan tambahkan (hanya terjadi satu kali di awal)
                    var card = CreateLineCard(line);
                    card.Text = line.LineName; // Penting: Set Text untuk pencarian
                    flowLayoutPanel1.Controls.Add(card);
                }
            }

            flowLayoutPanel1.ResumeLayout();
        }

        private Panel CreateLineCard(LineSummary line)
        {
            // ... (setup card, title, status) ...
            var card = new Panel
            {
                Width = 300,
                Height = 220,
                BackColor = Color.FromArgb(238, 238, 238),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(15),
                Cursor = Cursors.Hand
            };

            var title = new Label
            {
                Text = line.LineName,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };

            var status = new Label
            {
                Dock = DockStyle.Fill,
                Text = $"Active: {line.Active}\nInactive: {line.Inactive}\nTotal: {line.Total}",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 13, FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 55, 55),
                Cursor = Cursors.Hand
            };

            // 🚨 SIMPAN REFERENCE LABEL DI TAG UNTUK UPDATE REAL-TIME
            card.Tag = status;

            // LOGIKA KLIK CARD
            EventHandler cardClickHandler = (s, e) =>
            {
                var detailMachines = _allMachineData.Where(m => m.line_production == line.LineName).ToList();

                try
                {
                    var detailForm = new DetailForm(line, detailMachines);
                    detailForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Gagal membuka Detail Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Lampirkan handler ke SEMUA kontrol (Fix Masalah Klik)
            card.Click += cardClickHandler;
            title.Click += cardClickHandler;
            status.Click += cardClickHandler;

            card.Controls.Add(status);
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