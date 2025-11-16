using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MachineMonitoringApp.Models;
using MachineMonitoringApp.Services;
using MachineMonitoringApp.Utils;
using WinTimer = System.Windows.Forms.Timer;

namespace MachineMonitoringApp.Forms
{
    public partial class MainForm : Form
    {
        private SerialDataLogger _dataLogger;
        private WinTimer _uiUpdateTimer;
        private List<MachineData> _allMachineData = new List<MachineData>();

        public MainForm()
        {
            InitializeComponent();

            _allMachineData = GetInitialMachineData();
            InitializeLogger();
            InitializeUpdateTimer();
            // subscribe to layout event so we can center each row of cards
            this.flowLayoutPanel1.Layout += FlowLayoutPanel1_Layout;

            RenderDashboard(GenerateSummary(_allMachineData));
        }

        // ========== SERIAL LOGGER ==========
        private void InitializeLogger()
        {
            const string portToUse = "COM4";
            const int baudRate = 115200;
            try
            {
                _dataLogger = new SerialDataLogger(portToUse, baudRate);
                _dataLogger.DataReceived += DataLogger_DataReceived;
                _dataLogger.Open();
                System.Diagnostics.Debug.WriteLine($"Connected to {portToUse}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal membuka port {portToUse}: {ex.Message}");
            }
        }

        // ========== TIMER ==========
        private void InitializeUpdateTimer()
        {
            _uiUpdateTimer = new WinTimer();
            _uiUpdateTimer.Interval = 500;
            _uiUpdateTimer.Tick += UiUpdateTimer_Tick;
            _uiUpdateTimer.Start();
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss");
            RenderDashboard(GenerateSummary(_allMachineData));
        }

        // ========== DATA SERIAL ==========
        private void DataLogger_DataReceived(string rawData)
        {
            if (InvokeRequired)
                Invoke(new Action(() => ProcessDataForUI(rawData)));
            else
                ProcessDataForUI(rawData);
        }

        private void ProcessDataForUI(string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData) || rawData.Length < 10 || rawData.Trim().Equals("Reset", StringComparison.OrdinalIgnoreCase))
                return;

            var parts = rawData.Split(',').Select(p => p.Trim()).ToList();
            if (parts.Count < 10) return;

            var updatedMachine = ParseMachineGroup(parts.Take(10).ToList());
            if (updatedMachine == null) return;

            var existing = _allMachineData.FirstOrDefault(m => m.id == updatedMachine.id);
            if (existing != null)
            {
                existing.nilaiA0 = updatedMachine.nilaiA0;
                existing.parthours = updatedMachine.parthours;
                existing.nilaiTerakhirA2 = updatedMachine.nilaiTerakhirA2;
                existing.durasiTerakhirA4 = updatedMachine.durasiTerakhirA4;
                existing.ratarataTerakhirA4 = updatedMachine.ratarataTerakhirA4;
                existing.dataCh1 = updatedMachine.dataCh1;
                existing.uptime = updatedMachine.uptime;
                existing.p_datach1 = updatedMachine.p_datach1;
                existing.p_uptime = updatedMachine.p_uptime;
                existing.LastUpdated = DateTime.Now;

                // push ke database (silent on failure)
                try { DatabaseService.InsertSerialData(existing, rawData); } catch { }
            }
            else
            {
                // baru: tambahkan ke list dan simpan juga ke DB
                _allMachineData.Add(updatedMachine);
                try { DatabaseService.InsertSerialData(updatedMachine, rawData); } catch { }
            }
        }

        private MachineData ParseMachineGroup(List<string> parts)
        {
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

        // ========== RENDER DASHBOARD ==========
        private void RenderDashboard(Dictionary<string, LineSummary> data)
        {
            flowLayoutPanel1.SuspendLayout();
            foreach (var entry in data)
            {
                var line = entry.Value;
                var existingCard = flowLayoutPanel1.Controls.OfType<LineCardControl>()
                    .FirstOrDefault(c => c.LineName == line.LineName);

                if (existingCard != null)
                {
                    existingCard.SetSummary(line);
                }
                else
                {
                    var card = CreateLineCard(line);
                    card.Tag = line.LineName;
                    flowLayoutPanel1.Controls.Add(card);
                }
            }
            flowLayoutPanel1.ResumeLayout();
        }

        private LineCardControl CreateLineCard(LineSummary line)
        {
            var control = new LineCardControl();
            control.SetSummary(line);

            // When card clicked, show details (subscribe to CardClicked for explicit card events)
            control.CardClicked += (s, e) =>
            {
                var detail = _allMachineData.Where(m => m.line_production == e.LineName).ToList();
                new DetailForm(line, detail).ShowDialog();
            };

            return control;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dataLogger?.Close();
            _uiUpdateTimer?.Stop();
            base.OnFormClosing(e);
        }

        private List<MachineData> GetInitialMachineData()
        {
            var list = new List<MachineData>();
            foreach (var identity in MachineMapper.GetAllIdentities())
                list.Add(new MachineData { id = identity.Id, name = identity.Name, line_production = identity.Line });
            return list;
        }

        private Dictionary<string, LineSummary> GenerateSummary(List<MachineData> machines)
        {
            return machines.GroupBy(m => m.line_production)
                .ToDictionary(g => g.Key, g => new LineSummary(
                    g.Key,
                    g.Count(m => m.nilaiA0 == 1),
                    g.Count(m => m.nilaiA0 == 0),
                    g.Count()));
        }

        // Center each physical row of controls inside the FlowLayoutPanel.
        // This method runs on the Layout event and adjusts control.Left positions
        // so that each row's content is horizontally centered within the panel.
        private void FlowLayoutPanel1_Layout(object sender, LayoutEventArgs e)
        {
            try
            {
                var fl = this.flowLayoutPanel1;
                if (fl.Controls.Count == 0) return;

                int availableWidth = fl.ClientSize.Width - fl.Padding.Left - fl.Padding.Right;

                // collect visible controls in the order they are laid out
                var controls = fl.Controls.Cast<Control>().Where(c => c.Visible).ToList();

                // Responsive sizing: choose number of cards per row based on available width
                const int minCardWidth = 220; // smallest acceptable card width
                const int spacing = 16; // space to allocate between cards and at edges
                var sample = controls.FirstOrDefault();
                if (sample != null)
                {
                    int marginTotal = sample.Margin.Left + sample.Margin.Right;

                    // candidate per-row counts (we prefer 3 per row visually)
                    int[] candidates = new[] { 3, 4, 5, 2, 1 };
                    int chosenPerRow = 1;
                    int chosenWidth = sample.Width;

                    // find the candidate that produces the smallest leftover space (available - required)
                    // while keeping card width >= minCardWidth. This minimizes large right-side gaps.
                    int bestPerRow = -1;
                    int bestWidth = -1;
                    int bestLeftover = int.MaxValue;

                    foreach (var perRow in candidates)
                    {
                        int totalGaps = perRow + 1; // left + between + right
                        int candidateWidth = (availableWidth - (totalGaps * spacing) - (perRow * marginTotal)) / perRow;
                        if (candidateWidth < minCardWidth) continue;

                        int required = perRow * candidateWidth + (totalGaps * spacing) + (perRow * marginTotal);
                        int leftover = availableWidth - required; // >=0 by construction

                        if (leftover >= 0 && leftover < bestLeftover)
                        {
                            bestLeftover = leftover;
                            bestPerRow = perRow;
                            bestWidth = candidateWidth;
                        }
                    }

                    if (bestPerRow != -1)
                    {
                        chosenPerRow = bestPerRow;
                        chosenWidth = bestWidth;
                    }
                    else
                    {
                        // fallback: no candidate satisfied minCardWidth — choose perRow that minimizes overflow
                        int bestOverflow = int.MaxValue;
                        foreach (var perRow in candidates)
                        {
                            int totalGaps = perRow + 1;
                            int candidateWidth = (availableWidth - (totalGaps * spacing) - (perRow * marginTotal)) / perRow;
                            // compute overflow if any (negative leftover)
                            int required = perRow * candidateWidth + (totalGaps * spacing) + (perRow * marginTotal);
                            int overflow = Math.Abs(availableWidth - required);
                            if (overflow < bestOverflow)
                            {
                                bestOverflow = overflow;
                                bestPerRow = perRow;
                                bestWidth = Math.Max(160, candidateWidth);
                            }
                        }

                        chosenPerRow = bestPerRow > 0 ? bestPerRow : 1;
                        chosenWidth = bestWidth > 0 ? bestWidth : Math.Max(160, sample.Width);
                    }

                    // apply width to all cards, but do not grow above the designed card width to avoid one-per-row
                    const int defaultCardWidth = 300; // keep in sync with LineCardControl.Designer
                    chosenWidth = Math.Min(chosenWidth, defaultCardWidth);
                    foreach (var c in controls)
                    {
                        // only set if different to avoid excessive relayouts
                        if (c.Width != chosenWidth)
                            c.Width = chosenWidth;
                    }
                }
                var rows = new List<List<Control>>();

                const int rowTolerance = 10; // px tolerance to group controls on same row

                foreach (var ctrl in controls)
                {
                    // find existing row with similar Top
                    var row = rows.FirstOrDefault(r => Math.Abs(r[0].Top - ctrl.Top) <= rowTolerance);
                    if (row != null)
                        row.Add(ctrl);
                    else
                        rows.Add(new List<Control> { ctrl });
                }

                foreach (var row in rows)
                {
                    // compute total width of this row (including margins)
                    int rowWidth = 0;
                    foreach (var c in row)
                        rowWidth += c.Margin.Left + c.Width + c.Margin.Right;

                    int extra = availableWidth - rowWidth;
                    int gap = 0;
                    if (extra > 0)
                    {
                        // distribute extra space evenly into (n+1) gaps (left, between items, right)
                        gap = extra / (row.Count + 1);
                    }

                    int x = fl.Padding.Left + gap;
                    foreach (var c in row)
                    {
                        // set new position (keep Y as set by FlowLayoutPanel)
                        c.Left = x + c.Margin.Left;
                        x += c.Margin.Left + c.Width + c.Margin.Right + gap;
                    }
                }
            }
            catch { /* ignore layout errors */ }
        }
    }
}
