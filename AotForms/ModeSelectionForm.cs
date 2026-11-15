using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AotForms
{
    public partial class ModeSelectionForm : Form
    {
        public bool StreamerModeSelected { get; private set; }

        private readonly System.Windows.Forms.Timer _animationTimer;
        private int _colorIndex = 0;
        private readonly Color[] _colors = new Color[]
        {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Lime,
            Color.Cyan,
            Color.DodgerBlue,
            Color.Magenta,
            Color.White
        };

        // P/Invoke for display affinity (only essential)
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

        public ModeSelectionForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            _animationTimer = new System.Windows.Forms.Timer();
            _animationTimer.Interval = 2500;
            _animationTimer.Tick += (s, e) =>
            {
                if (guna2HtmlLabel1 != null && !guna2HtmlLabel1.IsDisposed)
                {
                    guna2HtmlLabel1.ForeColor = _colors[_colorIndex];
                    _colorIndex = (_colorIndex + 1) % _colors.Length;
                }
            };
            this.Load += (s, e) =>
            {
                _animationTimer.Start();
                ApplyCaptureExclusion();
            };
        }

        private void btnStreamer_Click(object sender, EventArgs e)
        {
            StreamerModeSelected = true;

            // Show ShortcutForm via its singleton instance
            ShortcutForm.Instance.Show();

            // Close the current form, but be careful:
            // If this form is the main application form, closing it will terminate the app.
            // You might want to use this.Hide() instead to keep the app running.
            this.Hide(); // or this.Close() if it's not the main form
        }

        private void btnGUI_Click(object sender, EventArgs e)
        {
            StreamerModeSelected = false;
            this.Close();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }
    }
}