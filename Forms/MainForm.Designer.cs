namespace MachineMonitoringApp.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label headerLabel;
        private Label footerLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerLabel = new Label();
            this.footerLabel = new Label();
            this.flowLayoutPanel1 = new FlowLayoutPanel();
            this.SuspendLayout();

            // ========================================
            // === PERUBAHAN HEADER (Lebih Profesional) ===
            // ========================================
            this.headerLabel.Text = "MACHINE UPTIME MONITORING";
            this.headerLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold); // Ukuran font sedikit lebih kecil
            this.headerLabel.ForeColor = Color.White;
            this.headerLabel.BackColor = Color.FromArgb(44, 62, 80); // Warna Dark Slate/Charcoal
            this.headerLabel.Dock = DockStyle.Top;
            this.headerLabel.Height = 60;
            this.headerLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.headerLabel.Padding = new Padding(10, 0, 10, 0);

            // ======================================
            // === PERUBAHAN FOOTER (Konsisten) ===
            // ======================================
            this.footerLabel.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy HH:mm:ss");
            this.footerLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular); // Font lebih kecil
            this.footerLabel.ForeColor = Color.White;
            this.footerLabel.BackColor = Color.FromArgb(44, 62, 80); // Warna Dark Slate/Charcoal
            this.footerLabel.Dock = DockStyle.Bottom;
            this.footerLabel.Height = 35; // Sedikit lebih tipis
            this.footerLabel.TextAlign = ContentAlignment.MiddleRight;
            this.footerLabel.Padding = new Padding(0, 0, 20, 0); // Padding di kanan

            // ======================================================
            // === PERUBAHAN FLOWLAYOUTPANEL (Latar Belakang) ===
            // ======================================================
            this.flowLayoutPanel1.Dock = DockStyle.Fill;
            this.flowLayoutPanel1.BackColor = Color.FromArgb(236, 240, 241); // Warna "Cloud" (Abu-abu sangat muda)
            this.flowLayoutPanel1.Padding = new Padding(25);
            this.flowLayoutPanel1.AutoScroll = true;

            // ========================================
            // === PENGATURAN MAINFORM ===
            // ========================================
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.footerLabel);
            this.Text = "Monitoring Dashboard";
            this.ClientSize = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;
            this.MinimumSize = new Size(1000, 700);
            this.BackColor = Color.FromArgb(236, 240, 241); // Samakan dengan FlowLayoutPanel

            this.ResumeLayout(false);
        }
    }
}
