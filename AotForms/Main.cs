using Broken3ifrit;
using Guna.UI2.WinForms;
using MagıcMemory;
using MAZEX_S_R_MEMM;
using NARZOPAPA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;

namespace AotForms
{
    public partial class Main : Form
    {
        IntPtr mainHandle;
        IntPtr hWnd;
        bool keybind1 = false;
        bool shift;
        bool aimbothks;
        bool esphks;
        bool confighks;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private Keys HideKey;
        private bool waitingForHidehackKey = false;
        private bool waitingForAimbothackKey = false;
        private bool formularioVisible = true;
        private const int WM_HOTKEY = 0x0312;
        private const int MYACTION_HOTKEY_ID = 1;
        private const int Hide_HOTKEY_ID = 2;

        // ----- Global Hotkey IDs -----
        private const int HOTKEY_ID_F3 = 104;
        private const int HOTKEY_ID_F4 = 105;
        private const int HOTKEY_ID_F5 = 106;
        private const int HOTKEY_ID_F6 = 107;
        private const int HOTKEY_ID_F7 = 108;
        private const int HOTKEY_ID_F8 = 109;
        private const int HOTKEY_ID_F9 = 100;
        private const int HOTKEY_ID_F10 = 101;
        private const int HOTKEY_ID_F11 = 110;

        // ----- Dragging fields -----
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        // ----- Streamer Mode -----
        public bool IsStreamerMode { get; private set; } = false;

        // ----- Animation -----
        private System.Windows.Forms.Timer _animationTimer;
        private float _hue = 0f;
        private float _brightness = 1f;
        private Label labelShadow;

        // ----- P/Invoke -----
        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // ----- ComboBox dropdown P/Invoke (to exclude its popup from capture) -----
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COMBOBOXINFO
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public int stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;
            public IntPtr hwndList;
        }

        [DllImport("user32.dll")]
        private static extern bool GetComboBoxInfo(IntPtr hWndComboBox, ref COMBOBOXINFO pcbi);

        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TOOLWINDOW = 0x80;

        // -------------------------------------------------
        public Main()
        {
            InitializeComponent();

            // ★ Make form always on top ★
            this.TopMost = true;

            CreateShadowLabel();
            InitAnimation();
            ApplyCaptureExclusion();
            HookComboBoxDropdowns(this);
        }

        public Main(IntPtr handle)
        {
            mainHandle = handle;
            NARZO NARZO = new NARZO();
            InitializeComponent();

            // ★ Make form always on top ★
            this.TopMost = true;

            CreateShadowLabel();
            InitAnimation();

            this.DoubleBuffered = true;

            this.KeyPreview = true;
            this.KeyDown += Kh_KeyDown;
            this.KeyUp += Kh_KeyUp;

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

            ShowModeSelection();
            RegisterGlobalHotkeys();

            ApplyCaptureExclusion();
            HookComboBoxDropdowns(this);
        }

        private void ApplyCaptureExclusion()
        {
            ApplyCaptureExclusion(_captureExcluded);
        }

        private void ApplyCaptureExclusion(bool exclude)
        {
            IntPtr hWnd = this.Handle;
            if (hWnd == IntPtr.Zero)
                return;

            SetWindowDisplayAffinity(hWnd, exclude ? WDA_EXCLUDEFROMCAPTURE : WDA_NONE);

            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TOOLWINDOW);

            // Keep every other open form (ESP, Form1, etc.) in sync too
            foreach (Form f in Application.OpenForms)
            {
                if (f == this || f.Handle == IntPtr.Zero) continue;
                SetWindowDisplayAffinity(f.Handle, exclude ? WDA_EXCLUDEFROMCAPTURE : WDA_NONE);
            }
        }

        // ----- ComboBox dropdown capture exclusion helpers -----
        private void ApplyComboPopupAffinity(IntPtr hwndList)
        {
            if (hwndList == IntPtr.Zero) return;

            // ★ Same layered + toolwindow styles as the main form → makes it truly invisible ★
            int exStyle = GetWindowLong(hwndList, GWL_EXSTYLE);
            SetWindowLong(hwndList, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TOOLWINDOW);

            SetWindowDisplayAffinity(
                hwndList,
                _captureExcluded ? WDA_EXCLUDEFROMCAPTURE : WDA_NONE);
        }

        private void HookComboBoxDropdowns(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is ComboBox combo)
                {
                    // Pre-style the popup HWND now (before it ever shows) to avoid a first-open black box
                    var info0 = new COMBOBOXINFO();
                    info0.cbSize = Marshal.SizeOf(info0);
                    if (GetComboBoxInfo(combo.Handle, ref info0) && info0.hwndList != IntPtr.Zero)
                        ApplyComboPopupAffinity(info0.hwndList);

                    combo.DropDown += (s, e) =>
                    {
                        var info = new COMBOBOXINFO();
                        info.cbSize = Marshal.SizeOf(info);
                        if (GetComboBoxInfo(combo.Handle, ref info))
                            ApplyComboPopupAffinity(info.hwndList);
                    };

                    combo.MouseDown += (s, e) =>
                    {
                        var info = new COMBOBOXINFO();
                        info.cbSize = Marshal.SizeOf(info);
                        if (GetComboBoxInfo(combo.Handle, ref info))
                            ApplyComboPopupAffinity(info.hwndList);
                    };
                }

                if (c.HasChildren)
                    HookComboBoxDropdowns(c);
            }
        }

        private void RefreshComboAffinity(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is ComboBox combo)
                {
                    var info = new COMBOBOXINFO();
                    info.cbSize = Marshal.SizeOf(info);
                    if (GetComboBoxInfo(combo.Handle, ref info))
                        ApplyComboPopupAffinity(info.hwndList);
                }
                if (c.HasChildren) RefreshComboAffinity(c);
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

        private void headerTitleLabel_Click(object sender, EventArgs e) { }

        private void CreateShadowLabel()
        {
            if (label1 == null) return;

            labelShadow = new Label
            {
                Text = label1.Text,
                Font = label1.Font,
                ForeColor = Color.FromArgb(80, 0, 0, 0),
                BackColor = Color.Transparent,
                AutoSize = label1.AutoSize,
                Location = new Point(label1.Location.X + 2, label1.Location.Y + 2),
                TextAlign = label1.TextAlign,
                Anchor = label1.Anchor,
                TabIndex = 0
            };

            this.Controls.Add(labelShadow);
            this.Controls.SetChildIndex(labelShadow, 0);
            this.Controls.SetChildIndex(label1, 1);
        }

        private void InitAnimation()
        {
            _animationTimer = new System.Windows.Forms.Timer();
            _animationTimer.Interval = 30;
            _animationTimer.Tick += (s, e) =>
            {
                if (label1 == null || label1.IsDisposed) return;

                _hue += 0.5f;
                if (_hue >= 360f) _hue -= 360f;

                float pulse = (float)Math.Sin(Environment.TickCount / 1000.0 * 2.0);
                _brightness = 0.6f + 0.4f * (pulse + 1f) / 2f;

                Color mainColor = HslToRgb(_hue / 360f, 1f, _brightness);
                label1.ForeColor = mainColor;

                if (labelShadow != null && !labelShadow.IsDisposed)
                {
                    Color shadowColor = HslToRgb(_hue / 360f, 1f, _brightness * 0.3f);
                    labelShadow.ForeColor = Color.FromArgb(120, shadowColor);
                }
            };
            this.Load += (s, e) => _animationTimer.Start();
        }

        private Color HslToRgb(float h, float s, float l)
        {
            float r, g, b;
            if (s == 0f) { r = g = b = l; }
            else
            {
                float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
                float p = 2f * l - q;
                r = HueToRgb(p, q, h + 1f / 3f);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1f / 3f);
            }
            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        private float HueToRgb(float p, float q, float t)
        {
            if (t < 0f) t += 1f;
            if (t > 1f) t -= 1f;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }

        private void ShowModeSelection()
        {
            using (var selectionForm = new ModeSelectionForm())
            {
                selectionForm.ShowDialog();
                IsStreamerMode = selectionForm.StreamerModeSelected;
            }

            if (IsStreamerMode)
                EnterStreamerMode();
            else
                EnterGUIMode();
        }

        private void EnterStreamerMode()
        {
            Config.StreamerMode = true;

            foreach (Form form in Application.OpenForms)
            {
                if (form is Form1)
                {
                    form.Hide();
                    break;
                }
            }

            this.Visible = false;

            if (ESP.Instance != null)
                ESP.Instance.SetVisible(false);

            ShortcutForm.Instance.ShowAutoClose(500000);
        }

        private void EnterGUIMode()
        {
            Config.StreamerMode = false;
            IsStreamerMode = false;

            foreach (Form form in Application.OpenForms)
            {
                if (form is Form1)
                {
                    form.Show();
                    form.BringToFront();
                    break;
                }
            }

            this.Show();
            SetStreamMode();
            this.BringToFront();
            Notification.Show("GUI Mode active", Color.Green);

            if (ESP.Instance != null)
                ESP.Instance.SetVisible(true);

            ApplyCaptureExclusion();
        }

        private void RegisterGlobalHotkeys()
        {
            RegisterHotKey(this.Handle, HOTKEY_ID_F3, 0, (uint)Keys.F3);
            RegisterHotKey(this.Handle, HOTKEY_ID_F4, 0, (uint)Keys.F4);
            RegisterHotKey(this.Handle, HOTKEY_ID_F5, 0, (uint)Keys.F5);
            RegisterHotKey(this.Handle, HOTKEY_ID_F6, 0, (uint)Keys.F6);
            RegisterHotKey(this.Handle, HOTKEY_ID_F7, 0, (uint)Keys.F7);
            RegisterHotKey(this.Handle, HOTKEY_ID_F8, 0, (uint)Keys.F8);
            RegisterHotKey(this.Handle, HOTKEY_ID_F9, 0, (uint)Keys.F9);
            RegisterHotKey(this.Handle, HOTKEY_ID_F10, 0, (uint)Keys.F10);
            RegisterHotKey(this.Handle, HOTKEY_ID_F11, 0, (uint)Keys.F11);
        }

        private void UnregisterGlobalHotkeys()
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_F3);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F4);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F5);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F6);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F7);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F8);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F9);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F10);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F11);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HOTKEY_ID_F3)
                {
                    Config.enableAimBot = true;
                    enableaimcheckbox.Checked = true;
                    guna2ComboBox1.SelectedIndex = 0; // HERE 
                }
                else if (id == HOTKEY_ID_F4)
                {
                    Config.enableAimBot = false;
                    enableaimcheckbox.Checked = false;
                    guna2ComboBox1.SelectedIndex = 3;
                }
                else if (id == HOTKEY_ID_F5)
                {
                    Config.NoRecoil = !Config.NoRecoil;
                    norecoilcheckbox.Checked = Config.NoRecoil;
                }
                else if (id == HOTKEY_ID_F6)
                {
                    Config.ESPLine = !Config.ESPLine;
                    esplinecheckbox.Checked = Config.ESPLine;
                }
                else if (id == HOTKEY_ID_F7)
                {
                    if (ESP.Instance != null)
                        ESP.Instance.SetVisible(false);
                }
                else if (id == HOTKEY_ID_F8)
                {
                    if (ESP.Instance != null)
                        ESP.Instance.SetVisible(true);
                }
                else if (id == HOTKEY_ID_F9)
                {
                    this.Hide();
                }
                else if (id == HOTKEY_ID_F10)
                {
                    this.Show();
                    SetStreamMode();
                    this.BringToFront();
                    ApplyCaptureExclusion();
                }
                else if (id == HOTKEY_ID_F11)
                {
                    ShortcutForm.Toggle();
                }
            }
            base.WndProc(ref m);
        }

        private void Kh_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey) shift = false;
        }

        private async void Kh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey) shift = true;

            if (e.KeyCode == Keys.F1)
            {
                if (Config.FixEsp)
                {
                    Core.Entities = new();
                    InternalMemory.Cache = new();
                }
            }

            if (e.KeyCode == Keys.Insert)
            {
                Config.Notif();
                if (keybind1 == false)
                {
                    this.Hide();
                    keybind1 = true;
                }
                else
                {
                    this.Show();
                    SetStreamMode();
                    keybind1 = false;
                    ApplyCaptureExclusion();
                }
            }
        }

        private void SetStreamMode()
        {
            foreach (var obj in Application.OpenForms)
            {
                var form = obj as Form;
                if (Config.StreamerMode)
                {
                    SetWindowDisplayAffinity(form.Handle, WDA_NONE);
                    SetWindowDisplayAffinity(form.Handle, WDA_EXCLUDEFROMCAPTURE);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _animationTimer?.Stop();
            UnregisterGlobalHotkeys();
            base.OnFormClosing(e);
        }

        // ============================================================
        //  EVENT HANDLERS
        // ============================================================

        private void enableaimcheckbox_Click(object sender, EventArgs e)
        {
            Config.enableAimBot = enableaimcheckbox.Checked;
            status.Text = "Enabled";
            Notification.Show("FUNCTION: ON", Color.Yellow);
        }

        private void upplayercheckbox_Click(object sender, EventArgs e) { }
        private void telekillcheckbox_Click(object sender, EventArgs e) { }

        private void norecoilcheckbox_Click(object sender, EventArgs e)
        {
            Config.NoRecoil = norecoilcheckbox.Checked;
            status.Text = "NoRecoil Enabled";
            Notification.Show("SPINBOT: ON", Color.Yellow);
        }

        private void ignoreknockedcheckbox_Click(object sender, EventArgs e)
        {
            Config.IgnoreKnocked = ignoreknockedcheckbox.Checked;
            status.Text = "IgnoreKnocked Enabled";
            Notification.Show("IGNOREKNOCK: ON", Color.Yellow);
        }

        private void showcirclefovcheckbox_Click(object sender, EventArgs e)
        {
            Config.FOVEnabled = showcirclefovcheckbox.Checked;
            Notification.Show("SHOWFAV: ON", Color.Yellow);
        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            var distance = guna2TrackBar1.Value;
            label7.Text = $"AimFov: {distance}";
            Config.AimFov = distance;
        }

        static IntPtr FindRenderWindow(IntPtr parent)
        {
            IntPtr renderWindow = IntPtr.Zero;
            WinAPI.EnumChildWindows(parent, (hWnd, lParam) =>
            {
                StringBuilder sb = new StringBuilder(256);
                WinAPI.GetWindowText(hWnd, sb, sb.Capacity);
                string windowName = sb.ToString();
                if (!string.IsNullOrEmpty(windowName))
                {
                    if (windowName != "HD-Player")
                    {
                        renderWindow = hWnd;
                    }
                }
                return true;
            }, IntPtr.Zero);
            return renderWindow;
        }

        // ------------------------------------------------------------
        //  enablehookbtn_Click – now uses zero‑trace Adb
        // ------------------------------------------------------------
        private async void enablehookbtn_Click(object sender, EventArgs e)
        {
            var processes = Process.GetProcessesByName("HD-Player");
            if (processes.Length != 1)
            {
                MessageBox.Show("Open emulator.");
                return;
            }
            var process = processes[0];
            string mainModulePath = Path.GetDirectoryName(process.MainModule?.FileName);
            if (string.IsNullOrEmpty(mainModulePath))
            {
                MessageBox.Show("Reinstall emulator.");
                return;
            }
            var adbPath = Path.Combine(mainModulePath, "HD-Adb.exe");
            if (!File.Exists(adbPath))
            {
                MessageBox.Show("Adb not Found. Reinstall emulator.");
                return;
            }

            var adb = new Adb(adbPath);
            await adb.Kill();
            if (!await adb.Start())
            {
                MessageBox.Show("Adb Error");
                Environment.Exit(0);
                return;
            }
            var moduleAddr = await adb.FindModule("com.dts.freefiremax", "libil2cpp.so");
            if (moduleAddr == 0)
            {
                MessageBox.Show("Go to Lobby then Logout then try to apply.");
                Environment.Exit(0);
                return;
            }
            Offsets.Il2Cpp = moduleAddr;
            Core.Handle = FindRenderWindow(mainHandle);

            adb.CleanTempFolder(); // remove temp traces

            Notification.Show("Hooks applied successfully!", Color.Green);
        }

        // -------------------- All other original event handlers (unchanged) --------------------
        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedItem = (string)comboBox.SelectedItem;
            switch (selectedItem)
            {


                case "AIM-VISIBLE":
                    Config.AimBotRage = false;
                    Config.SILENT = false;
                    Config.AimbotVisible = true;
                    Config.SilentAim = false;
                    break;
                case "AIMBOT-BRUTUAL":
                    Config.AimBotRage = true;
                    Config.SILENT = false;
                    Config.AimbotVisible = false;
                    Config.SilentAim = false;
                    break;
                case "SILENT-AIM(RISK)":
                    Config.AimBotRage = true;
                    Config.SILENT = true;
                    Config.AimbotVisible = false;
                    Config.SilentAim = true;
                    break;

                case "OFF":
                    Config.AimBotRage = false;
                    Config.SILENT = false;
                    Config.AimbotVisible = false;
                    Config.SilentAim = false;
                    break;


            }
        }

        private void esplinecheckbox_Click(object sender, EventArgs e)
        {
            Config.ESPLine = esplinecheckbox.Checked;
            Notification.Show("ESP LINE: ON", Color.Yellow);
        }

        private void espboxcheckbox_Click(object sender, EventArgs e)
        {
            Config.ESPBox = espboxcheckbox.Checked;
        }

        private void espcornercheckbox_Click(object sender, EventArgs e)
        {
            Config.ESPBox2 = espcornercheckbox.Checked;
        }

        private void esphealthcheckbox_Click(object sender, EventArgs e)
        {
            Config.ESPHealth = esphealthcheckbox.Checked;
        }

        private bool isAutoRefreshEnabled = false;
        private CancellationTokenSource cancellationTokenSource;

        private void guna2CustomCheckBox1_Click(object sender, EventArgs e)
        {
            isAutoRefreshEnabled = !isAutoRefreshEnabled;
            if (isAutoRefreshEnabled)
                StartAutoRefresh();
            else
                StopAutoRefresh();
        }

        private void StartAutoRefresh()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Core.Entities = new();
                    InternalMemory.Cache = new();
                    Thread.Sleep(4000);
                }
            }, token);
        }

        private void StopAutoRefresh()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        private void espnamecheckbox_Click(object sender, EventArgs e)
        {
            Config.ESPName = espnamecheckbox.Checked;
        }

        private void guna2ComboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedItem = (string)comboBox.SelectedItem;
            switch (selectedItem)
            {
                case "ClosestToCrosshair":
                    Config.TargetingMode = TargetingMode.ClosestToCrosshair;
                    break;
                case "Target360":
                    Config.TargetingMode1 = TargetingMode.Target360;
                    break;
                case "ClosestToPlayer":
                    Config.TargetingMode2 = TargetingMode.ClosestToPlayer;
                    break;
                case "LowestHealth":
                    Config.TargetingMode3 = TargetingMode.LowestHealth;
                    break;
            }
        }

        private void espdistancecheckbox_Click(object sender, EventArgs e)
        {
            Config.ESPDistance = espdistancecheckbox.Checked;
        }

        private void espfillboxcheckbox_Click(object sender, EventArgs e)
        {
            Config.ESPFillBox = espfillboxcheckbox.Checked;
        }

        private void NoCache()
        {
            InternalMemory.Cache = new();
            Core.Entities = new();
            Thread.Sleep(1000);
        }

        private void UpdateEntities()
        {
            foreach (var entity in Core.Entities.Values)
            {
                if (entity.IsTeam != Bool3.False) continue;
                TreeNode entityNode = new TreeNode(entity.Name);
                entityNode.Nodes.Add(new TreeNode($"IsKnown: {entity.IsKnown}"));
                entityNode.Nodes.Add(new TreeNode($"IsTeam: {entity.IsTeam}"));
                entityNode.Nodes.Add(new TreeNode($"Head: {entity.Head}"));
                entityNode.Nodes.Add(new TreeNode($"Root: {entity.Root}"));
                entityNode.Nodes.Add(new TreeNode($"Health: {entity.Health}"));
                entityNode.Nodes.Add(new TreeNode($"IsDead: {entity.IsDead}"));
                entityNode.Nodes.Add(new TreeNode($"IsKnocked: {entity.IsKnocked}"));
            }
            Thread.Sleep(1000);
        }

        private void updateentitybtn_Click(object sender, EventArgs e)
        {
            UpdateEntities();
        }

        bool resetCacheChecked = false;
        System.Timers.Timer timer = new System.Timers.Timer(10000);

        void SetupTimer()
        {
            timer.Elapsed += (sender, ev) => { resetCacheChecked = true; };
            timer.AutoReset = true;
            timer.Start();
        }

        private void resetcachebtn_Click(object sender, EventArgs e)
        {
            NoCache();
        }

        static void ResetCache()
        {
            Core.Entities = new();
            InternalMemory.Cache = new();
        }

        private void guna2CustomCheckBox3_Click(object sender, EventArgs e)
        {
            if (resetCacheChecked)
            {
                Core.HaveMatrix = true;
                SetupTimer();
                ResetCache();
                resetCacheChecked = false;
            }
        }

        private void anticheatbtn_Click(object sender, EventArgs e) { }

        private void linecolorpicker_Click(object sender, EventArgs e)
        {
            var picker = new ColorDialog();
            var result = picker.ShowDialog();
            if (result == DialogResult.OK)
            {
                linecolorpicker.FillColor = picker.Color;
                Config.ESPLineColor = picker.Color;
            }
        }

        private void boxcolorpicker_Click(object sender, EventArgs e)
        {
            var picker = new ColorDialog();
            var result = picker.ShowDialog();
            if (result == DialogResult.OK)
            {
                boxcolorpicker.FillColor = picker.Color;
                Config.ESPBoxColor = picker.Color;
            }
        }

        private void cornerboxcolorpicker_Click(object sender, EventArgs e)
        {
            var picker = new ColorDialog();
            var result = picker.ShowDialog();
            if (result == DialogResult.OK)
            {
                cornerboxcolorpicker.FillColor = picker.Color;
                Config.ESPBoxColor = picker.Color;
            }
        }

        private void fillboxcolorpicker_Click(object sender, EventArgs e)
        {
            var picker = new ColorDialog();
            var result = picker.ShowDialog();
            if (result == DialogResult.OK)
            {
                fillboxcolorpicker.FillColor = picker.Color;
                Config.ESPFillBoxColor = picker.Color;
            }
        }

        private void namecolorpicker_Click(object sender, EventArgs e)
        {
            var picker = new ColorDialog();
            var result = picker.ShowDialog();
            if (result == DialogResult.OK)
            {
                namecolorpicker.FillColor = picker.Color;
                Config.ESPNameColor = picker.Color;
            }
        }

        private void distancecolorpicker_Click(object sender, EventArgs e)
        {
            var picker = new ColorDialog();
            var result = picker.ShowDialog();
            if (result == DialogResult.OK)
            {
                distancecolorpicker.FillColor = picker.Color;
                Config.ESPDistanceColor = picker.Color;
            }
        }

        private void guna2CustomCheckBox2_Click(object sender, EventArgs e)
        {
            Config.ESPSkeleton = guna2CustomCheckBox2.Checked;
        }

        private void healthcolorpicker_Click(object sender, EventArgs e)
        {
            var picker = new ColorDialog();
            var result = picker.ShowDialog();
            if (result == DialogResult.OK)
            {
                healthcolorpicker.FillColor = picker.Color;
                Config.ESPHealthColor = picker.Color;
            }
        }

        private void skeletoncolorpicker_Click(object sender, EventArgs e)
        {
            var picker = new ColorDialog();
            var result = picker.ShowDialog();
            if (result == DialogResult.OK)
            {
                skeletoncolorpicker.FillColor = picker.Color;
                Config.ESPSkeletonColor = picker.Color;
            }
        }

        private void targetaimcolorpicker_Click(object sender, EventArgs e) { }

        private List<long> a = new List<long>();
        private Dictionary<long, string> m = new Dictionary<long, string>();

        private async void guna2CustomCheckBox4_Click(object sender, EventArgs e) { }
        private void guna2CustomCheckBox5_Click(object sender, EventArgs e) { }
        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }

        private async void Main_Load(object sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                status.Text = "Design mode - no actions";
                return;
            }

            // ★ Ensure TopMost is true when the form loads ★
            this.TopMost = true;
            // Also sync the checkbox state:
            guna2CustomCheckBox10.Checked = true;

            // ★ Stream-proof ON by default ★
            guna2CustomCheckBox4.Checked = true;
            _captureExcluded = true;

            ApplyCaptureExclusion();
            HookComboBoxDropdowns(this);
        }

        private void guna2TabControl1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void guna2Button1_Click(object sender, EventArgs e) { }
        private async void guna2CustomCheckBox6_Click(object sender, EventArgs e) { }
        private async void guna2CustomCheckBox9_Click(object sender, EventArgs e) { }
        private async void guna2CustomCheckBox8_Click(object sender, EventArgs e) { }
        private async void guna2Button1_Click_1(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void guna2CustomCheckBox6_Click_1(object sender, EventArgs e) { }
        private async void guna2CustomCheckBox8_Click_1(object sender, EventArgs e) { }
        private void label25_Click(object sender, EventArgs e) { }
        private void guna2CustomCheckBox7_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label27_Click(object sender, EventArgs e) { }
        private void guna2CustomCheckBox9_Click_1(object sender, EventArgs e) { }
        private void tabPage4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }

        private void guna2TrackBar2_Scroll(object sender, ScrollEventArgs e)
        {
            var distance = guna2TrackBar2.Value;
            label5.Text = $"Smoothness: {distance}";
            Config.AimBotSmooth = distance;
        }

        private void guna2CustomCheckBox4_Click_1(object sender, EventArgs e) { }

        const uint WDA_NONE = 0x00000000;
        const uint WDA_MONITOR = 0x00000001;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private void guna2CustomCheckBox5_Click_1(object sender, EventArgs e)
        {
            Config.sound = guna2CustomCheckBox2.Checked;
        }

        private void guna2ControlBox2_Click(object sender, EventArgs e)
        {
            Config.Notif();
            try
            {
                var processes = Process.GetProcessesByName("HD-Player");
                foreach (var process in processes)
                {
                    process.Kill();
                    process.WaitForExit();
                }
            }
            catch (Exception ex) { }
        }

        private void guna2TrackBar3_Scroll_1(object sender, ScrollEventArgs e)
        {
            Config.SpinSpeed = (float)spinSpeedTrackBar.Value;
            status.Text = "Speed: " + spinSpeedTrackBar.Value.ToString();
        }

        private void status_Click(object sender, EventArgs e) { }

        private void guna2CustomCheckBox8_Click_3(object sender, EventArgs e)
        {
            Config.spinbot = guna2CustomCheckBox8.Checked;
            if (Config.spinbot)
            {
                RapidSpin.Activate();
                Notification.Show("SPINBOT: ON", Color.Yellow);
            }
            else
            {
                RapidSpin.Deactivate();
                Notification.Show("SPINBOT: OFF", Color.Yellow);
            }
        }

        private void label22_Click(object sender, EventArgs e) { }

        private void guna2CustomCheckBox10_Click(object sender, EventArgs e)
        {
            this.TopMost = guna2CustomCheckBox10.Checked;
            if (this.TopMost)
                Notification.Show("PINNED TO TOP", Color.Yellow);
            else
                Notification.Show("UNPINNED", Color.Gray);
        }

        private void guna2TabControl1_SelectedIndexChanged_1(object sender, EventArgs e) { }
        private void shayanHeader1_Load(object sender, EventArgs e) { }

        private void tabPage3_Click(object sender, EventArgs e) { }
        private void tabPage1_Click(object sender, EventArgs e) { }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedItem = (string)comboBox.SelectedItem;
            switch (selectedItem)
            {


                case "VISIBLE":
                    Config.AimBotRage = false;
                    Config.SILENT = false;
                    Config.AimbotVisible = true;
                    Config.SilentAim = false;
                    break;
                case "AIMBOT-BRUTUAL":
                    Config.AimBotRage = true;
                    Config.SILENT = false;
                    Config.AimbotVisible = false;
                    Config.SilentAim = false;
                    break;

                case "OFF":
                    Config.AimBotRage = false;
                    Config.SILENT = false;
                    Config.AimbotVisible = false;
                    Config.SilentAim = false;
                    break;


            }
        }

        // ---- Stream-proof / capture exclusion state ----
        private bool _captureExcluded = true;   // default = invisible on screenshare

        private void guna2CustomCheckBox4_Click_2(object sender, EventArgs e)
        {
            _captureExcluded = guna2CustomCheckBox4.Checked;
            ApplyCaptureExclusion(_captureExcluded);

            // ★ Refresh combobox dropdown popups so they follow the new state ★
            RefreshComboAffinity(this);

            Notification.Show(
                _captureExcluded ? "STREAM-PROOF: ON (hidden)" : "STREAM-PROOF: OFF (visible)",
                _captureExcluded ? Color.LimeGreen : Color.OrangeRed);
        }
    }
}