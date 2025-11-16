using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Guna.UI2.WinForms;
using System.IO;

namespace MachineMonitoringApp.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel contentPanel;
        private FlowLayoutPanel flowLayoutPanel1;
        private Guna2Panel headerPanel;
        private Label lblLogo;
        private Label lblTitle;
        private Label lblTime;
        private Guna.UI2.WinForms.Guna2PictureBox picLogo;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Make form scale based on monitor DPI so layout stays consistent
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

            // === HEADER PANEL ===
            this.headerPanel = new Guna2Panel();
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Height = 88;
            this.headerPanel.FillColor = Color.FromArgb(37, 112, 159);
            this.headerPanel.ShadowDecoration.Enabled = false;
            this.headerPanel.UseTransparentBackground = false;

            // === LOGO ===
            this.lblLogo = new Label();
            this.lblLogo.Text = "🏭";
            this.lblLogo.ForeColor = Color.White;
            this.lblLogo.Font = new Font("Segoe UI Emoji", 28, FontStyle.Regular);
            this.lblLogo.Dock = DockStyle.Left;
            this.lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            this.lblLogo.Width = 120;
            this.lblLogo.Padding = new Padding(10, 0, 0, 0);

            this.picLogo = new Guna.UI2.WinForms.Guna2PictureBox();
            this.picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            this.picLogo.Width = 72;
            this.picLogo.Height = 72;
            this.picLogo.Dock = DockStyle.Left;
            this.picLogo.Margin = new Padding(6);
            this.picLogo.BackColor = Color.Transparent;
            // load image safely (avoid crash if file missing). Try several common file names/locations.
            try
            {
                string[] candidates = new[] { "CCI.jpg", "CCI.png", "logo.jpg", "logo.png" };
                string found = null;
                foreach (var name in candidates)
                {
                    var p = Path.Combine(Application.StartupPath, "Assets", name);
                    if (File.Exists(p)) { found = p; break; }
                    p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? Application.StartupPath, "Assets", name);
                    if (File.Exists(p)) { found = p; break; }
                }
                if (found != null)
                {
                    this.picLogo.Image = Image.FromFile(found);
                    // hide emoji fallback
                    this.lblLogo.Visible = false;
                    this.picLogo.Visible = true;
                }
                else
                {
                    this.picLogo.Visible = false; // keep emoji label as fallback
                }
            }
            catch
            {
                this.picLogo.Visible = false;
            }

            // === TITLE ===
            this.lblTitle = new Label();
            this.lblTitle.Text = "MACHINE UPTIME MONITORING"; // match screenshot title
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            this.lblTitle.Dock = DockStyle.Fill;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Padding = new Padding(0, 0, 0, 0);

            // === TIME LABEL (inside rounded date panel) ===
            this.lblTime = new Label();
            this.lblTime.ForeColor = Color.FromArgb(30, 30, 30);
            this.lblTime.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            this.lblTime.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTime.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy\nHH:mm:ss");

            // small rounded panel to hold the date/time on the right
            var datePanel = new Guna2Panel();
            datePanel.Width = 220;
            datePanel.Height = 56;
            datePanel.Dock = DockStyle.Right;
            datePanel.Margin = new Padding(10);
            datePanel.Padding = new Padding(6);
            datePanel.BorderRadius = 8;
            datePanel.FillColor = Color.FromArgb(209, 227, 240);
            datePanel.ShadowDecoration.Enabled = false;
            datePanel.Controls.Add(this.lblTime);
            this.lblTime.Dock = DockStyle.Fill;

            // build header using left/center/right panels for cleaner layout
            var leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = 160;
            leftPanel.Padding = new Padding(12);
            leftPanel.BackColor = Color.Transparent;

            var centerPanel = new Panel();
            centerPanel.Dock = DockStyle.Fill;
            centerPanel.BackColor = Color.Transparent;

            var rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Right;
            rightPanel.Width = 260;
            rightPanel.Padding = new Padding(8);
            rightPanel.BackColor = Color.Transparent;

            // prepare logo inside left panel
            this.lblLogo.Dock = DockStyle.Left;
            this.lblLogo.Width = 72;
            leftPanel.Controls.Add(this.picLogo);

            // // add a compact Download button under the logo in the left panel
            // var btnDownload = new Guna.UI2.WinForms.Guna2Button();
            // btnDownload.Text = "Download Excel";
            // btnDownload.Height = 30;
            // btnDownload.Width = 130;
            // btnDownload.FillColor = Color.FromArgb(27, 127, 207);
            // btnDownload.ForeColor = Color.White;
            // btnDownload.BorderRadius = 6;
            // btnDownload.Margin = new Padding(6, 8, 6, 6);
            // place button near top-left inside left panel
            // btnDownload.Location = new Point(8, leftPanel.Padding.Top);
            // btnDownload.ShadowDecoration.Enabled = false;
            // leftPanel.Controls.Add(btnDownload);

            // center the title in the center panel
            this.lblTitle.Dock = DockStyle.Fill;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            centerPanel.Controls.Add(this.lblTitle);

            // put the datePanel into rightPanel
            datePanel.Dock = DockStyle.Fill;
            rightPanel.Controls.Add(datePanel);

            // add panels to header (center first to occupy middle)
            this.headerPanel.Controls.Add(centerPanel);
            this.headerPanel.Controls.Add(rightPanel);
            this.headerPanel.Controls.Add(leftPanel);
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTime.BackColor = Color.Transparent;
            this.lblLogo.BackColor = Color.Transparent;

            // === CONTENT PANEL (container untuk men-center-kan FlowLayoutPanel) ===
            this.contentPanel = new Panel();
            this.contentPanel.Dock = DockStyle.Fill;
            // darker gray body background like screenshot
            this.contentPanel.BackColor = Color.FromArgb(180, 180, 180);
            // this.contentPanel.Padding = new Padding(16);
            // this.contentPanel.AutoScroll = true;

            // no inline download button here (moved to header)

            // === FLOWLAYOUTPANEL (CARD AREA) ===
            this.flowLayoutPanel1 = new FlowLayoutPanel();
            // Dock Fill agar FlowLayoutPanel mengisi area dan membungkus item ke baris baru
            this.flowLayoutPanel1.Dock = DockStyle.Fill;
            this.flowLayoutPanel1.AutoSize = false;
            // make flow panel background match content so there is no white strip
            this.flowLayoutPanel1.BackColor = this.contentPanel.BackColor;
            this.flowLayoutPanel1.Padding = new Padding(0, 10, 0, 0);
            // ensure consistent left-to-right flow and disable wrapping so items flow in a single row
            this.flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            this.flowLayoutPanel1.WrapContents = true; // wrap ke baris baru sehingga scrolling vertikal muncul
            this.flowLayoutPanel1.AutoScroll = true; // flow panel sendiri menampilkan scrollbar vertikal saat overflow
            this.flowLayoutPanel1.Margin = new Padding(12);
            // tidak perlu positioning manual/centering di Resize karena FlowLayoutPanel dock = Fill

            // Reduce flicker: enable double-buffering for form and flowLayoutPanel
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
            this.flowLayoutPanel1?.GetType()
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(this.flowLayoutPanel1, true, null);

            // === FORM ===
            this.Text = "Machine Monitoring Dashboard";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // tempatkan flowLayoutPanel ke dalam contentPanel, lalu tambahkan keduanya ke Form
            this.contentPanel.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerPanel);
        }
    }
}
