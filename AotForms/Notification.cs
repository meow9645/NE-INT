using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AotForms
{
    public partial class Notification : Form
    {
        // Fully qualify to avoid ambiguity with System.Threading.Timer
        private System.Windows.Forms.Timer timeoutTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer();

        // P/Invoke for display affinity (exclude from screen capture)
        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);
        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        private void ApplyCaptureExclusion()
        {
            IntPtr hWnd = this.Handle;
            if (hWnd == IntPtr.Zero)
                return;
            SetWindowDisplayAffinity(hWnd, WDA_EXCLUDEFROMCAPTURE);
        }

        public Notification(string message, Color neonColor)
        {
            InitializeComponent();

            lblMsg.Text = message;
            lblMsg.ForeColor = neonColor;

            this.BackColor = Color.FromArgb(25, 25, 25);
            this.Opacity = 0;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Width - this.Width - 10,
                Screen.PrimaryScreen.WorkingArea.Height - this.Height - 10
            );

            fadeTimer.Interval = 15;
            fadeTimer.Tick += (s, e) =>
            {
                if (this.Opacity < 0.85)
                    this.Opacity += 0.05;
                else
                    fadeTimer.Stop();
            };
            fadeTimer.Start();

            timeoutTimer.Interval = 3000;
            timeoutTimer.Tick += (s, e) => { this.Close(); };
            timeoutTimer.Start();

            ApplyCaptureExclusion();
        }

        public static void Show(string message, Color color)
        {
            Notification notif = new Notification(message, color);
            notif.Show();
        }

        public Notification()
        {
            InitializeComponent();
        }

        // No leftover event handlers – they were removed with the Guna2 controls
    }
}