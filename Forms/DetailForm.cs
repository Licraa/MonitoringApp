using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MachineMonitoringApp.Models;

namespace MachineMonitoringApp.Forms
{
    public partial class DetailForm : Form
    {
        private LineSummary _lineSummary;
        private List<MachineData> _machines;
        private TableLayoutPanel _layout;

        public DetailForm(LineSummary summary, List<MachineData> machines)
        {
            InitializeComponent();
            _lineSummary = summary;
            _machines = machines;

            // 🔹 Setting tampilan full layar
            this.Text = $"Detail Line - {_lineSummary.LineName}";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = Color.WhiteSmoke;
            this.Padding = new Padding(15);

            SetupLayout();
            RenderMachineCards();
        }

        private void SetupLayout()
        {
            _layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                BackColor = Color.WhiteSmoke,
                AutoScroll = true,
                Padding = new Padding(10),
            };

            // 🔹 Set ukuran kolom & baris
            for (int i = 0; i < 3; i++)
            {
                _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
                _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            }

            // 🔹 Tambahkan ke Form
            this.Controls.Clear();
            this.Controls.Add(_layout);
        }

        private void RenderMachineCards()
        {
            _layout.SuspendLayout();
            _layout.Controls.Clear();

            if (_machines == null || _machines.Count == 0)
            {
                Label emptyLabel = new Label
                {
                    Text = "Tidak ada data mesin untuk line ini.",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 16, FontStyle.Italic),
                    ForeColor = Color.Gray
                };
                _layout.Controls.Add(emptyLabel, 1, 1);
                _layout.ResumeLayout();
                return;
            }

            int totalCards = Math.Min(_machines.Count, 9); // Maks 9 mesin (3x3)
            for (int i = 0; i < totalCards; i++)
            {
                var machine = _machines[i];
                var card = CreateMachineCard(machine);
                _layout.Controls.Add(card, i % 3, i / 3);
            }

            _layout.ResumeLayout();
        }

        private Panel CreateMachineCard(MachineData m)
        {
            var card = new Panel
            {
                Margin = new Padding(10),
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Judul mesin
            var lblTitle = new Label
            {
                Text = $"{m.name}",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Height = 45,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 152, 219)
            };

            // === Detail tabel (gunakan TableLayout di dalam card) ===
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));

            AddRow(tbl, "Line", m.line_production);
            AddRow(tbl, "Part Hours", m.parthours);
            AddRow(tbl, "Uptime", m.uptime);
            AddRow(tbl, "Downtime", m.durasiTerakhirA4);
            AddRow(tbl, "Average", m.ratarataTerakhirA4);
            AddRow(tbl, "Updated", m.LastUpdated.ToString("HH:mm:ss"));

            // Footer status
            var lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Text = m.nilaiA0 == 1 ? "ACTIVE" : "INACTIVE",
                BackColor = m.nilaiA0 == 1 ? Color.SeaGreen : Color.IndianRed
            };

            // Urutan penempatan
            card.Controls.Add(tbl);
            card.Controls.Add(lblStatus);
            card.Controls.Add(lblTitle);

            // Efek hover
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(245, 245, 245);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            return card;
        }

        private void AddRow(TableLayoutPanel tbl, string label, string value)
        {
            var lblKey = new Label
            {
                Text = label,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            var lblValue = new Label
            {
                Text = string.IsNullOrWhiteSpace(value) ? "-" : value,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            int row = tbl.RowCount - 1;
            tbl.Controls.Add(lblKey, 0, row);
            tbl.Controls.Add(lblValue, 1, row);
            tbl.RowCount++;
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }
    }
}
