using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MachineMonitoringApp.Models;
using Guna.UI2.WinForms;

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

            // ensure the detail window opens maximized
            this.Text = $"Detail Line - {_lineSummary.LineName}";
            this.WindowState = FormWindowState.Maximized;

            // _layout is created in InitializeComponent (designer); move padding to the layout
            if (_layout != null)
                _layout.Padding = new Padding(100,0,0,0);

            // render cards into the designer-provided layout
            RenderMachineCards();
        }

        private void SetupLayout()
        {
            // SetupLayout is now handled by the designer (DetailForm.Designer.cs).
            // This method is kept for backward compatibility but intentionally left empty.
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

            // Deduplicate machines by id in case the source list contains duplicates
            var machinesToShow = _machines
                .GroupBy(m => m.id)
                .Select(g => g.First())
                .ToList();

            int totalCards = Math.Min(machinesToShow.Count, 6); // Maks 6 kartu (3x2)
            for (int i = 0; i < totalCards; i++)
            {
                var machine = machinesToShow[i];
                var card = CreateMachineCard(machine);
                _layout.Controls.Add(card, i % 3, i / 3);
            }

            _layout.ResumeLayout();
        }

        private Control CreateMachineCard(MachineData m)
        {
            // Use Guna2Panel for nicer visuals
            var card = new Guna2Panel
            {
                Margin = new Padding(12),
                Size = new Size(360, 420),
                BackColor = Color.Transparent,
                // keep card background white and use a separate header panel for the red header
                FillColor = Color.White,
                BorderRadius = 8,
                ShadowDecoration = { Enabled = true, Depth = 8 },
                BorderColor = Color.FromArgb(230, 200, 200),
                BorderThickness = 1,
                // Padding = new Padding(10,0,0,0)
            };

            // Header (top bar with machine name) - use Guna2Panel so corners blend nicely
            var header = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                FillColor = Color.FromArgb(180, 50, 50),
                BorderRadius = 8,
                ShadowDecoration = { Enabled = false },
                Padding = new Padding(8)
            };
            var lblTitle = new Label
            {
                Text = m.name,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold),
                ForeColor = Color.Black
            };
            header.Controls.Add(lblTitle);

            // Main inner white panel (content area)
            var inner = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(255, 249, 249),
                BorderRadius = 6,
                Margin = new Padding(10),
                // increase top padding so the inner content sits clearly below the header
                Padding = new Padding(8, 24, 8, 8),
                BorderColor = Color.FromArgb(220, 180, 180),
                BorderThickness = 1
            };

            // Top info table: LINE | PROCESS | LAST UPDATE
            var topInfo = new TableLayoutPanel { Dock = DockStyle.Top, Height = 64, ColumnCount = 3, RowCount = 1, BackColor = Color.Transparent , Padding = new Padding(0,5,0,0)};
            topInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            topInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            topInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));

            var lblLine = new Label { Text = "LINE", Font = new Font("Segoe UI", 8, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            var lblProcess = new Label { Text = "PROCESS", Font = new Font("Segoe UI", 8, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            var lblUpdate = new Label { Text = "LAST UPDATE", Font = new Font("Segoe UI", 8, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };

            var valLine = new Label { Text = m.line_production ?? "-", Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, BackColor = Color.Transparent };
            var valProcess = new Label { Text = "Packing", Font = new Font("Segoe UI", 10, FontStyle.Regular), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, BackColor = Color.Transparent };
            var valUpdate = new Label { Text = m.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss"), Font = new Font("Segoe UI", 9, FontStyle.Regular), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, BackColor = Color.Transparent };

            var topHeaderTbl = new TableLayoutPanel { Dock = DockStyle.Top, Height = 28, ColumnCount = 3 };
            topHeaderTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            topHeaderTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            topHeaderTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            topHeaderTbl.Controls.Add(lblLine, 0, 0);
            topHeaderTbl.Controls.Add(lblProcess, 1, 0);
            topHeaderTbl.Controls.Add(lblUpdate, 2, 0);
            topHeaderTbl.Margin = new Padding(0, 6, 0, 0);

            var topValueTbl = new TableLayoutPanel { Dock = DockStyle.Top, Height = 28, ColumnCount = 3, Padding = new Padding(2) };
            topValueTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            topValueTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            topValueTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            topValueTbl.Controls.Add(valLine, 0, 0);
            topValueTbl.Controls.Add(valProcess, 1, 0);
            topValueTbl.Controls.Add(valUpdate, 2, 0);

            // Metrics area: PART/HR | CYCLE | AVG CYCLE (simple horizontal box)
            var metricsBox = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.FromArgb(255, 240, 240), Padding = new Padding(6), Margin = new Padding(0,8,0,8) };
            var metricsTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2 };
            metricsTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            metricsTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            metricsTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            metricsTbl.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            metricsTbl.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            metricsTbl.Controls.Add(new Label { Text = "PART/HR", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 0);
            metricsTbl.Controls.Add(new Label { Text = "CYCLE", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 0);
            metricsTbl.Controls.Add(new Label { Text = "AVG CYCLE", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 2, 0);

            metricsTbl.Controls.Add(new Label { Text = m.parthours ?? "0", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Consolas", 14, FontStyle.Bold) }, 0, 1);
            metricsTbl.Controls.Add(new Label { Text = m.nilaiTerakhirA2 ?? "0 s", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Consolas", 14, FontStyle.Bold) }, 1, 1);
            metricsTbl.Controls.Add(new Label { Text = m.ratarataTerakhirA4 ?? "0 s", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Consolas", 14, FontStyle.Bold) }, 2, 1);

            metricsBox.Controls.Add(metricsTbl);

            // Shift rows: create 3 compact horizontal tables
            Control CreateShiftRow(string shiftName)
            {
                var panel = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.FromArgb(255, 249, 249), Padding = new Padding(0,11,0,0) };
                var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 2 };
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                // header row
                tbl.Controls.Add(new Label { Text = "SHIFT", Font = new Font("Segoe UI", 8, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 0, 0);
                tbl.Controls.Add(new Label { Text = "COUNT", Font = new Font("Segoe UI", 8, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 1, 0);
                tbl.Controls.Add(new Label { Text = "DOWNTIME", Font = new Font("Segoe UI", 8, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 2, 0);
                tbl.Controls.Add(new Label { Text = "UPTIME", Font = new Font("Segoe UI", 8, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 3, 0);
                // value row
                tbl.Controls.Add(new Label { Text = shiftName, Font = new Font("Segoe UI", 9, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 0, 1);
                tbl.Controls.Add(new Label { Text = "0", Font = new Font("Consolas", 12, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 1, 1);
                tbl.Controls.Add(new Label { Text = "00:00:00\n0%", Font = new Font("Segoe UI", 9, FontStyle.Regular), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 2, 1);
                tbl.Controls.Add(new Label { Text = "00:00:00\n0%", Font = new Font("Segoe UI", 9, FontStyle.Regular), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 3, 1);
                panel.Controls.Add(tbl);
                return panel;
            }

            // remark at bottom
            var remarkPanel = new Panel { Dock = DockStyle.Bottom, Height = 36, BackColor = Color.Transparent };
            var lblRemark = new Label { Text = "** Remark :", Dock = DockStyle.Left, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(100, 20, 20), Width = 120 };
            var lblRemarkVal = new Label { Text = "-", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Regular), ForeColor = Color.FromArgb(60, 60, 60) };
            remarkPanel.Controls.Add(lblRemarkVal);
            remarkPanel.Controls.Add(lblRemark);

            // assemble content in inner panel
            inner.Controls.Add(remarkPanel);
            inner.Controls.Add(CreateShiftRow("3rd"));
            inner.Controls.Add(CreateShiftRow("2nd"));
            inner.Controls.Add(CreateShiftRow("1st"));
            inner.Controls.Add(metricsBox);
            inner.Controls.Add(topValueTbl);
            inner.Controls.Add(topHeaderTbl);

            // add header first, then inner (inner Dock=Fill will sit below header automatically)
            card.Controls.Add(header);
            card.Controls.Add(inner);

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
