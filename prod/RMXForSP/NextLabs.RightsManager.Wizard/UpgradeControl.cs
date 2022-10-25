
using System;

namespace NextLabs.RightsManager.Wizard
{
    public partial class UpgradeControl : InstallerControl
    {
        private readonly InstallProcessControl processControl;

        public UpgradeControl()
        {
            this.processControl = Program.CreateProcessControl();
            InitializeComponent();

            messageLabel.Text = InstallConfiguration.FormatString(messageLabel.Text);
        }

        protected internal override void Open(InstallOptions options)
        {
            Form.NextButton.Enabled = removeRadioButton.Checked;
            if (removeRadioButton.Checked)
            {
                Form.Operation = InstallOperation.Uninstall;
            }
        }

        protected internal override void Close(InstallOptions options)
        {
            Form.ContentControls.Add(processControl);
        }

        private void removeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (removeRadioButton.Checked)
            {
                Form.Operation = InstallOperation.Uninstall;
                Form.NextButton.Enabled = true;
            }
            else
            {
                Form.NextButton.Enabled = false;
            }
        }

        private void UpgradeControl_Load(object sender, EventArgs e)
        {

        }
    }
}
