
using System;
using System.Windows.Forms;
using Microsoft.SharePoint.Administration;

namespace NextLabs.RightsManager.Wizard
{
    public partial class DeploymentTargetsControl : InstallerControl
    {
        public DeploymentTargetsControl()
        {
            InitializeComponent();
            webApplicationsCheckedListBox.ItemCheck += new ItemCheckEventHandler(webApplicationsCheckedListBox_ItemCheck);
            this.Load += new EventHandler(Control_Load);
        }

        private void Control_Load(object sender, EventArgs e)
        {
            ConfigureWebApplicationList();
        }

        private void webApplicationsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            WebApplicationInfo info = (WebApplicationInfo)webApplicationsCheckedListBox.Items[e.Index];
            if (info.Required)
            {
                e.NewValue = CheckState.Indeterminate;
            }
        }

        protected internal override void Close(InstallOptions options)
        {
            options.WebApplicationTargets.Clear();
            foreach (WebApplicationInfo info in webApplicationsCheckedListBox.CheckedItems)
            {
                options.WebApplicationTargets.Add(info.Application);
            }
        }

        private void webApplicationsCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enable = webApplicationsCheckedListBox.CheckedItems.Count > 0;
            Form.NextButton.Enabled = enable;
        }

        private void ConfigureWebApplicationList()
        {
            foreach (SPWebApplication application in SPWebService.AdministrationService.WebApplications)
            {
                WebApplicationInfo webAppInfo = new WebApplicationInfo(application, true);
                webApplicationsCheckedListBox.Items.Add(webAppInfo, CheckState.Indeterminate);
            }

            foreach (SPWebApplication application in SPWebService.ContentService.WebApplications)
            {
                WebApplicationInfo webAppInfo = new WebApplicationInfo(application, false);
                webApplicationsCheckedListBox.Items.Add(webAppInfo, CheckState.Checked);
            }
        }

        private class WebApplicationInfo
        {
            private readonly SPWebApplication application;
            private readonly bool required;

            internal WebApplicationInfo(SPWebApplication application, bool required)
            {
                this.application = application;
                this.required = required;
            }

            internal SPWebApplication Application
            {
                get { return application; }
            }

            public bool Required
            {
                get { return required; }
            }

            public bool IsSRP
            {
                get { return application.Properties.ContainsKey("Microsoft.Office.Server.SharedResourceProvider"); }
            }

            public override string ToString()
            {
                string str = application.GetResponseUri(SPUrlZone.Default).ToString();

                if (application.IsAdministrationWebApplication)
                {
                    str += "     (Central Administration)";
                }
                else if (IsSRP)
                {
                    str += "     (Shared Resource Provider)";
                }
                else if (!String.IsNullOrEmpty(application.DisplayName))
                {
                    str += "     (" + application.DisplayName + ")";
                }
                else if (!String.IsNullOrEmpty(application.Name))
                {
                    str += "     (" + application.Name + ")";
                }

                return str;
            }
        }
    }
}
