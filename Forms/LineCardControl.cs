using System;
using System.Windows.Forms;
using System.Drawing;
using MachineMonitoringApp.Models;

namespace MachineMonitoringApp.Forms
{
    public partial class LineCardControl : UserControl
    {
        private string _lineName;

        // Raised when the user clicks anywhere on the card. Event passes the current LineName.
        public class CardClickedEventArgs : EventArgs
        {
            public string LineName { get; }
            public CardClickedEventArgs(string lineName) => LineName = lineName;
        }

        public event EventHandler<CardClickedEventArgs>? CardClicked;

    // debounce guard to avoid multiple rapid invocations when multiple child controls
    // raise Click events for the same user action.
    private DateTime _lastCardClick = DateTime.MinValue;

        public LineCardControl()
        {
            // Reduce flicker by enabling double-buffering for this UserControl
            this.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer |
                          System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                          System.Windows.Forms.ControlStyles.UserPaint, true);
            this.UpdateStyles();

            InitializeComponent();

            // Give the control a consistent outer margin (helps FlowLayout spacing)
            this.Margin = new System.Windows.Forms.Padding(18, 12, 18, 12);

            // Wire recursive click forwarding so clicks anywhere on the card (labels, panels, etc.)
            // both raise the standard Click event and the CardClicked event with the current LineName.
            WireClickForwarding();

            // Also try to set DoubleBuffered on internal panel (cardPanel) via reflection to reduce flicker
            try
            {
                var pi = this.GetType().GetField("cardPanel", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                var panel = pi?.GetValue(this) as System.Windows.Forms.Control;
                if (panel != null)
                {
                    var prop = panel.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    prop?.SetValue(panel, true, null);
                }
            }
            catch { }
        }

        private void WireClickForwarding()
        {
            EventHandler handler = (s, e) =>
            {
                // simple debounce: ignore if a CardClicked was fired very recently
                var now = DateTime.UtcNow;
                if ((now - _lastCardClick).TotalMilliseconds < 350)
                    return;
                _lastCardClick = now;

                try { this.OnClick(e); } catch { }
                CardClicked?.Invoke(this, new CardClickedEventArgs(this._lineName));
            };

            void AttachRecursive(Control parent)
            {
                parent.Click += handler;
                foreach (Control child in parent.Controls)
                    AttachRecursive(child);
            }

            if (this.cardPanel != null)
                AttachRecursive(this.cardPanel);

            // We avoid attaching a separate this.Click handler to prevent duplicate invocations;
            // the recursive handlers above will capture clicks on children and the panel.
        }

        public string LineName => _lineName;

        public void SetSummary(LineSummary summary)
        {
            if (summary == null) return;
            _lineName = summary.LineName;
            this.lblTitle.Text = summary.LineName;
            this.lblActiveValue.Text = summary.Active.ToString();
            this.lblInactiveValue.Text = summary.Inactive.ToString();
            this.lblTotalValue.Text = summary.Total.ToString();

            // Accent color based on active
            var accent = summary.Active > 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(100, 100, 100);
            this.lblTitle.ForeColor = accent;

            // Per-value coloring
            this.lblActiveValue.ForeColor = summary.Active > 0 ? Color.FromArgb(34, 139, 34) : Color.FromArgb(130, 130, 130);
            this.lblInactiveValue.ForeColor = Color.FromArgb(150, 150, 150);
            this.lblTotalValue.ForeColor = Color.FromArgb(60, 60, 60);
        }
    }
}
