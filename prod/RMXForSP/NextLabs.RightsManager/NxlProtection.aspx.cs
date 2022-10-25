using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SkyDrmRestHelp;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.Services;

namespace NextLabs.RightsManager
{
    public class NxlProtection : UnsecuredLayoutsPageBase
    {
        protected HtmlGenericControl FileName;
        protected Panel MainView;
        protected HtmlGenericControl MainContent;
        protected Panel ErrorView;
        protected HtmlGenericControl TagCountDiv;
        protected HtmlGenericControl InfoText;
        protected HtmlGenericControl ItemId;
        protected HtmlGenericControl ListId;
        protected HtmlGenericControl SiteUrl;
        private const string backSiteScript = "if(history.length>1) {history.back(-1);}else{window.close();}";

        public const int SuccessStatus = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                bool bAddControls = false;
                string strItemId = Request.Params["itemId"];
                string strListId = Request.Params["listId"];
                if (!string.IsNullOrEmpty(strItemId) && strItemId.Contains("."))
                {
                    // fix bug 55528, when id is "5.0.2019-06-03T10:00:00Z" for repeating calendar item.
                    strItemId = strItemId.Substring(0, strItemId.IndexOf("."));
                }
                ItemId.InnerText = strItemId;
                ListId.InnerText = strListId;
                SiteUrl.InnerText = this.Web.Url;
                SPList list = this.Web.Lists.GetList(new Guid(strListId), true);
                if (list != null)
                {
                    SPListItem listItem = list.GetItemById(int.Parse(strItemId));
                    if (listItem != null)
                    {
                        int templateId = (int)list.BaseTemplate;
                        //check is library or list type
                        if (EventReceiversModule.SupportedLibraryTypes.Contains(templateId))
                        {
                            //for folder or documentset
                            if (listItem.FileSystemObjectType != SPFileSystemObjectType.File)
                            {
                                Response.Write(string.Format("<script>alert('This file type does not support rights protection.');{0}</script>", backSiteScript));
                                return;
                            }
                            //for onenote 
                            if (listItem.File.Name.EndsWith(".one"))
                            {
                                Response.Write(string.Format("<script>alert('This file type does not support rights protection.');{0}</script>", backSiteScript));
                                return;
                            }
                            if (listItem.File.Name.EndsWith(".nxl"))
                            {
                                Response.Write(string.Format("<script>alert('This file is already rights protected.');{0}</script>", backSiteScript));
                                return;
                            }
                            if (!listItem.DoesUserHavePermissions(SPContext.Current.Web.CurrentUser, SPBasePermissions.EditListItems))
                            {
                                Response.Write(string.Format("<script>alert('You do not have permission to protect this item.');{0}</script>", backSiteScript));
                                return;
                            }
                            if (listItem.File.CheckOutType != SPFile.SPCheckOutType.None)
                            {
                                Response.Write(string.Format("<script>alert('Please check in before protecting this item.');{0}</script>", backSiteScript));
                                return;
                            }
                            if (!string.IsNullOrEmpty(listItem.File.LockId))
                            {
                                Response.Write(string.Format("<script>alert('This file is locked, please try it at later.');{0}</script>", backSiteScript));
                                return;
                            }
                            FileName.InnerText = "Protect File:" + listItem.File.Name;
                        }
                        else if (EventReceiversModule.SupportedListTypes.Contains(templateId))
                        {
                            if (!listItem.DoesUserHavePermissions(SPContext.Current.Web.CurrentUser, SPBasePermissions.EditListItems))
                            {
                                Response.Write(string.Format("<script>alert('You do not have permission to protect this item.');{0}</script>", backSiteScript));
                                return;
                            }
                            // get attachment of list
                            SPAttachmentCollection attachments = listItem.Attachments;
                            bool isNxl = true;
                            if (attachments.Count > 0)
                            {
                                foreach (var attachment in attachments)
                                {
                                    string fileUrl = attachments.UrlPrefix + attachment;
                                    SPFile itemFile = list.ParentWeb.GetFile(fileUrl);
                                    if (!itemFile.Name.EndsWith(".nxl"))
                                    {
                                        isNxl = false;
                                        break;
                                    }
                                }
                                if (isNxl)
                                {
                                    Response.Write(string.Format("<script>alert('This item does not support rights protection.');{0}</script>", backSiteScript));
                                    return;
                                }
                            }
                            else
                            {
                                Response.Write(string.Format("<script>alert('This list item have no attachment.');{0}</script>", backSiteScript));
                                return;
                            }
                            FileName.InnerText = "Protect File: all attachments in " + listItem.DisplayName;
                        }
                        else
                        {
                            Response.Write(string.Format("<script>alert('This list item does not support rights protection.');{0}</script>", backSiteScript));
                            return;
                        }
                        bAddControls = true;
                    }
                }
                LoginData ld = SkyDrmSessionMgr.GetSkyDrmLoginData(Request);
                if (ld == null)
                {
                    ld = SkyDrmSessionMgr.LoginSkyDrmByTrustApp(this.Web, Response);
                }
                if (ld == null)
                {
                    Response.Write(string.Format("<script>alert('Failed to log in to SkyDrm server.');{0}</script>", backSiteScript));
                    throw new Exception("LoginSkyDrmByTrustApp failed.");
                }

                //Dynamically add controls
                if (bAddControls)
                {
                    AddControl(ld, strListId, strItemId);
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        private void AddControl(LoginData ld, string strListId, string strItemId)
        {
            try
            {
                SkyDrmRestHelp.ClassificationResult classficationResult = SkyDrmSessionMgr.GetClassificationResult(ld);
                if (SkyDrmSessionMgr.IsUnAuth(classficationResult.statusCode))
                {
                    ld = SkyDrmSessionMgr.LoginSkyDrmByTrustApp(this.Web, Response);
                    if (ld == null)
                    {
                        throw new Exception("LoginSkyDrmByTrustApp failed.");
                    }
                    else
                    {
                        classficationResult = SkyDrmSessionMgr.GetClassificationResult(ld);
                    }
                }
                TagCountDiv.Attributes.Add("tagCount", classficationResult.data.categories.Count.ToString());
                if (classficationResult.statusCode == 200 && classficationResult.data.categories.Count > 0)
                {
                    int index = 1;
                    foreach (var catory in classficationResult.data.categories)
                    {
                        Panel titlePanel = new Panel();
                        titlePanel.CssClass = "catoryTitle";
                        titlePanel.ID = "catoryTitle" + index;

                        Label titleLabel = new Label();
                        titleLabel.CssClass = "catoryLabel";
                        titleLabel.Text = catory.name;
                        titleLabel.ID = "catoryLabel" + index;
                        titleLabel.Attributes.Add("name", "catoryLabel" + index);
                        titleLabel.Attributes.Add("multiSel", catory.multiSel.ToString());
                        titleLabel.Attributes.Add("mandatory", catory.mandatory.ToString());
                        titlePanel.Controls.Add(titleLabel);

                        Panel contextPanel = new Panel();
                        contextPanel.CssClass = "catoryContext";
                        contextPanel.ID = "catoryContext" + index;
                        contextPanel.Attributes.Add("name", "catoryContext" + index);
                        int btnIndex = 1;
                        foreach (var context in catory.labels)
                        {
                            HtmlGenericControl btn = new HtmlGenericControl("Input");
                            btn.Attributes.Add("type", "Button");
                            btn.Attributes.Add("name", "catoryContextButton" + index + btnIndex);
                            btn.Attributes.Add("class", "catoryContextButton");
                            btn.Attributes.Add("onclick", "updateTagBtn(this)");
                            btn.Attributes.Add("value", context.name);
                            btn.ID = "catoryContextButton" + index + btnIndex;
                            contextPanel.Controls.Add(btn);
                            btnIndex++;
                        }
                        MainContent.Controls.Add(titlePanel);
                        MainContent.Controls.Add(contextPanel);
                        index++;
                    }
                }
                else
                {
                    MainView.CssClass = "hide";
                    ErrorView.CssClass = "";
                }
            }
            catch (Exception exp)
            {
                InfoText.InnerText = "Error:Load classfication result failed, please try again at later .";
                MainView.CssClass = "hide";
                ErrorView.CssClass = "";
                ULSLogger.LogError(exp.ToString());
            }
        }

        [WebMethod]
        public static string Upoload(string tagArray, string itemId, string listId, string siteUrl)
        {
            string result = "0";
            bool bSuccess = false;
            System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, string> dicTags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var listDic = javaScriptSerializer.Deserialize<List<Tag>>(tagArray);
                foreach (var tag in listDic)
                {
                    string strValues = string.Join(Global.TagSeparator, tag.tagValue);
                    dicTags.Add(tag.tagName, strValues);
                }

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            bool oldSafe = web.AllowUnsafeUpdates;
                            web.AllowUnsafeUpdates = true;
                            ItemHandler itemHandle = new ItemHandler();
                            itemHandle.DisableEventFiring();
                            try
                            {
                                SPList list = web.Lists[new Guid(listId)];
                                SPListItem item = list.GetItemById(int.Parse(itemId));
                                ListSetInfor listSetInfor = Global.GetListSetInfor(list);
                                //check is library or list type
                                int templateId = (int)list.BaseTemplate;

                                if (EventReceiversModule.SupportedLibraryTypes.Contains(templateId))
                                {
                                    SPFile itemFile = web.GetFile(web.Url + "/" + item.Url);
                                    if (itemFile == null) return;
                                    if (itemFile.CheckOutType != SPFile.SPCheckOutType.None)
                                    {
                                        ULSLogger.LogWarning("Protect file [" + itemFile.Url + "] failed because the file is checkouted status.");
                                        result = "1";
                                        return;
                                    }
                                    if (!string.IsNullOrEmpty(itemFile.LockId))
                                    {
                                        ULSLogger.LogWarning("Protect file [" + itemFile.Url + "] failed because the file is locked status.");
                                        result = "1";
                                        return;
                                    }
                                    if (!RmxModule.EncryptItemVerstions(item, itemFile, web.Url + "/" + item.Url, dicTags, listSetInfor)) result = "1";
                                }
                                else if (EventReceiversModule.SupportedListTypes.Contains(templateId))
                                {
                                    int oldCount = item.Versions.Count;
                                    SPAttachmentCollection attachments = item.Attachments;
                                    foreach (string url in attachments)
                                    {
                                        string fileUrl = attachments.UrlPrefix + url;
                                        if (fileUrl.EndsWith(".nxl", StringComparison.OrdinalIgnoreCase))
                                        {
                                            continue;
                                        }
                                        SPFile itemFile = web.GetFile(fileUrl);
                                        if (itemFile == null) continue;
                                        if (itemFile.CheckOutType != SPFile.SPCheckOutType.None)
                                        {
                                            ULSLogger.LogWarning("Protect file [" + itemFile.Url + "] failed because the file is checkouted status.");
                                            result = "1";
                                            continue;
                                        }
                                        if (!string.IsNullOrEmpty(itemFile.LockId))
                                        {
                                            ULSLogger.LogWarning("Protect file [" + itemFile.Url + "] failed because the file is locked status.");
                                            result = "1";
                                            continue;
                                        }
                                        if (!RmxModule.EncryptItemVerstions(item, itemFile, fileUrl, dicTags, listSetInfor)) result = "1";   // failed
                                        else bSuccess = true;
                                    }
                                    Global.RemoveListItemNewVersions(web, web.Url + "/" + item.Url, oldCount);
                                }
                            }
                            catch (Exception ex)
                            {
                                result = "1";
                                ULSLogger.LogError(ex.ToString());
                            }
                            web.AllowUnsafeUpdates = oldSafe;
                            itemHandle.EnableEventFiring();
                        }
                    }
                });
            }
            catch (Exception exp)
            {
                result = "1";
                ULSLogger.LogError(exp.ToString());
            }
            if (result.Equals("1") && bSuccess) result = "2";   // some failed but some success

            return result;
        }
    }
    public class Tag
    {
        public string tagName { get; set; }
        public List<string> tagValue { get; set; }
    }

}
