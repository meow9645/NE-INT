namespace AotForms
{
    partial class ShortcutForm
    {
        private System.ComponentModel.IContainer components = null;

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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            richTextBox3 = new RichTextBox();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnClose = new Guna.UI2.WinForms.Guna2Button();
            richTextBox2 = new RichTextBox();
            richTextBox1 = new RichTextBox();
            rtbShortcuts = new RichTextBox();
            guna2Panel1.SuspendLayout();
            SuspendLayout();
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackColor = Color.FromArgb(25, 25, 30);
            guna2Panel1.Controls.Add(richTextBox3);
            guna2Panel1.Controls.Add(lblTitle);
            guna2Panel1.Controls.Add(btnClose);
            guna2Panel1.Controls.Add(richTextBox2);
            guna2Panel1.Controls.Add(richTextBox1);
            guna2Panel1.Controls.Add(rtbShortcuts);
            guna2Panel1.CustomizableEdges = customizableEdges3;
            guna2Panel1.Dock = DockStyle.Fill;
            guna2Panel1.Location = new Point(0, 0);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2Panel1.ShadowDecoration.Depth = 20;
            guna2Panel1.ShadowDecoration.Enabled = true;
            guna2Panel1.Size = new Size(420, 320);
            guna2Panel1.TabIndex = 0;
            guna2Panel1.Paint += guna2Panel1_Paint;
            // 
            // richTextBox3
            // 
            richTextBox3.BackColor = Color.FromArgb(25, 25, 30);
            richTextBox3.BorderStyle = BorderStyle.None;
            richTextBox3.Font = new Font("Consolas", 11.6F, FontStyle.Regular, GraphicsUnit.Point);
            richTextBox3.ForeColor = Color.Red;
            richTextBox3.Location = new Point(182, 150);
            richTextBox3.Name = "richTextBox3";
            richTextBox3.ReadOnly = true;
            richTextBox3.ScrollBars = RichTextBoxScrollBars.None;
            richTextBox3.Size = new Size(226, 40);
            richTextBox3.TabIndex = 5;
            richTextBox3.Text = "BEFORE USING ESP (STREAMER) \nPRESS F8 FIRST THEN F6 ...";
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.CornflowerBlue;
            lblTitle.Location = new Point(116, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(160, 34);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Shortcut Keys";
            // 
            // btnClose
            // 
            btnClose.Animated = true;
            btnClose.BorderRadius = 8;
            btnClose.CustomizableEdges = customizableEdges1;
            btnClose.DisabledState.BorderColor = Color.DarkGray;
            btnClose.DisabledState.CustomBorderColor = Color.DarkGray;
            btnClose.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnClose.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnClose.FillColor = Color.FromArgb(60, 60, 80);
            btnClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnClose.ForeColor = Color.White;
            btnClose.HoverState.FillColor = Color.FromArgb(80, 80, 100);
            btnClose.Location = new Point(314, 268);
            btnClose.Name = "btnClose";
            btnClose.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnClose.Size = new Size(80, 30);
            btnClose.TabIndex = 2;
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;
            // 
            // richTextBox2
            // 
            richTextBox2.BackColor = Color.FromArgb(25, 25, 30);
            richTextBox2.BorderStyle = BorderStyle.None;
            richTextBox2.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point);
            richTextBox2.ForeColor = Color.Red;
            richTextBox2.Location = new Point(12, 268);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.ReadOnly = true;
            richTextBox2.ScrollBars = RichTextBoxScrollBars.None;
            richTextBox2.Size = new Size(224, 21);
            richTextBox2.TabIndex = 4;
            richTextBox2.Text = "F11 – Show SHORTCUT KEYS";
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.FromArgb(25, 25, 30);
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point);
            richTextBox1.ForeColor = Color.Red;
            richTextBox1.Location = new Point(194, 220);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.None;
            richTextBox1.Size = new Size(189, 44);
            richTextBox1.TabIndex = 3;
            richTextBox1.Text = "SWITCH BACK TO GUI / STREAMER MODE\n";
            // 
            // rtbShortcuts
            // 
            rtbShortcuts.BackColor = Color.FromArgb(25, 25, 30);
            rtbShortcuts.BorderStyle = BorderStyle.None;
            rtbShortcuts.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point);
            rtbShortcuts.ForeColor = Color.FromArgb(220, 220, 220);
            rtbShortcuts.Location = new Point(12, 70);
            rtbShortcuts.Name = "rtbShortcuts";
            rtbShortcuts.ReadOnly = true;
            rtbShortcuts.ScrollBars = RichTextBoxScrollBars.None;
            rtbShortcuts.Size = new Size(224, 194);
            rtbShortcuts.TabIndex = 1;
            rtbShortcuts.Text = "F3  – Enable Aim-Visible\nF4  – Disable AimBot\nF5  – Toogle No Recoil\n-----------------------\nF6 – Toggle ESP\nF7  – Hide ESP\nF8  – Show ESP \n------------------------\nF9  – Hide Panel\nF10 – Show Panel\n\n";
            rtbShortcuts.TextChanged += rtbShortcuts_TextChanged;
            // 
            // ShortcutForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 25, 30);
            ClientSize = new Size(420, 320);
            Controls.Add(guna2Panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ShortcutForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Shortcuts";
            TopMost = true;
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private System.Windows.Forms.RichTextBox rtbShortcuts;
        private Guna.UI2.WinForms.Guna2Button btnClose;
        private RichTextBox richTextBox1;
        private RichTextBox richTextBox2;
        private RichTextBox richTextBox3;
    }
}