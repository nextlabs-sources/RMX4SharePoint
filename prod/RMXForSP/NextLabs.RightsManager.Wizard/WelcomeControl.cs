using System;

namespace NextLabs.RightsManager.Wizard
{
    public partial class WelcomeControl : InstallerControl
    {
        public WelcomeControl()
        {
            InitializeComponent();
            messageLabel.Text = InstallConfiguration.FormatString(messageLabel.Text);
        }

        private void WelcomeControl_Load(object sender, EventArgs e)
        {

        }
    }
}
