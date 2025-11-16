using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace MachineMonitoringApp.Forms
{
    partial class LineCardControl
    {
        private IContainer components = null;

        private Guna2Panel cardPanel;
        private Label lblTitle;

        // top active / inactive
        private Label lblActiveValue;
        private Label lblActiveText;
        private Label lblInactiveValue;
        private Label lblInactiveText;

        // inner metrics box
        private Guna2Panel innerBox;
        private TableLayoutPanel tblMetrics;
        private Label lblCountLabel;
        private Label lblCountValue;
        private Label lblPartHrLabel;
        private Label lblPartHrValue;
        private Label lblCycleLabel;
        private Label lblCycleValue;
        private Label lblAvgCycleLabel;
        private Label lblAvgCycleValue;
        private Label lblDowntimeLabel;
        private Label lblDowntimeValue;
        private Label lblUptimeLabel;
        private Label lblUptimeValue;

        // bottom total
        private Label lblTotalValue;
        private Label lblTotalText;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();

            // Make user control DPI-aware so its internal layout scales correctly
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

            // instantiate controls
            this.cardPanel = new Guna2Panel();
            this.lblTitle = new Label();

            this.lblActiveValue = new Label();
            this.lblActiveText = new Label();
            this.lblInactiveValue = new Label();
            this.lblInactiveText = new Label();

            this.innerBox = new Guna2Panel();
            this.tblMetrics = new TableLayoutPanel();
            this.lblCountLabel = new Label();
            this.lblCountValue = new Label();
            this.lblPartHrLabel = new Label();
            this.lblPartHrValue = new Label();
            this.lblCycleLabel = new Label();
            this.lblCycleValue = new Label();
            this.lblAvgCycleLabel = new Label();
            this.lblAvgCycleValue = new Label();
            this.lblDowntimeLabel = new Label();
            this.lblDowntimeValue = new Label();
            this.lblUptimeLabel = new Label();
            this.lblUptimeValue = new Label();

            this.lblTotalValue = new Label();
            this.lblTotalText = new Label();

            // cardPanel
            // Use a fixed Size + MinimumSize (not Dock=Fill) so FlowLayoutPanel will
            // arrange cards in a grid-like manner and cards won't stretch/overlap
            // when DPI or parent size changes.
            this.cardPanel.Size = new Size(300, 330);
            this.cardPanel.MinimumSize = new Size(300, 330);
            this.cardPanel.BorderRadius = 12;
            this.cardPanel.FillColor = Color.White;
            this.cardPanel.Margin = new Padding(12);
            this.cardPanel.Cursor = Cursors.Default;
            // stronger but subtle shadow to create a raised effect
            this.cardPanel.ShadowDecoration.Enabled = true;
            this.cardPanel.ShadowDecoration.Depth = 14;
            this.cardPanel.BorderColor = Color.FromArgb(230, 230, 230);
            this.cardPanel.BorderThickness = 1;
            this.cardPanel.Padding = new Padding(12);

            // lblTitle
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold);
            this.lblTitle.Height = 36;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Margin = new Padding(0, 4, 0, 8);

            // top row: active / inactive
            Action<Label> topValueStyle = (lbl) =>
            {
                lbl.Font = new Font("Consolas", 18, FontStyle.Bold);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Dock = DockStyle.Top;
                lbl.Height = 28;
                lbl.AutoSize = false;
            };
            Action<Label> topTextStyle = (lbl) =>
            {
                lbl.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                lbl.ForeColor = Color.FromArgb(110, 110, 110);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Dock = DockStyle.Top;
                lbl.Height = 28;
                lbl.AutoSize = false;
            };

            topValueStyle(this.lblActiveValue);
            topTextStyle(this.lblActiveText);
            topValueStyle(this.lblInactiveValue);
            topTextStyle(this.lblInactiveText);

            this.lblActiveText.Text = "Active\nMachine";
            this.lblInactiveText.Text = "Inactive\nMachine";

            // innerBox
            // inner box slightly offset to appear inset
            this.innerBox.FillColor = Color.FromArgb(248, 249, 250);
            this.innerBox.BorderColor = Color.FromArgb(225, 225, 225);
            this.innerBox.BorderThickness = 1;
            this.innerBox.BorderRadius = 6;
            this.innerBox.Padding = new Padding(10);
            this.innerBox.Margin = new Padding(6, 8, 6, 8);

            // tblMetrics: 2 columns x 4 rows
            this.tblMetrics.Dock = DockStyle.Fill;
            this.tblMetrics.ColumnCount = 2;
            this.tblMetrics.RowCount = 4;
            this.tblMetrics.ColumnStyles.Clear();
            this.tblMetrics.RowStyles.Clear();
            this.tblMetrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tblMetrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            for (int i = 0; i < 4; i++) this.tblMetrics.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            Action<Label> metricLabelStyle = (lbl) =>
            {
                lbl.Font = new Font("Segoe UI", 8, FontStyle.Regular);
                lbl.ForeColor = Color.FromArgb(120, 120, 120);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Dock = DockStyle.Fill;
                lbl.AutoSize = false;
            };
            Action<Label> metricValueStyle = (lbl) =>
            {
                lbl.Font = new Font("Consolas", 12, FontStyle.Bold);
                lbl.ForeColor = Color.FromArgb(60, 60, 60);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Dock = DockStyle.Fill;
                lbl.AutoSize = false;
            };

            metricLabelStyle(this.lblCountLabel);
            metricValueStyle(this.lblCountValue);
            metricLabelStyle(this.lblPartHrLabel);
            metricValueStyle(this.lblPartHrValue);
            metricLabelStyle(this.lblCycleLabel);
            metricValueStyle(this.lblCycleValue);
            metricLabelStyle(this.lblAvgCycleLabel);
            metricValueStyle(this.lblAvgCycleValue);
            metricLabelStyle(this.lblDowntimeLabel);
            metricValueStyle(this.lblDowntimeValue);
            metricLabelStyle(this.lblUptimeLabel);
            metricValueStyle(this.lblUptimeValue);

            this.lblCountLabel.Text = "COUNT";
            this.lblCountValue.Text = "0";
            this.lblPartHrLabel.Text = "PART/HR";
            this.lblPartHrValue.Text = "0.0";
            this.lblCycleLabel.Text = "CYCLE";
            this.lblCycleValue.Text = "0.0s";
            this.lblAvgCycleLabel.Text = "AVG CYCLE";
            this.lblAvgCycleValue.Text = "0.0s";
            this.lblDowntimeLabel.Text = "DOWNTIME";
            this.lblDowntimeValue.Text = "0.0 (0.0%)";
            this.lblUptimeLabel.Text = "UPTIME";
            this.lblUptimeValue.Text = "0.0 (0.0%)";

            // bottom total
            this.lblTotalValue.Font = new Font("Consolas", 18, FontStyle.Bold);
            this.lblTotalValue.ForeColor = Color.FromArgb(40, 40, 40);
            this.lblTotalValue.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTotalValue.Dock = DockStyle.Top;
            this.lblTotalValue.Height = 28;

            this.lblTotalText.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            this.lblTotalText.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblTotalText.Text = "Total Machine";
            this.lblTotalText.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTotalText.Dock = DockStyle.Top;

            // assemble inner metrics
            this.tblMetrics.Controls.Add(this.lblCountLabel, 0, 0);
            this.tblMetrics.Controls.Add(this.lblPartHrLabel, 1, 0);
            this.tblMetrics.Controls.Add(this.lblCountValue, 0, 1);
            this.tblMetrics.Controls.Add(this.lblPartHrValue, 1, 1);
            this.tblMetrics.Controls.Add(this.lblCycleLabel, 0, 2);
            this.tblMetrics.Controls.Add(this.lblAvgCycleLabel, 1, 2);
            this.tblMetrics.Controls.Add(this.lblCycleValue, 0, 3);
            this.tblMetrics.Controls.Add(this.lblAvgCycleValue, 1, 3);

            this.innerBox.Controls.Add(this.tblMetrics);

            // Compose final card: topRow, innerBox, bottomArea
            var topRow = new TableLayoutPanel();
            topRow.Dock = DockStyle.Top;
            topRow.ColumnCount = 2;
            topRow.RowCount = 2;
            topRow.Height = 64;
            topRow.ColumnStyles.Clear();
            topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            topRow.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            topRow.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            topRow.Controls.Add(this.lblActiveValue, 0, 0);
            topRow.Controls.Add(this.lblInactiveValue, 1, 0);
            topRow.Controls.Add(this.lblActiveText, 0, 1);
            topRow.Controls.Add(this.lblInactiveText, 1, 1);

            this.innerBox.Dock = DockStyle.Top;
            this.innerBox.Height = 160;

            var bottomArea = new Panel();
            bottomArea.Dock = DockStyle.Bottom;
            bottomArea.Height = 56;
            bottomArea.Controls.Add(this.lblTotalText);
            bottomArea.Controls.Add(this.lblTotalValue);

            // add ordered controls to cardPanel (top to bottom)
            // put title at very top, then topRow, innerBox, and bottom area so the card reads naturally
            this.cardPanel.Controls.Add(bottomArea);
            this.cardPanel.Controls.Add(this.innerBox);
            this.cardPanel.Controls.Add(topRow);
            this.cardPanel.Controls.Add(this.lblTitle);
            this.Controls.Add(this.cardPanel);
            // Set a sensible default/minimum size for the whole control. Allow the
            // parent FlowLayoutPanel to position/size cards; MinimumSize prevents
            // them from becoming too small on high-DPI or small-screen setups.
            this.MinimumSize = new Size(320, 330);
            this.Size = new Size(320, 330);
        }
    }
}

