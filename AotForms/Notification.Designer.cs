namespace AotForms
{
    partial class Notification
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblMsg = new Label();
            SuspendLayout();
            // 
            // lblMsg
            // 
            lblMsg.Dock = DockStyle.Fill;
            lblMsg.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblMsg.ForeColor = Color.White;
            lblMsg.Location = new Point(0, 0);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(280, 80);
            lblMsg.TabIndex = 0;
            lblMsg.Text = "-----------";
            lblMsg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Notification
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 25, 25);
            ClientSize = new Size(280, 80);
            Controls.Add(lblMsg);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Notification";
            Text = "Notification";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private Label lblMsg;
    }
}