namespace AGV_ANALYZE_LIST
{
    partial class MainForm
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
            this.ucLogger = new DebugForm.ucLog();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelProgress = new System.Windows.Forms.Label();
            this.buttonClick = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ucLogger
            // 
            this.ucLogger.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucLogger.Location = new System.Drawing.Point(0, 89);
            this.ucLogger.Name = "ucLogger";
            this.ucLogger.Size = new System.Drawing.Size(533, 188);
            this.ucLogger.TabIndex = 0;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(32, 41);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 22);
            this.buttonOpen.TabIndex = 1;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(391, 42);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Location = new System.Drawing.Point(388, 26);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(16, 13);
            this.labelProgress.TabIndex = 3;
            this.labelProgress.Text = "...";
            // 
            // buttonClick
            // 
            this.buttonClick.Location = new System.Drawing.Point(192, 41);
            this.buttonClick.Name = "buttonClick";
            this.buttonClick.Size = new System.Drawing.Size(75, 23);
            this.buttonClick.TabIndex = 4;
            this.buttonClick.Text = "Click";
            this.buttonClick.UseVisualStyleBackColor = true;
            this.buttonClick.Click += new System.EventHandler(this.buttonClick_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 277);
            this.Controls.Add(this.buttonClick);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.ucLogger);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DebugForm.ucLog ucLogger;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Button buttonClick;
    }
}

