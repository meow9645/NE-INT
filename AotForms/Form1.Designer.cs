namespace AotForms
{
    partial class Form1
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            headerPanel = new Panel();
            label1 = new Label();
            headerTitleLabel = new Label();
            separator = new Panel();
            guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            label2 = new Label();
            statuslbl = new Label();
            headerPanel.SuspendLayout();
            guna2Panel3.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(35, 35, 42);
            headerPanel.Controls.Add(label1);
            headerPanel.Controls.Add(headerTitleLabel);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Padding = new Padding(10, 0, 10, 0);
            headerPanel.Size = new Size(320, 50);
            headerPanel.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 8F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(180, 180, 200);
            label1.Location = new Point(103, 34);
            label1.Name = "label1";
            label1.Size = new Size(113, 13);
            label1.TabIndex = 2;
            label1.Text = "Developed By B!LLA";
            label1.Click += label1_Click;
            // 
            // headerTitleLabel
            // 
            headerTitleLabel.Dock = DockStyle.Fill;
            headerTitleLabel.Font = new Font("Sitka Small", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            headerTitleLabel.ForeColor = Color.Red;
            headerTitleLabel.Location = new Point(10, 0);
            headerTitleLabel.Name = "headerTitleLabel";
            headerTitleLabel.Size = new Size(300, 50);
            headerTitleLabel.TabIndex = 3;
            headerTitleLabel.Text = "STREAMER ULT!MATEv5";
            headerTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            headerTitleLabel.Click += headerTitleLabel_Click;
            // 
            // separator
            // 
            separator.BackColor = Color.FromArgb(232, 24, 74);
            separator.Dock = DockStyle.Top;
            separator.Location = new Point(0, 50);
            separator.Name = "separator";
            separator.Size = new Size(320, 2);
            separator.TabIndex = 1;
            // 
            // guna2Panel3
            // 
            guna2Panel3.BackColor = Color.Transparent;
            guna2Panel3.BorderColor = Color.Transparent;
            guna2Panel3.Controls.Add(label2);
            guna2Panel3.Controls.Add(statuslbl);
            guna2Panel3.CustomizableEdges = customizableEdges1;
            guna2Panel3.Dock = DockStyle.Fill;
            guna2Panel3.FillColor = Color.FromArgb(17, 17, 22);
            guna2Panel3.Location = new Point(0, 52);
            guna2Panel3.Name = "guna2Panel3";
            guna2Panel3.Padding = new Padding(25, 30, 25, 15);
            guna2Panel3.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2Panel3.Size = new Size(320, 408);
            guna2Panel3.TabIndex = 0;
            guna2Panel3.Paint += guna2Panel3_Paint;
            // 
            // label2
            // 
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(200, 200, 200);
            label2.Location = new Point(22, 149);
            label2.Name = "label2";
            label2.Size = new Size(270, 50);
            label2.TabIndex = 2;
            label2.Text = "LOADING...";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.Click += label2_Click;
            // 
            // statuslbl
            // 
            statuslbl.BackColor = Color.Transparent;
            statuslbl.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            statuslbl.ForeColor = Color.FromArgb(200, 200, 200);
            statuslbl.Location = new Point(22, 199);
            statuslbl.Name = "statuslbl";
            statuslbl.Size = new Size(270, 23);
            statuslbl.TabIndex = 1;
            statuslbl.Text = "STATUS: Wait...";
            statuslbl.TextAlign = ContentAlignment.MiddleCenter;
            statuslbl.Click += statuslbl_Click;
            // 
            // Form1
            // 
            BackColor = Color.FromArgb(17, 17, 22);
            ClientSize = new Size(320, 460);
            Controls.Add(guna2Panel3);
            Controls.Add(separator);
            Controls.Add(headerPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Load += Form1_Load;
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            guna2Panel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ---------- Control declarations ----------
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label headerTitleLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel separator;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private System.Windows.Forms.Label statuslbl;
        private System.Windows.Forms.Label label2;
    }
}