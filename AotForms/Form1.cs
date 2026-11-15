using Guna.UI2.WinForms;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AotForms
{
    public partial class Form1 : Form
    {
        IntPtr mainHandle;

        // ---- P/Invoke for capture exclusion ----
        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        private void ApplyCaptureExclusion()
        {
            IntPtr hWnd = this.Handle;
            if (hWnd == IntPtr.Zero)
                return;

            SetWindowDisplayAffinity(hWnd, WDA_EXCLUDEFROMCAPTURE);
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);
        }

        // ---- Drag fields ----
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        // Designer‑safe constructor
        public Form1()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(0);
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(17, 17, 22);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            AttachDragEvents();
        }

        // Runtime constructor (if used)
        public Form1(IntPtr handle)
        {
            InitializeComponent();
            mainHandle = handle;

            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(0);
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(17, 17, 22);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            AttachDragEvents();
            ApplyCaptureExclusion();
        }

        private void AttachDragEvents()
        {
            if (headerPanel != null)
            {
                headerPanel.MouseDown += HeaderPanel_MouseDown;
                headerPanel.MouseMove += HeaderPanel_MouseMove;
                headerPanel.MouseUp += HeaderPanel_MouseUp;
            }
            if (headerTitleLabel != null)
            {
                headerTitleLabel.MouseDown += HeaderPanel_MouseDown;
                headerTitleLabel.MouseMove += HeaderPanel_MouseMove;
                headerTitleLabel.MouseUp += HeaderPanel_MouseUp;
            }
        }

        private void HeaderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void HeaderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        private void HeaderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        // ------------------------------------------------------------
        //  Form1_Load – auto‑starts the cheat without any login
        // ------------------------------------------------------------
        private async void Form1_Load(object sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                statuslbl.Text = "Design mode - no actions";
                return;
            }

            ApplyCaptureExclusion();

            // ---- Emulator & ADB checks ----
            statuslbl.Text = "STATUS: Checking emulator...";

            var processes = Process.GetProcessesByName("HD-Player");
            if (processes.Length != 1)
            {
                CustomMessageBox.Show("Open emulator.");
                return;
            }
            var process = processes[0];
            string mainModulePath = Path.GetDirectoryName(process.MainModule?.FileName);
            if (string.IsNullOrEmpty(mainModulePath))
            {
                CustomMessageBox.Show("Reinstall emulator.");
                return;
            }
            var adbPath = Path.Combine(mainModulePath, "HD-Adb.exe");
            if (!File.Exists(adbPath))
            {
                CustomMessageBox.Show("ERROR... Reinstall emulator.");
                return;
            }

            statuslbl.Text = "STATUS: Initializing ...";

            var adb = new Adb(adbPath);
            await adb.Kill();
            if (!await adb.Start())
            {
                CustomMessageBox.Show("Adb Error");
                Environment.Exit(0);
                return;
            }
            var moduleAddr = await adb.FindModule("com.dts.freefiremax", "libil2cpp.so");
            if (moduleAddr == 0)
            {
                CustomMessageBox.Show("Go to Lobby then Logout then try to apply.");
                Environment.Exit(0);
                return;
            }
            Offsets.Il2Cpp = moduleAddr;
            Core.Handle = FindRenderWindow(mainHandle);

            adb.CleanTempFolder(); // clean temp traces

            statuslbl.Text = "STATUS: Starting cheat...";

            // ============================================================
            // 🚀 START THE CHEAT DIRECTLY (no login required)
            // ============================================================
            try
            {
                // 1) Start ESP overlay
                var esp = new ESP();
                await esp.Start();

                // 2) Disable imgui.ini
                unsafe
                {
                    ImGui.GetIO().NativePtr->IniFilename = null;
                }

                // 3) Start threads
                new Thread(Data.Work) { IsBackground = true }.Start();
                new Thread(AimbotV2.Work) { IsBackground = true }.Start();
                new Thread(KXCAimbotVisible.Work) { IsBackground = true }.Start();
                new Thread(SilentC.Work) { IsBackground = true }.Start();

                // 4) Show main menu
                Main ML = new Main(mainHandle);
                if (!ML.IsStreamerMode)
                    ML.Show();

                // 5) Hide this login form
                this.Hide();

                Notification.Show("WELCOME", Color.Yellow);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Startup error: {ex.Message}");
            }
        }

        // ---- Helpers ----
        private string ExtractPID(string psOutput, string processName) => null;
        private uint ExtractBaseAddress(string mapsOutput) => 0;

        static IntPtr FindRenderWindow(IntPtr parent)
        {
            IntPtr renderWindow = IntPtr.Zero;
            WinAPI.EnumChildWindows(parent, (hWnd, lParam) =>
            {
                StringBuilder sb = new StringBuilder(256);
                WinAPI.GetWindowText(hWnd, sb, sb.Capacity);
                string windowName = sb.ToString();
                if (!string.IsNullOrEmpty(windowName) && windowName != "HD-Player")
                {
                    renderWindow = hWnd;
                }
                return true;
            }, IntPtr.Zero);
            return renderWindow;
        }

        // ---- Empty stubs for designer events ----
        private void guna2Button1_Click(object sender, EventArgs e) { }
        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }
        private void usernametxt_TextChanged(object sender, EventArgs e) { }
        private void passwordtxt_TextChanged(object sender, EventArgs e) { }
        private void guna2Button7_Click(object sender, EventArgs e) { }
        private void guna2Button6_Click(object sender, EventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void statuslbl_Click(object sender, EventArgs e) { }
        private void passwordTextBox_TextChanged(object sender, EventArgs e) { }
        private void guna2Panel3_Paint(object sender, PaintEventArgs e) { }
        private void headerTitleLabel_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void forgotLabel_Click(object sender, EventArgs e) { }
        private void usernameTextBox_TextChanged(object sender, EventArgs e) { }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }

    // ========== CUSTOM MESSAGEBOX ==========
    public static class CustomMessageBox
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        public static DialogResult Show(string text, string caption = "")
        {
            using (var form = new Form())
            {
                form.Text = caption;
                form.FormBorderStyle = FormBorderStyle.None;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.Size = new Size(360, 160);
                form.BackColor = Color.FromArgb(40, 40, 45);
                form.ForeColor = Color.White;
                form.TopMost = true;

                SetWindowDisplayAffinity(form.Handle, WDA_EXCLUDEFROMCAPTURE);
                int exStyle = GetWindowLong(form.Handle, GWL_EXSTYLE);
                SetWindowLong(form.Handle, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);

                Label label = new Label
                {
                    Text = text,
                    AutoSize = false,
                    Size = new Size(320, 60),
                    Location = new Point(20, 20),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10)
                };
                form.Controls.Add(label);

                Button button = new Button
                {
                    Text = "OK",
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(60, 60, 70),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9),
                    Size = new Size(80, 30),
                    Location = new Point((form.Width - 80) / 2, 100)
                };
                button.FlatAppearance.BorderSize = 0;
                button.Click += (s, ev) => { form.DialogResult = DialogResult.OK; form.Close(); };
                form.Controls.Add(button);

                return form.ShowDialog();
            }
        }
    }
}