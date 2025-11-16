
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MachineMonitoringApp.Models;
using Guna.UI2.WinForms;
using System.IO;
namespace MachineMonitoringApp.Forms

{
    partial class DetailForm
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
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // this.SuspendLayout();

            // === HEADER PANEL ===
            this.headerPanel = new Guna2Panel();
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Height = 88;
            this.headerPanel.FillColor = Color.FromArgb(37, 112, 159);
            this.headerPanel.ShadowDecoration.Enabled = false;
            // make header flush with the form edges so it doesn't look "floating"
            this.headerPanel.Margin = new Padding(0);
            this.headerPanel.BorderRadius = 0;
            this.headerPanel.UseTransparentBackground = false;

            // Ensure form scales properly on different monitor DPI settings
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

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
                    this.lblLogo.Visible = false;
                    this.picLogo.Visible = true;
                }
                else
                    this.picLogo.Visible = false;
            }
            catch { this.picLogo.Visible = false; }

            // === TITLE ===
            this.lblTitle = new Label();
            this.lblTitle.Text = "MACHINE UPTIME MONITORING";
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            this.lblTitle.Dock = DockStyle.Fill;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Padding = new Padding(0, 0, 0, 0);

            var centerPanel = new Panel();
            centerPanel.Dock = DockStyle.Fill;
            centerPanel.BackColor = Color.Transparent;

            this.lblTitle.Dock = DockStyle.Fill;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            centerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(centerPanel);
            this.lblTitle.BackColor = Color.Transparent;


            this.lblTime = new Label();
            this.lblTime.ForeColor = Color.FromArgb(30, 30, 30);
            this.lblTime.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            this.lblTime.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTime.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy\nHH:mm:ss");

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


            var rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Right;
            rightPanel.Width = 260;
            rightPanel.Padding = new Padding(8);
            rightPanel.BackColor = Color.Transparent;

            datePanel.Dock = DockStyle.Fill;
            rightPanel.Controls.Add(datePanel);
            this.headerPanel.Controls.Add(rightPanel);
            this.lblTime.BackColor = Color.Transparent;

            // Tambahkan semua komponen ke header
            // this.headerPanel.Controls.Add(this.lblTitle);
            // this.headerPanel.Controls.Add(this.lblTime);
            this.headerPanel.Controls.Add(this.picLogo);
            this.lblLogo.BackColor = Color.Transparent;


            // === MAIN LAYOUT ===
            this._layout = new System.Windows.Forms.TableLayoutPanel();
            this._layout.Dock = DockStyle.Fill;
            // this._layout.ColumnCount = 3;
            // this._layout.RowCount = 2;
            this._layout.BackColor = Color.FromArgb(180, 180, 180);
            this._layout.AutoScroll = true;
            // move the visual padding to the content area so header stays flush
            // this._layout.Padding = new Padding(150,0,0,0);

            // Add controls to the form: main layout first (fills), then header on top
            this.Controls.Add(this._layout);
            this.Controls.Add(this.headerPanel);

            // this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            // this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // this.ClientSize = new System.Drawing.Size(1200, 800);
            // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            // this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Name = "DetailForm";
            // this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            // this.Text = "Detail Line";
            this.ResumeLayout(false);
        }
    }
}
