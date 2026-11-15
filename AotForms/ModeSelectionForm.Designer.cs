namespace AotForms
{
    partial class ModeSelectionForm
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnStreamer = new Guna.UI2.WinForms.Guna2Button();
            btnGUI = new Guna.UI2.WinForms.Guna2Button();
            lblSubtitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2Panel1.SuspendLayout();
            SuspendLayout();
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackColor = Color.FromArgb(17, 17, 17);
            guna2Panel1.Controls.Add(guna2HtmlLabel1);
            guna2Panel1.Controls.Add(lblTitle);
            guna2Panel1.Controls.Add(lblSubtitle);
            guna2Panel1.Controls.Add(btnStreamer);
            guna2Panel1.Controls.Add(btnGUI);
            guna2Panel1.CustomizableEdges = customizableEdges5;
            guna2Panel1.Dock = DockStyle.Fill;
            guna2Panel1.Location = new Point(0, 0);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2Panel1.ShadowDecoration.Depth = 20;
            guna2Panel1.ShadowDecoration.Enabled = true;
            guna2Panel1.Size = new Size(460, 220);
            guna2Panel1.TabIndex = 0;
            guna2Panel1.Paint += guna2Panel1_Paint;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            guna2HtmlLabel1.ForeColor = Color.Red;
            guna2HtmlLabel1.Location = new Point(341, 200);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(116, 17);
            guna2HtmlLabel1.TabIndex = 4;
            guna2HtmlLabel1.Text = "Developed By B!LLA";
            guna2HtmlLabel1.Click += guna2HtmlLabel1_Click;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.Red;
            lblTitle.Location = new Point(30, 36);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(173, 34);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Panel Mode ->";
            // 
            // btnStreamer
            // 
            btnStreamer.Animated = true;
            btnStreamer.BorderRadius = 8;
            btnStreamer.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            btnStreamer.CustomizableEdges = customizableEdges1;
            btnStreamer.DisabledState.BorderColor = Color.DarkGray;
            btnStreamer.DisabledState.CustomBorderColor = Color.DarkGray;
            btnStreamer.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnStreamer.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnStreamer.FillColor = Color.FromArgb(40, 40, 50);
            btnStreamer.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            btnStreamer.ForeColor = Color.White;
            btnStreamer.HoverState.FillColor = Color.FromArgb(60, 60, 80);
            btnStreamer.HoverState.ForeColor = Color.White;
            btnStreamer.Location = new Point(30, 110);
            btnStreamer.Name = "btnStreamer";
            btnStreamer.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnStreamer.Size = new Size(180, 70);
            btnStreamer.TabIndex = 2;
            btnStreamer.Text = "🎥 Streamer Mode\r\n";
            btnStreamer.Click += btnStreamer_Click;
            // 
            // btnGUI
            // 
            btnGUI.Animated = true;
            btnGUI.BorderRadius = 8;
            btnGUI.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            btnGUI.CustomizableEdges = customizableEdges3;
            btnGUI.DisabledState.BorderColor = Color.DarkGray;
            btnGUI.DisabledState.CustomBorderColor = Color.DarkGray;
            btnGUI.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnGUI.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnGUI.FillColor = Color.FromArgb(40, 40, 50);
            btnGUI.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            btnGUI.ForeColor = Color.White;
            btnGUI.HoverState.FillColor = Color.FromArgb(60, 60, 80);
            btnGUI.HoverState.ForeColor = Color.White;
            btnGUI.Location = new Point(250, 110);
            btnGUI.Name = "btnGUI";
            btnGUI.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnGUI.Size = new Size(180, 70);
            btnGUI.TabIndex = 3;
            btnGUI.Text = "🖥️ GUI Mode";
            btnGUI.Click += btnGUI_Click;
            // 
            // lblSubtitle
            // 
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = Color.FromArgb(180, 180, 180);
            lblSubtitle.Location = new Point(30, 76);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(253, 19);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Choose how you want to access the panel?\r\n";
            // 
            // ModeSelectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 25, 30);
            ClientSize = new Size(460, 220);
            Controls.Add(guna2Panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ModeSelectionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Launch Mode";
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2Button btnStreamer;
        private Guna.UI2.WinForms.Guna2Button btnGUI;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSubtitle;
    }
}