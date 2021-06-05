namespace Sys0Decompiler
{
    partial class FileFormatSelectionForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.lstFileType = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(241, 139);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(322, 139);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // lstFileType
            // 
            this.lstFileType.FormattingEnabled = true;
            this.lstFileType.Items.AddRange(new object[] {
            "ALD File [AliceSoft]",
            "DAT File [AliceSoft] for System 3.0",
            "ALK File [AliceSoft]",
            "AFA File v1 [AliceSoft] for Daiteikoku, Rance Quest, Oyakorankan, etc.",
            "AFA File v2 [AliceSoft] for Pastel Chime 3, Drapeko, Rance 01, Rance IX, etc.",
            "VFS File 1.01 [SofthouseChara] for Wizard\'s Climber, Suzukuri Dragon, etc.",
            "VFS File 2.00 [SofthouseChara] for Bunny Black, Bunny Black 2, etc.",
            "ARC File [HoneyBee ASGARD] for Starry Sky"});
            this.lstFileType.Location = new System.Drawing.Point(12, 12);
            this.lstFileType.Name = "lstFileType";
            this.lstFileType.ScrollAlwaysVisible = true;
            this.lstFileType.Size = new System.Drawing.Size(385, 121);
            this.lstFileType.TabIndex = 7;
            this.lstFileType.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstFileType_MouseDoubleClick);
            this.lstFileType.DoubleClick += new System.EventHandler(this.lstFileType_DoubleClick);
            // 
            // FileFormatSelectionForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(409, 171);
            this.Controls.Add(this.lstFileType);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileFormatSelectionForm";
            this.Text = "Select File Format:";
            this.Load += new System.EventHandler(this.FileFormatSelectionForm_Load);
            this.Shown += new System.EventHandler(this.FileFormatSelectionForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ListBox lstFileType;
    }
}