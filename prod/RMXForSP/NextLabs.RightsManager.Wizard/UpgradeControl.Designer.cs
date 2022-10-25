namespace NextLabs.RightsManager.Wizard
{
    partial class UpgradeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpgradeControl));
            this.removeRadioButton = new System.Windows.Forms.CheckBox();
            this.messageLabel = new System.Windows.Forms.Label();
            this.hintLabel = new System.Windows.Forms.Label();
            this.removeDescriptionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // removeRadioButton
            // 
            resources.ApplyResources(this.removeRadioButton, "removeRadioButton");
            this.removeRadioButton.Checked = false;
            this.removeRadioButton.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.removeRadioButton.ImageKey = global::NextLabs.RightsManager.Wizard.Resources.CommonUIStrings_en_US.controlSubTitleOptions;
            this.removeRadioButton.Name = "removeRadioButton";
            this.removeRadioButton.UseVisualStyleBackColor = true;
            this.removeRadioButton.CheckedChanged += new System.EventHandler(this.removeRadioButton_CheckedChanged);
            // 
            // messageLabel
            // 
            resources.ApplyResources(this.messageLabel, "messageLabel");
            this.messageLabel.ImageKey = global::NextLabs.RightsManager.Wizard.Resources.CommonUIStrings_en_US.controlSubTitleOptions;
            this.messageLabel.Name = "messageLabel";
            // 
            // hintLabel
            // 
            resources.ApplyResources(this.hintLabel, "hintLabel");
            this.hintLabel.ImageKey = global::NextLabs.RightsManager.Wizard.Resources.CommonUIStrings_en_US.controlSubTitleOptions;
            this.hintLabel.Name = "hintLabel";
            // 
            // removeDescriptionLabel
            // 
            resources.ApplyResources(this.removeDescriptionLabel, "removeDescriptionLabel");
            this.removeDescriptionLabel.ImageKey = global::NextLabs.RightsManager.Wizard.Resources.CommonUIStrings_en_US.controlSubTitleOptions;
            this.removeDescriptionLabel.Name = "removeDescriptionLabel";            
            // 
            // UpgradeControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.removeDescriptionLabel);
            this.Controls.Add(this.hintLabel);
            this.Controls.Add(this.removeRadioButton);
            this.Name = "UpgradeControl";
            this.Load += new System.EventHandler(this.UpgradeControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox removeRadioButton;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.Label hintLabel;
        private System.Windows.Forms.Label removeDescriptionLabel;
    }
}
