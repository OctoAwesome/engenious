namespace ContentTool.Forms.Dialogs
{
    partial class OverwriteDialog
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
            this.button_overwrite = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_skip = new System.Windows.Forms.Button();
            this.button_repeat = new System.Windows.Forms.Button();
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button_overwrite
            // 
            this.button_overwrite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_overwrite.Location = new System.Drawing.Point(10, 75);
            this.button_overwrite.Name = "button_overwrite";
            this.button_overwrite.Size = new System.Drawing.Size(75, 23);
            this.button_overwrite.TabIndex = 0;
            this.button_overwrite.Text = "Overwrite";
            this.button_overwrite.UseVisualStyleBackColor = true;
            this.button_overwrite.Click += new System.EventHandler(this.button_overwrite_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "File x already exists at the destination.";
            // 
            // button_skip
            // 
            this.button_skip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_skip.Location = new System.Drawing.Point(91, 75);
            this.button_skip.Name = "button_skip";
            this.button_skip.Size = new System.Drawing.Size(75, 23);
            this.button_skip.TabIndex = 2;
            this.button_skip.Text = "Skip";
            this.button_skip.UseVisualStyleBackColor = true;
            this.button_skip.Click += new System.EventHandler(this.button_skip_Click);
            // 
            // button_repeat
            // 
            this.button_repeat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_repeat.Location = new System.Drawing.Point(172, 75);
            this.button_repeat.Name = "button_repeat";
            this.button_repeat.Size = new System.Drawing.Size(75, 23);
            this.button_repeat.TabIndex = 3;
            this.button_repeat.Text = "Repeat";
            this.button_repeat.UseVisualStyleBackColor = true;
            this.button_repeat.Click += new System.EventHandler(this.button_repeat_Click);
            // 
            // checkBox
            // 
            this.checkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox.AutoSize = true;
            this.checkBox.Location = new System.Drawing.Point(301, 79);
            this.checkBox.Name = "checkBox";
            this.checkBox.Size = new System.Drawing.Size(83, 17);
            this.checkBox.TabIndex = 4;
            this.checkBox.Text = "Remember?";
            this.checkBox.UseVisualStyleBackColor = true;
            this.checkBox.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // OverwriteDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 108);
            this.Controls.Add(this.checkBox);
            this.Controls.Add(this.button_repeat);
            this.Controls.Add(this.button_skip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_overwrite);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OverwriteDialog";
            this.ShowInTaskbar = false;
            this.Text = "Overwrite?";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OverwriteDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_overwrite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_skip;
        private System.Windows.Forms.Button button_repeat;
        private System.Windows.Forms.CheckBox checkBox;
    }
}