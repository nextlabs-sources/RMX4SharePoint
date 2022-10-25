using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;

namespace NextLabs.RightsManager
{
    public class RmsController : LayoutsPageBase
    {
        protected DropDownList WebAppDropDown;
        protected TreeView FeatureTree;
        protected InputFormSection CentralAdminSection;
        protected InputFormSection OptionsSection;
        protected ImageButton RootSiteImageButton;
        protected LinkButton RootSiteLinkButton;
        protected Button UpdateButton;
        protected HtmlInputRadioButton OptionCheckBox;
        protected HtmlInputRadioButton DeactivateCheckBox;
        protected Literal PageDescription;
        protected InputFormControl ProgressBar;

        public string webAppName = string.Empty;
        private const string SiteIdSplit = ";";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "StartRmsFeatureStyling", "StyleFeatureList();", true);
                if (!Page.IsPostBack)
                {
                    SPWeb currentWeb = SPContext.Current.Web;
                    //PageDescription.Text = "NextLabs Rights Management administration";
                    if (currentWeb != null)
                    {
                        LoadWebAppDropDown();

                        if (!string.IsNullOrEmpty(WebAppDropDown.SelectedValue))
                        {
                            LoadTreeView();
                        }
                        else
                        {
                            ULSLogger.LogWarning( "RmsController Page_Load not select any web application.");
                            this.UpdateButton.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            bool bRet = false;
            try
            {
                if (String.IsNullOrEmpty(WebAppDropDown.SelectedValue))
                {
                    return;
                }
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    bool bActive = OptionCheckBox.Checked;
                    SPWebApplication selWebApp = null;
                    List<string> selectedSiteIDs = null;
                    List<string> activedSiteIDs = new List<string>();
                    try
                    {
                        selectedSiteIDs = GetSelectedSites();
                        // Update features for selected sites.
                        SPFarm farm = SPFarm.Local;
                        SPWebService spws = farm.Services.GetValue<SPWebService>("");
                        selWebApp = spws.WebApplications[new Guid(WebAppDropDown.SelectedValue)];

                        //Activate or deactive Rights Management Extension feature
                        EventReceiversModule.AddOrRemoveFeatureForWebApp(selWebApp, selectedSiteIDs, bActive);

                        bRet = EventReceiversModule.CheckFeaturesAndEvents(selWebApp, selectedSiteIDs, bActive, activedSiteIDs);
                    }
                    catch (Exception exp)
                    {
                        ULSLogger.LogError("AddOrRemoveFeatureForWebApp Error: " + exp.ToString());
                        bRet = false;
                    }
                    // Set web appliaction Rights Management Extension information.
                    if (selWebApp != null && selectedSiteIDs != null)
                    {
                        string strAction = bActive ? "Activate RMX " : "Deactive RMX ";
                        string strStatus = bRet ? "successfully" : "failed on some sites";
                        string strUpdateStatus = strAction + strStatus + " at " + DateTime.Now.ToString();

                        WebAppSetInfor webAppSetInfor = new WebAppSetInfor();
                        webAppSetInfor.WebAppRmsStatus = bActive ? Global.StrEnabled : Global.StrDisabled;
                        webAppSetInfor.WebAppSelectedSites = string.Join(";", selectedSiteIDs);
                        webAppSetInfor.WebAppActivatedSites = string.Join(";", activedSiteIDs);
                        webAppSetInfor.WebAppUpdateStatus = strUpdateStatus;

                        var serializer = new JavaScriptSerializer();
                        string strSetInfor = serializer.Serialize(webAppSetInfor);
                        selWebApp.Properties[Global.StrRmsWebAppSetInfor] = strSetInfor;
                        selWebApp.Update();

                        FeatureTree.Nodes.Clear();
                        LoadTreeView(selWebApp, webAppSetInfor);
                    }
                });
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        protected void ReturnButton_Click(object sender, EventArgs e)
        {
            string CAURL = this.Context.Request.Url.ToString().Replace(this.Context.Request.Url.AbsolutePath, "");
            SPUtility.Redirect(CAURL, SPRedirectFlags.Trusted, this.Context);
        }
       
        protected void WebAppDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadTreeView();
                WebAppDropDown.Focus();
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        private List<string> GetSelectedSites()
        {
            List<string> selectedSiteIDs = new List<string>();
            try
            {
                //To set the selected site colleciton EM's switch
                foreach (TreeNode webAppNode in FeatureTree.Nodes)
                {
                    if (webAppNode.Value.Equals(WebAppDropDown.SelectedValue))
                    {
                        foreach (TreeNode sitecollectionNode in webAppNode.ChildNodes)
                        {
                            if (!String.IsNullOrEmpty(sitecollectionNode.Value) && sitecollectionNode.Checked)
                            {
                                selectedSiteIDs.Add(sitecollectionNode.Value);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            return selectedSiteIDs;
        }

        private void LoadWebAppDropDown()
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    WebAppDropDown.Items.Clear();
                    SPWebApplication adminWebApp = SPAdministrationWebApplication.Local;
                    if (adminWebApp.Properties.ContainsKey("DeployedWebApps"))
                    {
                        string deployedWebApps = adminWebApp.Properties["DeployedWebApps"] as string;
                        foreach (SPWebApplication webApp in SPWebService.ContentService.WebApplications)
                        {
                            if (deployedWebApps.Contains(webApp.Id.ToString()))
                            {
                                WebAppDropDown.Items.Add(new ListItem(webApp.Name + " - " + webApp.GetResponseUri(SPUrlZone.Default).ToString(), webApp.Id.ToString()));
                            }
                        }
                    }
                });
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        private void LoadTreeView()
        {
            try
            {
                if (SPContext.Current.Site.WebApplication.IsAdministrationWebApplication)
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        FeatureTree.Nodes.Clear();
                        if (WebAppDropDown.SelectedValue != null && WebAppDropDown.SelectedValue != "")
                        {
                            SPFarm farm = SPFarm.Local;
                            SPWebService spws = farm.Services.GetValue<SPWebService>("");
                            SPWebApplication selWebApp = spws.WebApplications[new Guid(WebAppDropDown.SelectedValue)];
                            WebAppSetInfor webAppSetInfor = GetWebAppSetInfor(selWebApp);
                            LoadTreeView(selWebApp, webAppSetInfor);
                        }
                    });
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        private void LoadTreeView(SPWebApplication selWebApp, WebAppSetInfor webAppSetInfor)
        {
            try
            {
                //Set Update status for web App
                ProgressBar.LabelText = webAppSetInfor.WebAppUpdateStatus;

                //Set the Checkbox.
                bool bWebAppStatus = webAppSetInfor.WebAppRmsStatus.Equals(Global.StrEnabled);
                OptionCheckBox.Checked = bWebAppStatus;
                DeactivateCheckBox.Checked = !bWebAppStatus;

                //To update the Treeview.
                TreeNode webAppNode = new TreeNode(selWebApp.Name + " - " + selWebApp.GetResponseUri(SPUrlZone.Default).ToString(), selWebApp.Id.ToString());
                webAppName = selWebApp.Name;
                webAppNode.NavigateUrl = "javascript:CheckUncheckTree();";
                webAppNode.ToolTip = "Click to select/unselect Site Collections (including SubSites)";
                webAppNode.ShowCheckBox = false;
                webAppNode.ImageUrl = bWebAppStatus ? Global.StrGifActiveUrl : Global.StrGifDeactiveUrl;
                webAppNode.Expand();

                string strSelectedSites = webAppSetInfor.WebAppSelectedSites;
                string strActivatedSites = webAppSetInfor.WebAppActivatedSites;
                foreach (SPSite site in selWebApp.Sites)
                {
                    if (site == null)
                    {
                        continue; // Don't care null site colletion.
                    }
                    using (site)
                    {
                        try
                        {
                            TreeNode siteRootNode = new TreeNode();
                            if (site.ReadOnly)
                            {
                                // Set ReadOnly site colletion.
                                siteRootNode.Value = "";
                                siteRootNode.ShowCheckBox = false;
                                siteRootNode.Checked = false;
                                siteRootNode.Text = site.RootWeb.Title + " (Site Collection - ReadOnly : Not Supported)";
                                siteRootNode.ImageUrl = Global.StrGifDeactiveUrl;
                            }
                            else
                            {
                                siteRootNode.Value = site.ID.ToString();
                                siteRootNode.ShowCheckBox = true;
                                siteRootNode.Checked = strSelectedSites.Contains(site.ID.ToString());
                                siteRootNode.Text = site.RootWeb.Title + " (Site Collection)";
                                siteRootNode.ImageUrl = strActivatedSites.Contains(site.ID.ToString()) ? Global.StrGifActiveUrl : Global.StrGifDeactiveUrl;
                            }
                            webAppNode.ChildNodes.Add(siteRootNode);
                        }
                        catch (Exception ex)
                        {
                            ULSLogger.LogError(ex.ToString());

                        }
                    }
                }
                FeatureTree.Nodes.Add(webAppNode);
                if (!Page.IsPostBack)
                {
                    webAppNode.CollapseAll();
                    webAppNode.Expand();
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        private WebAppSetInfor GetWebAppSetInfor(SPWebApplication webApp)
        {
            WebAppSetInfor webAppSetInfor = new WebAppSetInfor();
            try
            {
                if (webApp.Properties.ContainsKey(Global.StrRmsWebAppSetInfor))
                {
                    string strSetInfor = webApp.Properties[Global.StrRmsWebAppSetInfor] as string;
                    var serializer = new JavaScriptSerializer();
                    webAppSetInfor = (WebAppSetInfor)serializer.Deserialize(strSetInfor, typeof(WebAppSetInfor));
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            return webAppSetInfor;
        }
    }
}