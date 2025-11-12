namespace MachineMonitoringApp.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private FlowLayoutPanel flowLayoutPanel1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new FlowLayoutPanel();
            this.SuspendLayout();

            this.flowLayoutPanel1.Dock = DockStyle.Fill;
            this.flowLayoutPanel1.BackColor = Color.FromArgb(165, 165, 165);
            this.flowLayoutPanel1.Padding = new Padding(30);
            this.flowLayoutPanel1.AutoScroll = true;

            this.Controls.Add(this.flowLayoutPanel1);

            this.Text = "Monitoring Dashboard";
            this.ClientSize = new Size(1024, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }
    }
}
