using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using QueryCloudAZSDK;
using QueryCloudAZSDK.CEModel;
using System.Web.Script.Serialization;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Office.Server.UserProfiles;
using System.Linq;

namespace NextLabs.RightsManager
{
    public enum SiteLevel
    {
        Both = 3,
        SiteCollection = 2,
        Subsite = 1,
        None = 0
    }

    public class ZNodeModel
    {
        public string id { get; set; } = "";
        public string name { get; set; } = "";
        public string pId { get; set; } = "";
        public bool @checked { get; set; } = false;
        public bool isParent { get; set; } = false;
        public bool isLoaded { get; set; } = false;
    }
    public class ZSiteNodeModel:ZNodeModel
    {
        public List<SitePropertyModel> siteProperties { get; set; } = new List<SitePropertyModel>();
    }

    public class ListSetInfor
    {
        public string BatchStatus;
        public string VersionsStatus;
        public string DeleteStatus;
        //public string BackupStatus;
        //public string BackupPath;
        public string BatchLog;
        public bool BatchSuccess;
        public List<BatchModeFailedModel> FailedItems;
        public string RunningTime;
        public string ScheduleList;
        public string SelectedColumns;
        public ListSetInfor()
        {
            BatchStatus = Global.StrDisabled;
            VersionsStatus = Global.StrDisabled;
            DeleteStatus = Global.StrDisabled;
            //BackupStatus = Global.StrDisabled;
            //BackupPath = "";
            BatchLog = "";
            BatchSuccess = true;
            FailedItems = new List<BatchModeFailedModel>();
            RunningTime = "";
            ScheduleList = "";
            SelectedColumns = "";
        }
    }

    public class SitePropSetInfor
    {
        public int Level;
        public string siteProp;
        public SitePropSetInfor()
        {
            Level = (int)SiteLevel.None; // Use "None" as default
            siteProp = "";
        }
    }

    public class SitePropertyModel
    {
        public string displayName { get; set; } = "";
        public bool @checked { get; set; } = false;
    }
    public class BatchModeFailedModel
    {
        public string FileUrl { get; set; }
        public string Reason { get; set; }
        public BatchModeFailedModel(string fileUrl, string reason)
        {
            this.FileUrl = fileUrl;
            this.Reason = reason;
        }
        public BatchModeFailedModel()
        {
            FileUrl = "";
            Reason = "";
        }
    }
    public class WebAppSetInfor
    {
        public string WebAppRmsStatus;
        public string WebAppSelectedSites;
        public string WebAppActivatedSites;
        public string WebAppUpdateStatus;
        public WebAppSetInfor()
        {
            WebAppRmsStatus = Global.StrDisabled;
            WebAppSelectedSites = "";
            WebAppActivatedSites = "";
            WebAppUpdateStatus = "";
        }
        public WebAppSetInfor(string status, string selectedSites, string activatedSites, string processLog)
        {
            WebAppRmsStatus = status;
            WebAppSelectedSites = selectedSites;
            WebAppActivatedSites = activatedSites;
            WebAppUpdateStatus = processLog;
        }
    }

    public class GeneralSetInfor
    {
        public string JavaPcHost;
        public string OAUTHHost;
        public string ClientSecureID;
        public string ClientSecureKey;
        public string RouterURL;
        public string AppId;
        public string AppKey;
        public string TenantName;
        public string CertificatePfxFileBase64;
        public string CertificatePassword;
        public string CertificateFileName;
        //public bool SelectAllColumns;
        //public string ColumnNames;
        public string SecureViewURL;
        public GeneralSetInfor()
        {
            JavaPcHost = "";
            OAUTHHost = "";
            ClientSecureID = "";
            ClientSecureKey = "";
            RouterURL = "";
            AppId = "";
            AppKey = "";
            TenantName = "";
            //SelectAllColumns = false;
            //ColumnNames = "";
            SecureViewURL = "";
            CertificatePfxFileBase64 = "";
            CertificatePassword = "";
        }
        public GeneralSetInfor(string strJavaPcHost, string strOAUTHHost, string strClientSecureID, string strClientSecureKey,
            string strRouterURL, string strAppId, string strAppKey, string strTenantName,string strSecureViewURL)
        {
            JavaPcHost = strJavaPcHost;
            OAUTHHost = strOAUTHHost;
            ClientSecureID = strClientSecureID;
            ClientSecureKey = strClientSecureKey;
            RouterURL = strRouterURL;
            AppId = strAppId;
            AppKey = strTenantName;
            TenantName = "";
            //ColumnNames = strColumnNames;
            SecureViewURL = strSecureViewURL;
            CertificatePfxFileBase64 = "";
            CertificatePassword = "";
        }
    }

    public class Global
    {
//#if SP2016
//        public const int LimitRightFileName = 15;
//        public const int LimitBackupFileName = 68;
//        public const int LimitBackupFileUrl = 260;
//#elif SP2019
//        public const int LimitRightFileName = 25;
//        public const int LimitBackupFileName = 128;
//        public const int LimitBackupFileUrl = 400;
//#endif
        public const int LimitTagSize = 60;

        public const string StrGifActiveUrl = "/_layouts/15/images/NextLabs.RightsManager/Active.gif";
        public const string StrGifDeactiveUrl = "/_layouts/15/images/NextLabs.RightsManager/Deactive.gif";

        public const string StrRmsWebAppSetInfor = "RmsWebAppSetInfor";
        public const string StrGeneralSetInfor = "GeneralSetInfor";
        public const string StrListSetInfor = "ListSetInfor";
        public const string StrSitePropSetInfor = "SitePropSetInfor";

        public const string StrEnabled = "Enabled";
        public const string StrDisabled = "Disabled";

        public const string StrRmxObName = "RMX";
        public const string StrRmxAction= "EDIT";
        public const string TagSeparator = "\n";
        public const string ColumnSeparator = "\r\n";
        public static char[] TrimFlag = new char[] { '/' };
        public static object LockListSetInfor = new object(); // lock for "ListSetInfor".

        public static GeneralSetInfor GeneralInfor = null;
        static Global()
        {
            GeneralInfor = Global.GetGeneralSetInfor();
        }      

        public static GeneralSetInfor GetGeneralSetInfor()
        {
            GeneralSetInfor generalSetInfor = new GeneralSetInfor();
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    SPWebApplication adminWebApp = SPAdministrationWebApplication.Local;
                    if (adminWebApp.Properties.ContainsKey(StrGeneralSetInfor))
                    {
                        string strSetInfor = adminWebApp.Properties[StrGeneralSetInfor] as string;
                        var serializer = new JavaScriptSerializer();
                        generalSetInfor = (GeneralSetInfor)serializer.Deserialize(strSetInfor, typeof(GeneralSetInfor));
                    }
                }
                catch (Exception exp)
                {
                    ULSLogger.LogError(exp.ToString());
                }

            });
            return generalSetInfor;
        }

        public static ListSetInfor GetListSetInfor(SPList splist)
        {
            ListSetInfor listSetInfor = new ListSetInfor();
            if (splist == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return listSetInfor;
            }
            try
            {
                lock (LockListSetInfor)
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(splist.ParentWeb.Url))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                SPList list = web.Lists[splist.ID];
                                if (list.RootFolder.Properties.ContainsKey(StrListSetInfor))
                                {
                                    string strSetInfor = list.RootFolder.Properties[StrListSetInfor] as string;
                                    var serializer = new JavaScriptSerializer();
                                    listSetInfor = (ListSetInfor)serializer.Deserialize(strSetInfor, typeof(ListSetInfor));
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            return listSetInfor;
        }

        public static void SetListSetInfor(SPList splist, ListSetInfor listSetInfor)
        {
            if (splist == null || listSetInfor == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return;
            }
            try
            {
                lock (LockListSetInfor)
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(splist.ParentWeb.Url))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                bool oldUpdate = web.AllowUnsafeUpdates;
                                web.AllowUnsafeUpdates = true;
                                SPList list = web.Lists[splist.ID];
                                var serializer = new JavaScriptSerializer();
                                string strSetInfor = serializer.Serialize(listSetInfor);
                                list.RootFolder.Properties[StrListSetInfor] = strSetInfor;
                                list.RootFolder.Update();
                                web.AllowUnsafeUpdates = oldUpdate;
                            }
                        }
                    });
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        public static SitePropSetInfor GetSitePropSetInfor(SPWeb web)
        {
            SitePropSetInfor propSetInfor = new SitePropSetInfor();
            if (web == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return propSetInfor;
            }
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(web.Url))
                    {
                        using (SPWeb curWeb = site.OpenWeb())
                        {
                            object objInfor = curWeb.GetProperty(StrSitePropSetInfor);
                            if (objInfor != null)
                            {
                                string strSetInfor = objInfor as string;
                                var serializer = new JavaScriptSerializer();
                                propSetInfor = (SitePropSetInfor)serializer.Deserialize(strSetInfor, typeof(SitePropSetInfor));
                            }
                        }
                    }
                });
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            return propSetInfor;
        }

        public static void SetSitePropSetInfor(SPWeb web, SitePropSetInfor propSetInfor)
        {
            if (web == null || propSetInfor == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return;
            }
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(web.Url))
                    {
                        using (SPWeb setWeb = site.OpenWeb())
                        {
                            bool oldUpdate = setWeb.AllowUnsafeUpdates;
                            setWeb.AllowUnsafeUpdates = true;
                            var serializer = new JavaScriptSerializer();
                            string strSetInfor = serializer.Serialize(propSetInfor);
                            setWeb.SetProperty(StrSitePropSetInfor, strSetInfor);
                            setWeb.Update();
                            setWeb.AllowUnsafeUpdates = oldUpdate;
                        }
                    }
                });
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        public static List<string> GetItemColumnValues(SPListItem item, List<string> listColumnName)
        {
            if (item == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return null;
            }
            List<string> listColumnValue = new List<string>();
            if (listColumnName.Count > 0)
            {
                foreach (string columnName in listColumnName)
                {
                    string columnValue = "";
                    foreach (SPField field in item.Fields)
                    {
                        if (columnName.Equals(field.Title, StringComparison.OrdinalIgnoreCase))
                        {
                            object objValue = item[field.InternalName];
                            if (objValue != null)
                            {
                                columnValue = field.GetFieldValueAsText(objValue);
                                if (!string.IsNullOrEmpty(columnValue))
                                {
                                    if (field.Type == SPFieldType.Invalid && field.TypeDisplayName.Equals("Managed Metadata", StringComparison.OrdinalIgnoreCase))
                                    {
                                        columnValue = columnValue.Replace(";", TagSeparator);
                                    }
                                    columnValue = columnValue.Replace("\r", TagSeparator);
                                }
                            }
                            break;
                        }
                    }
                    listColumnValue.Add(columnValue);
                }
            }
            return listColumnValue;
        }

        public static CEAttres GetItemAttrs(SPListItem item, Dictionary<string, string> dicColumn)
        {
            if (item == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return null;
            }
            CEAttres ceAttrs = new CEAttres();
            foreach (SPField field in item.Fields)
            {
                string key = field.Title;
                if (!string.IsNullOrEmpty(key) && (dicColumn.ContainsKey(field.Id.ToString())))
                {
                    System.Diagnostics.Trace.WriteLine("column:"+dicColumn[field.Id.ToString()]);
                    object objValue = item[field.InternalName];
                    if (objValue != null)
                    {
                        string strValue = field.GetFieldValueAsText(objValue);
                        if (!string.IsNullOrEmpty(strValue))
                        {
                            if (field.Type == SPFieldType.Invalid && field.TypeDisplayName.Equals("Managed Metadata", StringComparison.OrdinalIgnoreCase))
                            {
                                strValue = strValue.Replace(";", TagSeparator);
                            }
                            strValue = strValue.Replace("\r", TagSeparator);
                            string[] arrValue = strValue.Split(new string[] { TagSeparator }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string value in arrValue)
                            {
                                ceAttrs.AddAttribute(new CEAttribute(key, value, CEAttributeType.XacmlString));
                            }
                        }
                    }
                }
            }

            // Add Site propeties by configuration.
            SPWeb rootWeb = item.Web.Site.RootWeb;

            SitePropSetInfor propSetInfor = GetSitePropSetInfor(rootWeb);
            if(propSetInfor.Level == (int)SiteLevel.Subsite)
            {
                GetAttrFromSiteProp(item.Web, propSetInfor.siteProp, ceAttrs,false);
            }
            else if(propSetInfor.Level == (int)SiteLevel.SiteCollection)
            {
                GetAttrFromSiteProp(rootWeb, propSetInfor.siteProp, ceAttrs,true);
            }
            else if(propSetInfor.Level == (int)SiteLevel.Both)
            {
                if (item.Web.Url == rootWeb.Url)
                {
                    GetAttrFromSiteProp(rootWeb, propSetInfor.siteProp, ceAttrs, true);
                }
                else
                {
                    GetAttrFromSiteProp(item.Web, propSetInfor.siteProp, ceAttrs, false);
                    GetAttrFromSiteProp(rootWeb, propSetInfor.siteProp, ceAttrs, true);
                }
            }
            return ceAttrs;
        }
        private static void GetAttrFromSiteProp(SPWeb web,string siteProp, CEAttres ceAttrs,bool isRootWeb)
        {
            if (web.AllProperties != null)
            {
                System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                List<ZSiteNodeModel> nodes = javaScriptSerializer.Deserialize<List<ZSiteNodeModel>>(siteProp);
                if (nodes == null)
                {
                    nodes = new List<ZSiteNodeModel>();
                }
                var currentNode = nodes.Where(p => p.id == web.Url).FirstOrDefault();
                if (currentNode == null)
                {
                    return;
                }
                foreach (DictionaryEntry keyValue in web.AllProperties)
                {
                    SitePropertyModel selectedProp = currentNode.siteProperties.Where(p => p.displayName == keyValue.Key.ToString()).FirstOrDefault();
                    if (selectedProp != null)
                    {
                        if (isRootWeb)
                        {
                            System.Diagnostics.Trace.WriteLine("root property:"+ selectedProp.displayName+",value:"+ keyValue.Value.ToString());
                            ceAttrs.AddAttribute(new CEAttribute("sc."+selectedProp.displayName, keyValue.Value.ToString(), CEAttributeType.XacmlString));
                        }
                        else
                        {
                            System.Diagnostics.Trace.WriteLine("subsite property:" + selectedProp.displayName + ",value:" + keyValue.Value.ToString());
                            ceAttrs.AddAttribute(new CEAttribute("ss." + selectedProp.displayName, keyValue.Value.ToString(), CEAttributeType.XacmlString));
                        }
                    }
                }
            }
        }
        private static void GetAttrFromHashTable(Hashtable hsProps, SitePropSetInfor propSetInfor, CEAttres ceAttrs)
        {
            //if (hsProps != null)
            //{
            //    if (propSetInfor.SelectAll)
            //    {
            //        foreach (DictionaryEntry keyValue in hsProps)
            //        {
            //            ceAttrs.AddAttribute(new CEAttribute(keyValue.Key.ToString(), keyValue.Value.ToString(), CEAttributeType.XacmlString));
            //        }
            //    }
            //    else if (!string.IsNullOrEmpty(propSetInfor.PropertyNames) && !string.IsNullOrEmpty(propSetInfor.MapedNames))
            //    {
            //        string[] propNames = propSetInfor.PropertyNames.Split(new string[] { ColumnSeparator }, StringSplitOptions.RemoveEmptyEntries);
            //        string[] mapedNames = propSetInfor.MapedNames.Split(new string[] { ColumnSeparator }, StringSplitOptions.RemoveEmptyEntries);
            //        int count = propNames.Length < mapedNames.Length ? propNames.Length : mapedNames.Length;
            //        for (int i = 0; i < count; i++)
            //        {
            //            foreach (string key in hsProps.Keys)
            //            {
            //                if (key.Equals(propNames[i], StringComparison.OrdinalIgnoreCase))
            //                {
            //                    ceAttrs.AddAttribute(new CEAttribute(mapedNames[i], hsProps[key].ToString(), CEAttributeType.XacmlString));
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public static bool CheckListItemCloudAZ(SPListItem item, SPUser user, string strAction, ref List<CEObligation> listObligation)
        {
            try
            {
                SPWeb web = item.Web;
                string srcName = web.Url + "/" + item.Url;
                if (EventReceiversModule.SupportedListTypes.Contains((int)item.ParentList.BaseTemplate))
                {
                    srcName = srcName.Substring(0, srcName.LastIndexOf("/")) + "/" + item.DisplayName; ;
                }

                //  Convert "http://" or "https://" to "sharepoint://"
                int indx = srcName.IndexOf("://");
                srcName = "sharepoint" + srcName.Substring(indx);

                //get selected column
                string strSelectedColumns = Global.GetListSetInfor(item.ParentList).SelectedColumns;
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                Dictionary<string, string> dicListColumn = javaScriptSerializer.Deserialize<Dictionary<string, string>>(strSelectedColumns);
                if (dicListColumn == null )
                {
                    dicListColumn = new Dictionary<string, string>();
                }
                CEAttres ceSrcAttrs = GetItemAttrs(item, dicListColumn);

                string strUserName = "";
                string strSid = "";
                CEAttres userAttrs = new CEAttres();
                GetSPUserAttrs(item.Web, user, ref strUserName, ref strSid, userAttrs);

                CERequest obReq = CloudAZQuery.CreateQueryReq(strAction, "", srcName, ceSrcAttrs, strSid, strUserName, userAttrs);
                PolicyResult emPolicyResult = PolicyResult.DontCare;
                QueryStatus emQueryRes = CloudAZQuery.Instance.QueryCloudAZPC(obReq, ref listObligation, ref emPolicyResult);
                if (emQueryRes == QueryStatus.S_OK)
                {
                    if (emPolicyResult == PolicyResult.Deny)
                    {
                        listObligation = null; // Ignore obligations when policy result is deny.
                    }
                    return true;
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            return false;
        }

        private static bool IsValueMetaData(string strValue, List<string> lStartWith)
        {
            if (lStartWith == null)
            {
                lStartWith = new List<string>();
                return false;
            }
            foreach (string sw in lStartWith)
            {
                if (strValue.Contains(sw + "|")) return true;
            }
            return false;
        }

        public static Dictionary<string, string> GetTagsFromObligations(List<CEObligation> listObligation, SPListItem item, SPFile itemFile)
        {
            Dictionary<string, string> dicTags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> fileTags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bool bGetFileTag = false;
            Dictionary<string, string> listOverwriteLevel = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (item == null || itemFile == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return dicTags;
            }

            foreach (CEObligation obligation in listObligation)
            {
                if (obligation.GetName().Equals(StrRmxObName, StringComparison.OrdinalIgnoreCase))
                {
                    CEAttres attrs = obligation.GetCEAttres();
                    string tagging = "";
                    string obName = "";
                    string obValue = "";
                    string mode = "";
                    string overwriteLevel = "";
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        CEAttribute ceAttr = attrs[i];
                        if (ceAttr.Name.Equals("tagging", StringComparison.OrdinalIgnoreCase))
                            tagging = ceAttr.Value;
                        else if (ceAttr.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
                            obName = ceAttr.Value.ToLower();
                        else if (ceAttr.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
                            obValue = ceAttr.Value;
                        else if (ceAttr.Name.Equals("mode", StringComparison.OrdinalIgnoreCase))
                            mode = ceAttr.Value;
                        if (ceAttr.Name.Equals("overwriteLevel", StringComparison.OrdinalIgnoreCase))
                            overwriteLevel = ceAttr.Value;
                    }

                    // Get tags from file
                    if (!bGetFileTag && tagging.Equals("file-tag", StringComparison.OrdinalIgnoreCase))
                    {
                        if (itemFile.Url.EndsWith(".pdf",StringComparison.OrdinalIgnoreCase))
                        {
                            string sourcePath = RmxModule.SaveDataToTemp(itemFile.OpenBinary(), itemFile.Url);
                            RmxModule.GetNormalFileTags(sourcePath, fileTags, TagSeparator);
                            if (!string.IsNullOrEmpty(sourcePath) && File.Exists(sourcePath))
                            {
                                try
                                {
                                    File.SetAttributes(sourcePath, FileAttributes.Normal);
                                    File.Delete(sourcePath);
                                }
                                catch (Exception exp)
                                {
                                    ULSLogger.LogWarning(exp.ToString() + ". When deleting [" + sourcePath + "]");
                                }
                            }
                            bGetFileTag = true;
                        }
                        else
                        {
                            System.Collections.Hashtable hProp = itemFile.Properties; 
                            if (hProp != null && hProp.Keys.Count > 0)
                            {
                                List<string> lMDataColValue = new List<string>();
                                List<string> lMDataColTitle = new List<string>();
                                //get all Metadata column name
                                foreach (SPField field in item.Fields)
                                {
                                    if (!field.TypeDisplayName.Equals("Managed Metadata", StringComparison.OrdinalIgnoreCase)) continue;
                                    if (field.Type != SPFieldType.Invalid) continue;
                                    if (!string.IsNullOrEmpty(field.Title)) lMDataColTitle.Add(field.Title);
                                    object objValue = item[field.InternalName];
                                    if (objValue != null)
                                    {
                                        string columnValue = field.GetFieldValueAsText(objValue);
                                        if (columnValue != null)
                                        {
                                            if (columnValue.Contains(";"))
                                                lMDataColValue.AddRange(columnValue.Split(';'));
                                            else
                                                lMDataColValue.Add(columnValue);
                                        }
                                    }
                                }
                                object strvalue = null;
                                foreach (string strkey in hProp.Keys)
                                {
                                    strvalue = hProp[strkey];
                                    if (strvalue == null || string.IsNullOrEmpty(strvalue.ToString()) ||
                                        string.IsNullOrWhiteSpace(strvalue.ToString())) continue;
                                    //Filter some single-value Managed MetaData
                                    if (lMDataColValue.Count != 0 && strkey.Length == 32 && IsValueMetaData(strvalue.ToString(), lMDataColValue)) continue;
                                    //Filter multi-value Managed MetaData
                                    if (lMDataColTitle.Count != 0 && lMDataColTitle.Contains(strkey)) continue;
                                    if (strkey.Equals("vti_title", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (fileTags.ContainsKey("title"))
                                            fileTags["title"] += (TagSeparator + strvalue.ToString()); // mutiple value, use separator to separate.
                                        else
                                            fileTags.Add("title", strvalue.ToString());
                                    }
                                    else if (!strkey.StartsWith("vti_", StringComparison.OrdinalIgnoreCase) && !strkey.StartsWith("_")
                                        && !strkey.StartsWith("Keywords", StringComparison.OrdinalIgnoreCase) &&
                                        !strkey.StartsWith("ContentType", StringComparison.OrdinalIgnoreCase) &&
                                        !strkey.StartsWith("TaxCatchAll", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (fileTags.ContainsKey(strkey))
                                            fileTags[strkey] += (TagSeparator + strvalue.ToString()); // mutiple value, use separator to separate.
                                        else
                                            fileTags.Add(strkey, strvalue.ToString());
                                    }
                                }
                            }
                            if (fileTags.Count > 0) bGetFileTag = true;
                        }
                    }

                    // only file-tag , we don't need figure out name;
                    if (!tagging.Equals("file-tag", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(obName)) continue;

                    string[] arrName = obName.Split(new string[] { TagSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> listName = null;
                    if (arrName != null) listName = new List<string>(arrName);
                    else listName = new List<string>();
                    List<string> listValue = null;

                    // Different modes (user-defined, specific-column, file-tag)
                    if (tagging.Equals("user-defined", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(obValue))
                        {
                            string[] values = obValue.Split(new string[] { TagSeparator }, StringSplitOptions.RemoveEmptyEntries);
                            listValue = new List<string>(values);
                        }
                        else return dicTags;
                    }
                    else if (tagging.Equals("specific-column", StringComparison.OrdinalIgnoreCase))
                    {
                        listValue = GetItemColumnValues(item, listName);
                    }
                    else if (tagging.Equals("file-tag", StringComparison.OrdinalIgnoreCase))
                    {
                        listValue = new List<string>();
                        if (!string.IsNullOrEmpty(obName))
                        {
                            foreach (string name in listName)
                            {
                                if (fileTags.ContainsKey(name))
                                    listValue.Add(fileTags[name]);
                                else
                                    listValue.Add("");
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, string> keyValue in fileTags)
                            {
                                listName.Add(keyValue.Key);
                                listValue.Add(keyValue.Value);
                            }
                        }
                    }
                    // Merge Obligations Tags.
                    for (int i = 0; i < listName.Count; i++)
                    {
                        string name = listName[i];
                        if (i >= listValue.Count) break;
                        string value = listValue[i];
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                        {
                            MergeObligationsTags(dicTags, name, value, mode, overwriteLevel, listOverwriteLevel);
                        }
                    }
                }
            }
            return dicTags;
        }

        public static void MergeObligationsTags(Dictionary<string, string> dicTags, string name, string value, string mode, string overwriteLevel, Dictionary<string, string> listOverwriteLevel)
        {
            if (dicTags.ContainsKey(name))
            {
                if (mode.Equals("append", StringComparison.OrdinalIgnoreCase))
                {
                    if (!listOverwriteLevel.ContainsKey(name))
                        dicTags[name] = dicTags[name] + TagSeparator + value;
                }
                else if (mode.Equals("overwrite", StringComparison.OrdinalIgnoreCase))
                {
                    bool bOver = false;
                    if (!listOverwriteLevel.ContainsKey(name)) bOver = true;
                    else
                    {
                        string curLevel = listOverwriteLevel[name];
                        if (curLevel.Equals("Low", StringComparison.OrdinalIgnoreCase))
                        {
                            if (overwriteLevel.Equals("Middle", StringComparison.OrdinalIgnoreCase)
                                || overwriteLevel.Equals("High", StringComparison.OrdinalIgnoreCase))
                                bOver = true;
                        }
                        else if (curLevel.Equals("Middle", StringComparison.OrdinalIgnoreCase)
                            && overwriteLevel.Equals("High", StringComparison.OrdinalIgnoreCase))
                        {
                            bOver = true;
                        }
                    }

                    if (bOver)
                    {
                        listOverwriteLevel[name] = overwriteLevel;
                        dicTags[name] = value;
                    }
                }
            }
            else
            {
                dicTags.Add(name, value);
                if (mode.Equals("overwrite", StringComparison.OrdinalIgnoreCase))
                {
                    listOverwriteLevel[name] = overwriteLevel;
                }
            }
        }

        public static void DoRMXEnforcer(SPItemEventProperties properties, SPEventReceiverType type)
        {
            try
            {
                if (type == SPEventReceiverType.ItemAdded || type == SPEventReceiverType.ItemUpdated)
                {
                    if (properties.AfterProperties != null && properties.AfterProperties["vti_filesize"] != null && properties.AfterProperties["vti_filesize"].ToString() == "0")
                        return; // Do nothing when file size is 0kb.

                    if (type == SPEventReceiverType.ItemUpdated && !properties.AfterUrl.Equals(properties.BeforeUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        return; // Case: Rename file in library, do it in ItemFileMoved.
                    }

                    SPListItem listItem = properties.ListItem;
                    if (listItem != null && listItem.File != null && !listItem.File.Name.EndsWith(".nxl", StringComparison.OrdinalIgnoreCase))
                    {
                        string fileUrl = properties.Web.Url.TrimEnd(TrimFlag) + "/" + properties.AfterUrl.TrimStart(TrimFlag);
                        DoEventModeRMX(properties, properties.Web.Url, properties.ListId.ToString(), properties.ListItemId, fileUrl, properties.CurrentUserId, type);
                    }
                }
                else if (type == SPEventReceiverType.ItemAttachmentAdded)
                {
                    string fileUrl = properties.Web.Url.TrimEnd(TrimFlag) + "/" + properties.AfterUrl.TrimStart(TrimFlag);
                    DoEventModeRMX(properties, properties.Web.Url, properties.ListId.ToString(), properties.ListItemId, fileUrl, properties.CurrentUserId, type);
                }
                else if (type == SPEventReceiverType.ItemFileMoved)
                {
                    if (properties.AfterProperties != null && properties.AfterProperties["vti_filesize"] != null && properties.AfterProperties["vti_filesize"].ToString() == "0")
                        return; // Do nothing when file size is 0kb.
                    string fileUrl = properties.Web.Url.TrimEnd(TrimFlag) + "/" + properties.AfterUrl.TrimStart(TrimFlag);
                    SPListItem item = properties.Web.GetListItem(fileUrl);
                    if (item != null) DoEventModeRMX(properties, properties.Web.Url, item.ParentList.ID.ToString(), item.ID, fileUrl, properties.CurrentUserId, type);
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        public static void DoEventModeRMX(SPItemEventProperties properties, string webUrl, 
            string listGuid, int itemId, string fileUrl, int userId, SPEventReceiverType type)
        {
            SPList list = null;
            ListSetInfor listSetInfor = null;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(webUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            bool oldSafe = web.AllowUnsafeUpdates;
                            web.AllowUnsafeUpdates = true;
                            try
                            {
                                SPUser user = web.AllUsers.GetByID(userId);
                                list = web.Lists[new Guid(listGuid)];
                                listSetInfor = GetListSetInfor(list);
                                int templateId = (int)list.BaseTemplate;
                                SPListItem item = list.GetItemById(itemId);
                                int oldCount = item.Versions.Count;

                                //do delay delete for onedrive sync bug:55472 - the encrypted file have mutiple versions after sync lib to onedrive and copy file to it
                                bool bDelayProcess = false;
                                if (properties.AfterProperties["vti_syncupdatehiddenversion"] != null) bDelayProcess = true;
                                string failedInfo = "";
                                if (DoItemRMX(web, item, fileUrl, listSetInfor, user,ref failedInfo,bDelayProcess, true)  == 1)
                                {
                                    if (EventReceiversModule.SupportedListTypes.Contains(templateId))
                                    {
                                        RemoveListItemNewVersions(web, web.Url + "/" + item.Url, oldCount);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                ULSLogger.LogError(ex.ToString());
                            }
                            web.AllowUnsafeUpdates = oldSafe;
                        }
                    }
                });
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }
        public static void DoBatchModeRMX(string webUrl, string listGuid, List<Guid> listItemGuid)
        {
            if (string.IsNullOrEmpty(webUrl) || string.IsNullOrEmpty(listGuid))
            {
                ULSLogger.LogError("Parameter Null.");
                return;
            }
            SPList list = null;
            ListSetInfor listSetInfor = null;
            List<BatchModeFailedModel> listFailedItem = new List<BatchModeFailedModel>();
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(webUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            bool oldSafe = web.AllowUnsafeUpdates;
                            web.AllowUnsafeUpdates = true;
                            ItemHandler itemHandle = new ItemHandler();
                            itemHandle.DisableEventFiring();
                            try
                            {
                                SPUser user = web.CurrentUser;
                                list = web.Lists[new Guid(listGuid)];
                                listSetInfor = GetListSetInfor(list);
                                int templateId = (int)list.BaseTemplate;
                                SPListItemCollection items = list.Items;
                                foreach (Guid guid in listItemGuid)
                                {
                                    try
                                    {
                                        SPListItem item = items[guid];
                                        if (EventReceiversModule.SupportedLibraryTypes.Contains(templateId))
                                        {
                                            if (!item.File.Url.EndsWith(".nxl", StringComparison.OrdinalIgnoreCase) && CheckFileInBatch(web, item.File))
                                            {
                                                listSetInfor.RunningTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                Global.SetListSetInfor(list, listSetInfor);
                                                string failedInfo = "";
                                                bool bNeedRMXButFailed = DoItemRMX(web, item, web.Url + "/" + item.Url, listSetInfor, user, ref failedInfo) == -1 ? true : false;
                                                if (bNeedRMXButFailed)
                                                {
                                                    BatchModeFailedModel failedItem = new BatchModeFailedModel(item.Url, failedInfo);
                                                    listFailedItem.Add(failedItem);
                                                }
                                            }
                                        }
                                        else if (EventReceiversModule.SupportedListTypes.Contains(templateId))
                                        {
                                            int oldCount = item.Versions.Count;
                                            SPAttachmentCollection attachments = item.Attachments;
                                            foreach (string url in attachments)
                                            {
                                                string fileUrl = attachments.UrlPrefix + url;
                                                if (!fileUrl.EndsWith(".nxl", StringComparison.OrdinalIgnoreCase) && CheckFileInBatch(web, fileUrl))
                                                {
                                                    listSetInfor.RunningTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    Global.SetListSetInfor(list, listSetInfor);
                                                    string failedInfo = "";
                                                    bool bNeedRMXButFailed = DoItemRMX(web, item, fileUrl, listSetInfor, user, ref failedInfo) == -1 ? true : false;
                                                    if (bNeedRMXButFailed)
                                                    {
                                                        BatchModeFailedModel failedItem = new BatchModeFailedModel(item.DisplayName + "/" + url, failedInfo);
                                                        listFailedItem.Add(failedItem);
                                                    }
                                                }
                                            }
                                            RemoveListItemNewVersions(web, web.Url + "/" + item.Url, oldCount);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ULSLogger.LogError(ex.ToString());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
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
                ULSLogger.LogError(exp.ToString());
            }
            finally
            {
                if (list != null && listSetInfor != null)
                {
                    if(listFailedItem.Count > 0)
                    {
                        listSetInfor.BatchSuccess = false;
                        listSetInfor.FailedItems = new List<BatchModeFailedModel>(listFailedItem);
                        listSetInfor.BatchLog = "Batch mode process was finished at: " + DateTime.Now.ToString() + ".";
                    }
                    else
                    {
                        listSetInfor.BatchSuccess = true;
                        if (listSetInfor.FailedItems.Count > 0) listSetInfor.FailedItems.Clear();
                        listSetInfor.BatchLog = "Batch mode process was successful at: " + DateTime.Now.ToString() + ".";
                    }
                    listSetInfor.BatchStatus = StrDisabled;
                    SetListSetInfor(list, listSetInfor);
                }
            }
        }

        // Compare last modified time with "NXL" file and devide if item need to do RMX in Batch Mode.  
        public static bool CheckFileInBatch(SPWeb web, SPFile file)
        {
            bool bRet = true;
            try
            {
                SPFile nxlfile = web.GetFile(file.Url + ".nxl");
                if (nxlfile != null && nxlfile.Exists && DateTime.Compare(nxlfile.TimeLastModified, file.TimeLastModified) > 0)
                {
                    bRet = false;
                }
            }
            catch
            {
            }
            return bRet;
        }

        // Compare last modified time with "NXL" file and devide if item need to do RMX in Batch Mode.  
        public static bool CheckFileInBatch(SPWeb web, string fileUrl)
        {
            bool bRet = true;
            try
            {
                SPFile file = web.GetFile(fileUrl);
                bRet = CheckFileInBatch(web, file);
            }
            catch
            { }
            return bRet;
        }

        //use to pass OneNoteItem, Folder and DocumentSet
        public static bool IsFolderType(SPListItem spItem)
        {
            if (spItem == null) return true;
            if (spItem.FileSystemObjectType == SPFileSystemObjectType.Folder) return true;
            else return false;
        }
        //use to pass OneNoteItem
        public static bool IsOneNoteItem(SPListItem spItem)
        {
            if (spItem == null) return true; //true can stop invalid encryption
            bool bIsOneNote = false;
            if (spItem.FileSystemObjectType == SPFileSystemObjectType.Folder) //OneNote Item is Folder, Item.File is null 
            {
                string strHtmlType = null;
                object objHtmType = spItem["HTML_x0020_File_x0020_Type"];
                if (objHtmType != null) strHtmlType = objHtmType.ToString();
                if (!string.IsNullOrEmpty(strHtmlType) &&
                    strHtmlType.Equals("OneNote.Notebook", StringComparison.CurrentCultureIgnoreCase))
                {
                    bIsOneNote = true;
                }
            }
            return bIsOneNote;
        }
        //use to pass special format file by name
        public static bool IsIgnoredFileName(SPFile spFile)
        {
            if (spFile == null) return true; //true can stop invalid encryption
            if (spFile.InDocumentLibrary)
            {
                if (spFile.Item.FileSystemObjectType == SPFileSystemObjectType.File)
                {
                    return CheckIsIgnoredFileName(spFile.Name);
                }
            }
            else
            {
                return CheckIsIgnoredFileName(spFile.Name);
            }
            return false;
        }
        public static bool CheckIsIgnoredFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return true;
            string pattern = @"(^(~\$)?New Microsoft Excel Worksheet(\s\(\d\))?(\s-\scopy)?.xlsx)|(^Book(\d)?.xlsx)|(^(~\$)?New Microsoft Word Document(\s\(\d\))?(\s-\scopy)?.docx)|(^Document(\d)?.docx)|(^(~\$)?New Microsoft PowerPoint Presentation(\s\(\d\))?(\s-\scopy)?.pptx)|(^Presentation(\d)?.pptx)|(.+\.tmp)|(.+\.nxl)|(.+\.url)|(.+\.one)|(^Open Notebook\.onetoc(\d)?)";
            try
            {
                if (Regex.Matches(fileName, pattern, RegexOptions.IgnoreCase).Count > 0) return true;
            }
            catch (Exception ex)
            {
                ULSLogger.LogError(ex.ToString());
            }
            return false;
        }
        /*
         * policy based.
         *  return 0 - do nothing;
         *  return -1 - need to be protected but failed;
         *  return 1 - encrypt successfully 
         */
        public static int DoItemRMX(SPWeb web, SPListItem item, string fileUrl,
         ListSetInfor listSetInfor, SPUser user,ref string failedInfo, bool bDelayProcess = false,bool bEventMode = false)
        {
            try
            {
                if (web == null || item == null || string.IsNullOrEmpty(fileUrl) || fileUrl.EndsWith(".nxl", StringComparison.OrdinalIgnoreCase))
                {
                    return 0;
                }

                if (IsFolderType(item)) return 0;

                SPFile itemFile = web.GetFile(fileUrl);
                if (itemFile == null) return 0;

                if (IsIgnoredFileName(itemFile))
                {
                    failedInfo = "Ignore,unsupported Type";
                    return -1;
                }
                List<CEObligation> listObligation = new List<CEObligation>();
                bool ret = CheckListItemCloudAZ(item, user, StrRmxAction, ref listObligation);
                if (!ret)
                {
                    failedInfo = "Failed,error occurred when query policy";
                    return -1;
                }
                if (listObligation.Count < 1)
                {
                    failedInfo = "Ignore,no match policy";
                    return -1;
                }

                if (itemFile.CheckOutType != SPFile.SPCheckOutType.None)
                {
                    ULSLogger.LogWarning("Protect file [" + fileUrl + "] failed because the file is checkouted status.");
                    failedInfo = "Ignore,Checked out file";
                    return -1;
                }
                if (!string.IsNullOrEmpty(itemFile.LockId))
                {
                    if (bEventMode) bDelayProcess = true;   // same thing, delay to process
                    else
                    {
                        ULSLogger.LogWarning("Protect file [" + fileUrl + "] failed because the file is locked status.");
                        failedInfo = "Ignore, file is locked";
                        return -1;
                    }
                }
                

                Dictionary<string, string> dicTags = Global.GetTagsFromObligations(listObligation, item, itemFile);
                if (bDelayProcess)
                {
                    // one driver case or file was locked at event mode, process it with background thread
                    DelayedItemMgr.AddedDelayedItem(item.Web.Url, item.ParentList.ID,
                        item.ID, listSetInfor, fileUrl, dicTags, bDelayProcess);
                    return 0;
                }
                else
                {
                    bool bEncrypt = RmxModule.EncryptItemVerstions(item, itemFile, fileUrl, dicTags, listSetInfor);
                    if (!bEncrypt)
                    {
                        ULSLogger.LogWarning("Encrypted [" + fileUrl + "] failed.");
                        failedInfo = "Failed,error occurred when do encrypt";
                        return -1;
                    }
                    return 1;
                }
            }
            catch (Exception e)
            {
                ULSLogger.LogError(e.ToString());
                failedInfo = "Failed,error occurred";
                return -1;
            }
        }
        // for batch mode, the single task process funtion
        private static void DoPartItemsRMX(string webUrl, string listGuid, List<Guid> listItemGuid, List<BatchModeFailedModel> listFailedItem, int nTasks, int iCurTask)
        {
            if (string.IsNullOrEmpty(webUrl) || string.IsNullOrEmpty(listGuid) || listItemGuid == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return;
            }
            if (listFailedItem == null) listFailedItem = new List<BatchModeFailedModel>();
            SPList list = null;
            ListSetInfor listSetInfor = null;
            Guid[] arrItemGuid = listItemGuid.ToArray();
            int count = listItemGuid.Count;
            int ntimes = count / nTasks + 1;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(webUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            bool oldSafe = web.AllowUnsafeUpdates;
                            web.AllowUnsafeUpdates = true;
                            try
                            {
                                SPUser user = web.CurrentUser;
                                list = web.Lists[new Guid(listGuid)];
                                listSetInfor = GetListSetInfor(list);
                                int templateId = (int)list.BaseTemplate;
                                SPListItemCollection items = list.Items;
                                try
                                {
                                    int threshold = iCurTask * ntimes + ntimes;
                                    int edge = threshold < count ? threshold : count;
                                    for (int i = iCurTask * ntimes; i < edge; i++)
                                    {
                                        try
                                        {
                                            SPListItem item = items[arrItemGuid[i]];
                                            if (EventReceiversModule.SupportedLibraryTypes.Contains(templateId))
                                            {
                                                if (!item.File.Url.EndsWith(".nxl", StringComparison.OrdinalIgnoreCase) && CheckFileInBatch(web, item.File))
                                                {
                                                    listSetInfor.RunningTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    Global.SetListSetInfor(list, listSetInfor);
                                                    string failedInfo = "";
                                                    bool bNeedRMXButFailed = DoItemRMX(web, item, web.Url + "/" + item.Url, listSetInfor, user, ref failedInfo) == -1 ? true : false;
                                                    if (bNeedRMXButFailed)
                                                    {
                                                        BatchModeFailedModel failedItem = new BatchModeFailedModel(item.Url, failedInfo);
                                                        listFailedItem.Add(failedItem);
                                                    }
                                                }
                                            }
                                            else if (EventReceiversModule.SupportedListTypes.Contains(templateId))
                                            {
                                                int oldCount = item.Versions.Count;
                                                SPAttachmentCollection attachments = item.Attachments;
                                                foreach (string url in attachments)
                                                {
                                                    string fileUrl = attachments.UrlPrefix + url;
                                                    if (!fileUrl.EndsWith(".nxl", StringComparison.OrdinalIgnoreCase) && CheckFileInBatch(web, item.File))
                                                    {
                                                        listSetInfor.RunningTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                        Global.SetListSetInfor(list, listSetInfor);
                                                        string failedInfo = "";
                                                        bool bNeedRMXButFailed = DoItemRMX(web, item, fileUrl, listSetInfor, user, ref failedInfo) == -1 ? true : false;
                                                        if (bNeedRMXButFailed)
                                                        {
                                                            BatchModeFailedModel failedItem = new BatchModeFailedModel(item.DisplayName + "/" + url, failedInfo);
                                                            listFailedItem.Add(failedItem);
                                                        }
                                                    }
                                                }
                                                RemoveListItemNewVersions(web, web.Url + "/" + item.Url, oldCount);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            ULSLogger.LogError(e.ToString());
                                        }
                                    }
                                }
                                catch (Exception ee)
                                {
                                    ULSLogger.LogError(ee.ToString());
                                }
                            }
                            catch (Exception ex)
                            {
                                ULSLogger.LogError( ex.ToString());
                            }
                            web.AllowUnsafeUpdates = oldSafe;
                        }
                    }
                });
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }
        // for batch mode, when files is too much, use task to imprvoe performance
        public static void TaskDoBatchModeRMX(string webUrl, string listGuid, List<Guid> listItemGuid)
        {
            if (string.IsNullOrEmpty(webUrl) || string.IsNullOrEmpty(listGuid) || listItemGuid == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return;
            }
            int nTasks = 3;
            List<BatchModeFailedModel>[] listFailedItemArray = new List<BatchModeFailedModel>[nTasks];
            List<BatchModeFailedModel> listFailedItem = null;
            Task[] tasks = new Task[nTasks];
            for (int itask = 0; itask < nTasks; itask++)
            {
                int icurtask = itask;
                listFailedItemArray[icurtask] = new List<BatchModeFailedModel>();
                tasks[icurtask] = Task.Run(() => { DoPartItemsRMX(webUrl, listGuid, listItemGuid, listFailedItemArray[icurtask], nTasks, icurtask); });
            }
            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.Flatten().InnerExceptions)
                    ULSLogger.LogError(ex.ToString());
            }
            finally
            {
                SPList list = null;
                ListSetInfor listSetInfor = null;
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(webUrl))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                bool oldSafe = web.AllowUnsafeUpdates;
                                web.AllowUnsafeUpdates = true;
                                try
                                {
                                    list = web.Lists[new Guid(listGuid)];
                                    listSetInfor = GetListSetInfor(list);
                                }
                                catch (Exception ex)
                                {
                                    ULSLogger.LogError(ex.ToString());
                                }
                                web.AllowUnsafeUpdates = oldSafe;
                            }
                        }
                    });
                }
                catch (Exception exp)
                {
                    ULSLogger.LogError(exp.ToString());
                }
                if (list != null && listSetInfor != null)
                {
                    for(int i=0;i< nTasks;i++)
                    {
                        if(listFailedItemArray[i].Count>0)
                        {
                            if (listFailedItem == null) listFailedItem = new List<BatchModeFailedModel>();
                            listFailedItem.AddRange(listFailedItemArray[i]);
                        }
                    }
                    if (listFailedItem != null && listFailedItem.Count > 0)
                    {
                        listSetInfor.BatchSuccess = false;
                        listSetInfor.FailedItems = new List<BatchModeFailedModel>(listFailedItem);
                        listSetInfor.BatchLog = "Batch mode process was finished at: " + DateTime.Now.ToString() + ".";
                    }
                    else
                    {
                        listSetInfor.BatchSuccess = true;
                        if(listSetInfor.FailedItems.Count > 0) listSetInfor.FailedItems.Clear();
                        listSetInfor.BatchLog = "Batch mode process was successful at: " + DateTime.Now.ToString() + ".";
                    }
                    listSetInfor.BatchStatus = StrDisabled;
                    SetListSetInfor(list, listSetInfor);
                }
            }
        }
        // for batch mode, get items guid list first
        private static int GetItemGuidList(string webUrl, string listGuid, List<Guid> listItemGuid)
        {
		    if (string.IsNullOrEmpty(webUrl) || string.IsNullOrEmpty(listGuid))
            {
                ULSLogger.LogError("Parameter Null.");
                return -1;
            }
            if (listItemGuid == null) listItemGuid = new List<Guid>();
            SPList list = null;
            int total = -1;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(webUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            bool oldSafe = web.AllowUnsafeUpdates;
                            web.AllowUnsafeUpdates = true;
                            try
                            {
                                list = web.Lists[new Guid(listGuid)];
                                SPListItemCollection items = list.Items;
                                total = items.Count;
                                for (int i = total - 1; i >= 0; i--)
                                    listItemGuid.Add(items[i].UniqueId);
                            }
                            catch (Exception ex)
                            {
                                ULSLogger.LogError(ex.ToString());
                                total = -1;
                            }
                            web.AllowUnsafeUpdates = oldSafe;
                        }
                    }
                });
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
                total = -1;
            }
            return total;
        }
        // for batch mode, let's use muitlple thread to imprvoe performance
        public static void DoBatchModeRMXEX(string webUrl, string listGuid)
        {
            if (string.IsNullOrEmpty(webUrl) || string.IsNullOrEmpty(listGuid))
            {
                ULSLogger.LogError("Parameter Null.");
                return;
            }
            List<Guid> lItemGuid = new List<Guid>();
            int itemCount = GetItemGuidList(webUrl, listGuid, lItemGuid);
            if (itemCount == -1) return;
            if (lItemGuid.Count == 0) return;
            if (itemCount > 1000) TaskDoBatchModeRMX(webUrl, listGuid, lItemGuid);
            else DoBatchModeRMX(webUrl, listGuid, lItemGuid);
        }

        //public static string CheckBackupPath(SPWeb curWeb, string strBackupPath)
        //{
        //    string finalPath = null;
        //    if (string.IsNullOrEmpty(strBackupPath) || strBackupPath.Length > (LimitBackupFileUrl - LimitBackupFileName - 1))
        //    {
        //        return finalPath;
        //    }
        //    try
        //    {
        //        SPSecurity.RunWithElevatedPrivileges(delegate ()
        //        {
        //            using (SPSite site = new SPSite(strBackupPath))
        //            {
        //                using (SPWeb web = site.OpenWeb())
        //                {
        //                    if (web.ServerRelativeUrl.EndsWith(curWeb.ServerRelativeUrl, StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        SPFolder folder = web.GetFolder(strBackupPath);
        //                        if (folder != null && folder.Exists)
        //                        {
        //                            finalPath = curWeb.Url + "/" + folder.Url;
        //                        }
        //                    }
        //                }
        //            }
        //        });
        //    }
        //    catch (Exception exp)
        //    {
        //        ULSLogger.LogError(exp.ToString());
        //    }
        //    return finalPath;
        //}


        public static string JsonSerializeObject(object obj)
        {
            string strResult = "";
            try
            {
                var set = new DataContractJsonSerializerSettings();
                set.UseSimpleDictionaryFormat = true;
                DataContractJsonSerializer jsonSeria = new DataContractJsonSerializer(obj.GetType(), set);
                MemoryStream msObj = new MemoryStream();
                jsonSeria.WriteObject(msObj, obj);
                msObj.Position = 0;

                StreamReader sr = new StreamReader(msObj);
                strResult = sr.ReadToEnd();

                return strResult;
            }
            catch (System.Exception){
            }

            return strResult;
        }

        public static Dictionary<string,string[]> GetUserAttributeFromProfile(SPUser user, SPSite site)
        {
            Dictionary<string, string[]> dicAttrs = new Dictionary<string, string[]>();

            try
            {
                SPServiceContext context = SPServiceContext.GetContext(site);
                UserProfileManager upm = new UserProfileManager(context);

                UserProfile profile = upm.GetUserProfile(user.LoginName);
                if (profile != null)
                {
                    foreach (var varProp in profile.Properties)
                    {
                        if (varProp.Name != null /*&& (!varProp.Name.StartsWith("SPS-"))*/)
                        {
                            object objValue = profile[varProp.Name];
                            string strValue = "";
                            if (objValue != null)
                            {
                                try{
                                    strValue = objValue.ToString();
                                }
                                catch (System.Exception){ }
                            }

                            if (!string.IsNullOrEmpty(strValue))
                            {
                                string[] arrayValue = new string[1];
                                arrayValue[0] = strValue;
                                dicAttrs.Add(varProp.Name, arrayValue);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ULSLogger.LogError("Exception on get user profile:" + ex.ToString());
            }

            return dicAttrs;
        }

        public static void GetSPUserAttrs(SPWeb web, SPUser user, ref string userName, ref string userSid, CEAttres userAttrs)
        {
            if (web==null || user == null || userAttrs == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return;
            }

            //get actual user
            if (user.LoginName.Equals(@"SharePoint\system", StringComparison.OrdinalIgnoreCase))
            {//if user is system account, we get the user from thread

                /*  try
                  {
                      string strName = System.Security.Principal.WindowsIdentity.GetCurrent(false).Name;
                      web.AllowUnsafeUpdates = true;
                      user = web.EnsureUser(strName);
                  }
                  catch (System.Exception ex)
                  {
                      ULSLogger.LogError("Exception on LoginSkyDrmByTrustApp::EnsureUser:" + ex.ToString());
                  }
                  *///we can't use WindowsIdentity.GetCurrent(false).Name, because may be more than two user is "system account"
            }

            //get sid, email address
            userName = user.LoginName;
            if (!string.IsNullOrEmpty(userName) && userName.Contains("|"))
            {
                userName = userName.Substring(userName.IndexOf("|") + 1);
                userName = userName.Replace("|", ":");
            }
            if (user.UserId != null && !string.IsNullOrEmpty(user.UserId.NameId))
            {
                userSid = user.UserId.NameId;
            }
            else
            {
                // ADFS and FBA user
                userSid = userName;
            }
            userAttrs.AddAttribute(new CEAttribute("emailaddress", user.Email, CEAttributeType.XacmlString));
            userAttrs.AddAttribute(new CEAttribute("username", user.Name, CEAttributeType.XacmlString));

            //get user attribute from UserProfile
            if (!user.LoginName.Equals(@"SharePoint\system", StringComparison.OrdinalIgnoreCase))
            {
                Dictionary<string, string[]> dicUserAttr = Global.GetUserAttributeFromProfile(user, web.Site);
                foreach (var varUserAttr in dicUserAttr)
                {
                    foreach (string strValue in varUserAttr.Value)
                    {
                        userAttrs.AddAttribute(new CEAttribute(varUserAttr.Key, strValue, CEAttributeType.XacmlString));
                    }
                }
            }
        }

        public static void RemoveListItemNewVersions(SPWeb web, string url, int oldCount)
        {
            if (web == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return;
            }
            SPListItem item = web.GetListItem(url);
            SPListItemVersionCollection itemVersions = item.Versions;
            int count = itemVersions.Count;
            if (count > oldCount + 1)
            {
                for (int i = count - oldCount - 1; i > 0; i--)
                {
                    SPListItemVersion verItem = itemVersions[i];
                    verItem.Delete();
                }
            }
        }

        public static void RemoveBatchModeLog(SPList list)
        {
            try
            {
                ListSetInfor listSetInfor = GetListSetInfor(list);
                if (!string.IsNullOrEmpty(listSetInfor.BatchLog) || !string.IsNullOrEmpty(listSetInfor.RunningTime) || !listSetInfor.BatchSuccess 
                    || listSetInfor.BatchStatus == Global.StrEnabled || listSetInfor.FailedItems.Count > 0)
                {
                    listSetInfor.BatchLog = "";
                    listSetInfor.RunningTime = "";
                    listSetInfor.BatchSuccess = true;
                    listSetInfor.BatchStatus = Global.StrDisabled;
                    listSetInfor.FailedItems.Clear();
                    SetListSetInfor(list, listSetInfor);
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogError(ex.ToString());
            }
        }
    }
}