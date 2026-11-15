using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices; // Added

namespace AotForms
{
    public partial class ShortcutForm : Form
    {
        private static ShortcutForm _instance;
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

        // P/Invoke for anti-recording
        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TOOLWINDOW = 0x80;

        private void ApplyCaptureExclusion()
        {
            IntPtr hWnd = this.Handle;
            if (hWnd == IntPtr.Zero)
                return;
            SetWindowDisplayAffinity(hWnd, WDA_EXCLUDEFROMCAPTURE);
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TOOLWINDOW);
        }

        public static ShortcutForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new ShortcutForm();
                return _instance;
            }
        }

        private ShortcutForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Load += ShortcutForm_Load;

            // Animation timer – cycles colors every 500 ms
            _animationTimer = new System.Windows.Forms.Timer();
            _animationTimer.Interval = 500;
            _animationTimer.Tick += (s, e) =>
            {
                if (richTextBox2 != null && !richTextBox2.IsDisposed)
                {
                    richTextBox2.ForeColor = _colors[_colorIndex];
                    _colorIndex = (_colorIndex + 1) % _colors.Length;
                }
            };

            // Apply anti-recording after handle is created
            ApplyCaptureExclusion();
        }

        private void ShortcutForm_Load(object sender, EventArgs e)
        {
            // Highlight "F11" in red and bold
            int startIndex = rtbShortcuts.Text.IndexOf("F11");
            if (startIndex >= 0)
            {
                rtbShortcuts.Select(startIndex, 3);
                rtbShortcuts.SelectionColor = Color.Red;
                rtbShortcuts.SelectionFont = new Font(rtbShortcuts.Font, FontStyle.Bold);
                rtbShortcuts.Select(0, 0);
            }

            // Start animation when form is shown
            _animationTimer.Start();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _animationTimer.Stop();
            this.Hide();
        }

        public static void Toggle()
        {
            if (Instance.Visible)
            {
                Instance._animationTimer.Stop();
                Instance.Hide();
            }
            else
            {
                Instance.Show();
                Instance._animationTimer.Start();
                Instance.ApplyCaptureExclusion(); // Re-apply on show
            }
        }

        public void ShowAutoClose(int milliseconds)
        {
            this.Show();
            _animationTimer.Start();
            ApplyCaptureExclusion(); // Apply on show

            System.Windows.Forms.Timer autoCloseTimer = new System.Windows.Forms.Timer();
            autoCloseTimer.Interval = milliseconds;
            autoCloseTimer.Tick += (s, ev) =>
            {
                autoCloseTimer.Stop();
                _animationTimer.Stop();
                this.Hide();
            };
            autoCloseTimer.Start();
        }

        // Keep the designer‑generated Paint stub
        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }

        private void rtbShortcuts_TextChanged(object sender, EventArgs e)
        {

        }
    }
}