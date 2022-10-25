
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using NextLabs.RightsManager.Wizard.Resources;


namespace NextLabs.RightsManager.Wizard
{
    public partial class FinishedControl : NextLabs.RightsManager.Wizard.InstallerControl
    {
        #region Constants
        public const int DefaultLinkHeight = 28;
        #endregion

        #region Member Variables
        private bool initialized = false;
        #endregion

        #region Constructor
        public FinishedControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Overrides
        protected internal override void Open(InstallOptions options)
        {
            if (!initialized)
            {
                initialized = true;
            }
            Form.AbortButton.Enabled = true;
        }
        #endregion

        #region Private Methods
        private string FormatRelativeLink(string relativeLink)
        {
            if (!relativeLink.StartsWith("/"))
            {
                relativeLink = "/" + relativeLink;
            }
            return relativeLink;
        }

        private void AddLink(string linkText, int linkStart, int linkLength, string url)
        {
            LinkLabel linkLabel = new LinkLabel();
            linkLabel.Text = linkText;
            linkLabel.LinkArea = new LinkArea(linkStart, linkLength);
            linkLabel.Links[0].LinkData = url;
            linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel_LinkClicked);
            linkLabel.Width = tableLayoutPanel.Width - 10;
            linkLabel.Height = DefaultLinkHeight;
            tableLayoutPanel.Controls.Add(linkLabel);
        }

        private void AddSspLinks(IList<SPWebApplication> webApplicationTargets, string relativeLink)
        {
            foreach (SPWebApplication webApp in webApplicationTargets)
            {
                Hashtable properties = webApp.Properties;
                if (properties.ContainsKey("Microsoft.Office.Server.SharedResourceProvider"))
                {
                    string linkText = InstallConfiguration.FormatString(CommonUIStrings.finishedLinkTextSsp, webApp.Sites[0].Url);

                    SPSite siteCollectionInner = null;
                    try
                    {
                        siteCollectionInner = webApp.Sites[0];
                        AddLink(linkText, 6, 4, siteCollectionInner.Url + relativeLink);
                    }
                    finally
                    {
                        if (siteCollectionInner != null)
                            siteCollectionInner.Dispose();
                    }
                }
            }
        }

        private void AddSiteCollectionLinks(IList<SPSite> siteCollectionTargets, string relativeLink)
        {
            foreach (SPSite siteCollection in siteCollectionTargets)
            {
                string linkText = InstallConfiguration.FormatString(CommonUIStrings.finishedLinkTextSiteCollection, siteCollection.Url);
                AddLink(linkText, 6, 4, siteCollection.Url + relativeLink);
            }
        }
        #endregion

        #region Event Handlers
        void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            if (target != null && (target.StartsWith("http") || target.StartsWith("www")))
            {
                System.Diagnostics.Process.Start(target);
            }
            else
            {
                MessageBox.Show(string.Format(CommonUIStrings.linkLabelDialogText, target));
            }
        }
        #endregion
        private void FinishedControl_Load(object sender, EventArgs e)
        {

        }
    }
}

