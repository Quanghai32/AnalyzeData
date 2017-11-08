namespace DebugForm
{
    partial class ucLog
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBoxLogger = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBoxLogger
            // 
            this.richTextBoxLogger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLogger.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxLogger.Name = "richTextBoxLogger";
            this.richTextBoxLogger.Size = new System.Drawing.Size(150, 150);
            this.richTextBoxLogger.TabIndex = 0;
            this.richTextBoxLogger.Text = "";
            // 
            // ucLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextBoxLogger);
            this.Name = "ucLog";
            this.Load += new System.EventHandler(this.ucLog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxLogger;
    }
}
