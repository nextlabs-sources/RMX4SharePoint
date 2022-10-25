using System;

namespace NextLabs.RightsManager.Wizard
{
    public partial class CompletionControl : InstallerControl
    {
        public CompletionControl()
        {
            InitializeComponent();
            this.Load += new EventHandler(CompletionControl_Load);
        }

        void CompletionControl_Load(object sender, EventArgs e)
        {
        }

        public string Details
        {
            get { return detailsTextBox.Text; }
            set { detailsTextBox.Text = value; }
        }

        protected internal override void Open(InstallOptions options)
        {
            Form.PrevButton.Enabled = false;
        }
    }
}
