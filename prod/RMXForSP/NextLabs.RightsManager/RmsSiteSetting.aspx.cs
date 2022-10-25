using System;
using System.Threading;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace NextLabs.RightsManager
{
    public class RmsSiteSetting : LayoutsPageBase
    {
        protected DropDownList SiteLevel;
        protected Button BtnOK;
        protected HtmlInputHidden sitePropertyJson;

        public RmsSiteSetting()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
                bool bAjax = false;
                try
                {
                    string ajaxMethod = Request["ajaxMethod"].ToString();
                    System.Diagnostics.Trace.WriteLine("ajaxMethod:" + ajaxMethod);
                    System.Reflection.MethodInfo method = this.GetType().GetMethod(ajaxMethod);
                    if (method != null)
                    {
                        bAjax = true;
                        method.Invoke(this, new object[] { });
                        Response.End();
                    }
                }
                catch (Exception)
                {
                }
               

                // Validate the Page Request to avoid any malicious posts
                if (Request.HttpMethod == "POST"&&!bAjax)
                    SPUtility.ValidateFormDigest();

                if (!Page.IsPostBack&&!bAjax)
                {
                    SitePropSetInfor propSetInfor = Global.GetSitePropSetInfor(this.Web);
                    SiteLevel.SelectedIndex = propSetInfor.Level;
                    System.Diagnostics.Trace.WriteLine("SiteLevel.SelectedIndex:"+ SiteLevel.SelectedIndex);
                    List<ZSiteNodeModel> znodeList = new List<ZSiteNodeModel>();
                    InitSiteProperty(znodeList, propSetInfor.siteProp);
                    System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    sitePropertyJson.Value = javaScriptSerializer.Serialize(znodeList);
                }
            }
            catch (Exception exp)
            {
                if(exp.GetType().ToString()!= "System.Threading.ThreadAbortException")
                {
                    ULSLogger.LogError(exp.ToString());
                }
            }
        }

        private void InitSiteProperty(List<ZSiteNodeModel> znodeList,string siteProperties)
        {
            System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<ZSiteNodeModel> nodes = javaScriptSerializer.Deserialize<List<ZSiteNodeModel>>(siteProperties);
            if (nodes == null)
            {
                nodes = new List<ZSiteNodeModel>();
            }

            ZSiteNodeModel rootNode = new ZSiteNodeModel();
            rootNode.name = this.Web.Title;
            rootNode.id = this.Web.Url;
            rootNode.pId = "0";
            if (this.Web.Webs.Count > 0)
            {
                rootNode.isParent = true;
            }
            rootNode.isLoaded = true;
            ZSiteNodeModel node = nodes.Where(p=>p.id==this.Web.Url).FirstOrDefault();
            rootNode.siteProperties = GetWebProperty(this.Web, node);
            znodeList.Add(rootNode);
           
            foreach (SPWeb subWeb in this.Web.Webs)
            {
                ZSiteNodeModel subNode = new ZSiteNodeModel();
                subNode.name = subWeb.Title;
                subNode.id = subWeb.Url;
                subNode.pId = this.Web.Url;
                if (subWeb.Webs.Count > 0)
                {
                    subNode.isParent = true;
                }
                node = nodes.Where(p => p.id == subWeb.Url).FirstOrDefault();
                subNode.siteProperties = GetWebProperty(subWeb, node);
                znodeList.Add(subNode);
            }
        }
        private List<SitePropertyModel> GetWebProperty(SPWeb web, ZSiteNodeModel node)
        {
            List<SitePropertyModel> siteProperties = new List<SitePropertyModel>();
            try
            {
                foreach (DictionaryEntry dic in web.AllProperties)
                {
                    var prop = new SitePropertyModel();
                    prop.displayName = dic.Key.ToString();
                    if (prop.displayName == "")
                    {
                        continue;
                    }
                    if (node != null)
                    {
                        var property = node.siteProperties.Where(p => p.displayName == dic.Key.ToString()).FirstOrDefault();
                        if (property != null)
                        {
                            prop.@checked = true;
                        }
                    }
                    siteProperties.Add(prop);
                }
                siteProperties = siteProperties.OrderBy(p => p.displayName).ToList();
            }
            catch (Exception ex)
            {
                var prop = new SitePropertyModel();
                prop.displayName = "failed,please try again";
                siteProperties.Add(prop);
                ULSLogger.LogError(ex.ToString());
            }

            return siteProperties;
        }
        public void AsyncSubSiteNode()
        {
            var id = Request.Params["id"];
            System.Diagnostics.Trace.WriteLine("AsyncSubSiteNode enter");
            System.Diagnostics.Trace.WriteLine("id:" + id);
            System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var znodeList = new List<ZSiteNodeModel>();
            var result = "";
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(id))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SitePropSetInfor propSetInfor = Global.GetSitePropSetInfor(site.RootWeb);
                            List<ZSiteNodeModel> nodes = javaScriptSerializer.Deserialize<List<ZSiteNodeModel>>(propSetInfor.siteProp);
                            if (nodes == null)
                            {
                                nodes = new List<ZSiteNodeModel>();
                            }
                            foreach (SPWeb subWeb in web.Webs)
                            {
                                System.Diagnostics.Trace.WriteLine("foreach webs enter");
                                ZSiteNodeModel subNode = new ZSiteNodeModel();
                                subNode.name = subWeb.Title;
                                subNode.id = subWeb.Url;
                                subNode.pId = id;
                                if(subWeb.Webs.Count>0)
                                {
                                    subNode.isParent = true;
                                }
                                ZSiteNodeModel node = nodes.Where(p => p.id == subWeb.Url).FirstOrDefault();
                                subNode.siteProperties = GetWebProperty(subWeb, node);
                                znodeList.Add(subNode);
                                System.Diagnostics.Trace.WriteLine("foreach webs end");

                            }

                            result = javaScriptSerializer.Serialize(znodeList);
                            System.Diagnostics.Trace.WriteLine("result:" + result);
                        }
                    }
                });
                Response.Write(result);
            }
            catch (Exception ex)
            {
                ULSLogger.LogError(ex.ToString());
                znodeList = new List<ZSiteNodeModel>();
                var node = new ZSiteNodeModel();
                node.name = "failed,please try again";
                node.id = Guid.NewGuid().ToString();
                node.pId = id;
                znodeList.Add(node);
                result = javaScriptSerializer.Serialize(znodeList);
                Response.Write(result);
            }
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }

        protected void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                SPWeb web = SPControl.GetContextWeb(this.Context);
                if (web != null)
                {
                    SitePropSetInfor propSetInfor = new SitePropSetInfor();
                    propSetInfor.Level = SiteLevel.SelectedIndex;
                    propSetInfor.siteProp = this.sitePropertyJson.Value;
                    //propSetInfor.SelectAll = SelectedAll.Checked;
                    //propSetInfor.PropertyNames = PropertyNames.Text;
                    //propSetInfor.MapedNames = MapedNames.Text;
                    Global.SetSitePropSetInfor(web, propSetInfor);
                }
                Response.Redirect(Request.RawUrl, false);
                this.Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        //protected void SelectAllCheck_Changed(object sender, EventArgs e)
        //{
        //    PropertySetting.Visible = !SelectedAll.Checked;
        //}

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Page.Validate();
            if (this.Page.IsValid)
            {
                SPUtility.Redirect("settings.aspx", SPRedirectFlags.UseSource | SPRedirectFlags.RelativeToLayoutsPage, this.Context);
            }
        }
    }
}
