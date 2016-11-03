namespace F1mpEditor
{
    partial class OptionsForm
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
            this.PathGroupBox = new System.Windows.Forms.GroupBox();
            this.PathTextBox = new System.Windows.Forms.TextBox();
            this.PathLabel = new System.Windows.Forms.Label();
            this.ChangeGameFolderButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.GameFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.PathGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // PathGroupBox
            // 
            this.PathGroupBox.Controls.Add(this.PathTextBox);
            this.PathGroupBox.Controls.Add(this.PathLabel);
            this.PathGroupBox.Location = new System.Drawing.Point(12, 12);
            this.PathGroupBox.Name = "PathGroupBox";
            this.PathGroupBox.Size = new System.Drawing.Size(440, 45);
            this.PathGroupBox.TabIndex = 0;
            this.PathGroupBox.TabStop = false;
            this.PathGroupBox.Text = "Current game folder path";
            // 
            // PathTextBox
            // 
            this.PathTextBox.Location = new System.Drawing.Point(41, 19);
            this.PathTextBox.Name = "PathTextBox";
            this.PathTextBox.ReadOnly = true;
            this.PathTextBox.Size = new System.Drawing.Size(393, 20);
            this.PathTextBox.TabIndex = 0;
            this.PathTextBox.TabStop = false;
            // 
            // PathLabel
            // 
            this.PathLabel.AutoSize = true;
            this.PathLabel.Location = new System.Drawing.Point(6, 23);
            this.PathLabel.Name = "PathLabel";
            this.PathLabel.Size = new System.Drawing.Size(29, 13);
            this.PathLabel.TabIndex = 0;
            this.PathLabel.Text = "Path";
            // 
            // ChangeGameFolderButton
            // 
            this.ChangeGameFolderButton.Location = new System.Drawing.Point(191, 63);
            this.ChangeGameFolderButton.Name = "ChangeGameFolderButton";
            this.ChangeGameFolderButton.Size = new System.Drawing.Size(180, 23);
            this.ChangeGameFolderButton.TabIndex = 1;
            this.ChangeGameFolderButton.Text = "Change path to game folder...";
            this.ChangeGameFolderButton.UseVisualStyleBackColor = true;
            this.ChangeGameFolderButton.Click += new System.EventHandler(this.ChangeGameFolderButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(377, 63);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 2;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // GameFolderBrowserDialog
            // 
            this.GameFolderBrowserDialog.ShowNewFolderButton = false;
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 98);
            this.Controls.Add(this.ChangeGameFolderButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.PathGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.PathGroupBox.ResumeLayout(false);
            this.PathGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox PathGroupBox;
        private System.Windows.Forms.TextBox PathTextBox;
        private System.Windows.Forms.Label PathLabel;
        private System.Windows.Forms.Button ChangeGameFolderButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.FolderBrowserDialog GameFolderBrowserDialog;
    }
}