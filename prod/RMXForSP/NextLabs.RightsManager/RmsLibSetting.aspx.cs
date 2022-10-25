using System;
using System.Threading;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Linq;

namespace NextLabs.RightsManager
{
    public class RmsLibSetting : UnsecuredLayoutsPageBase
    {
        // Batch mode
        public const string NotRunning = "Status: Not Running";
        public const string InProgress = "Status: In Progress";
        public const string RunningBatchMode = "Batch mode is performing rights protection for all items.";
        public const string FailedBatchMode = "Batch mode process was failed.";
        //public const string BakupPathError = "The backup path is don't supported, please input the path with same site and try to set again.";
        //public const string BakupPathLongError = "The backup path is too long, please try to set again.";
        
          
        protected Button BtnOK;
        protected EncodedLiteral CAStatus;
        protected EncodedLiteral BatchLogEX;
        protected HyperLink FailedItemsLink;
        protected HtmlInputHidden scheduleDiv;
      
        protected SPList List;
        protected HtmlInputHidden columnJson;
        protected HtmlInputHidden hiddenDeleteFileCheck;
        protected HtmlInputHidden hiddenVersionsRMSCheckBox;
        protected HtmlInputHidden hiddenCAStatusCheckBox;
        protected HtmlInputHidden bList;



        public RmsLibSetting()
        {
            CAStatus = new EncodedLiteral();
            BatchLogEX = new EncodedLiteral();
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                 base.OnLoad(e);
                // Validate the Page Request to avoid any malicious posts
                if (Request.HttpMethod == "POST")
                    SPUtility.ValidateFormDigest();
                if (!Page.IsPostBack)
                {
                    SPUser user = this.Web.CurrentUser;
                    String listGuid = Request.QueryString["List"];
                    SPList list = this.Web.Lists[new Guid(listGuid)];

                    ListSetInfor listSetInfor = Global.GetListSetInfor(list);
                    bool bRun = CheckRunning(listSetInfor);

                    // Fix bug 56998, failed to do batch mode with thread abort or timeout.
                    if (!bRun && listSetInfor.BatchLog.Equals(RunningBatchMode) && listSetInfor.BatchStatus.Equals(Global.StrEnabled))
                    {
                        listSetInfor.BatchStatus = Global.StrDisabled;
                        listSetInfor.BatchLog = FailedBatchMode;
                        Global.SetListSetInfor(list, listSetInfor);
                    }
                    bool versionEnabled = listSetInfor.VersionsStatus.Equals(Global.StrEnabled);
                    bool deleteFileEnabled = listSetInfor.DeleteStatus.Equals(Global.StrEnabled);
                    hiddenDeleteFileCheck.Value = deleteFileEnabled.ToString().ToLower();
                    if (deleteFileEnabled)
                    {
                        hiddenVersionsRMSCheckBox.Disabled = true;
                        hiddenVersionsRMSCheckBox.Value = deleteFileEnabled.ToString().ToLower();
                      
                    }
                    else
                    {
                        hiddenVersionsRMSCheckBox.Disabled = false;
                        hiddenVersionsRMSCheckBox.Value = versionEnabled.ToString().ToLower();
                    }
                    hiddenCAStatusCheckBox.Value = "false";
                    //BackupFileCheck.Checked = listSetInfor.BackupStatus.Equals(Global.StrEnabled);
                    //BackupFilePath.Visible = listSetInfor.BackupStatus.Equals(Global.StrEnabled);
                    //BackupFilePath.Text = listSetInfor.BackupPath;
                    BatchLogEX.Text = listSetInfor.BatchLog;
                    if (listSetInfor.BatchSuccess)
                    {
                        FailedItemsLink.NavigateUrl = "";
                        FailedItemsLink.Visible = false;
                    }
                    else
                    {
                        FailedItemsLink.NavigateUrl = string.Format("{0}/_layouts/15/NextLabs.RightsManager/FailedItemsList.aspx?List={1}", this.Web.Url, listGuid);
                        FailedItemsLink.Visible = true;
                    }
                    bList.Value = "false";
                    if (EventReceiversModule.SupportedListTypes.Contains((int)list.BaseTemplate))
                    {
                        hiddenVersionsRMSCheckBox.Disabled = true;
                        hiddenVersionsRMSCheckBox.Value = "false";
                        bList.Value = "true";
                    }
                    if (!user.IsSiteAdmin || bRun)
                    {
                        hiddenCAStatusCheckBox.Disabled = true;
                        hiddenDeleteFileCheck.Disabled = true;
                        hiddenVersionsRMSCheckBox.Disabled = true;
                        //BackupFileCheck.Enabled = false;
                        //BackupFilePath.Enabled = false;
                        //BtnOK.Enabled = false;
                    }

                    scheduleDiv.Value = listSetInfor.ScheduleList;
                    columnJson.Value = InitColumn(list, listSetInfor.SelectedColumns);
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }
        private string InitColumn(SPList list,string selectedColumns)
        {
            System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string,string> listColumn = javaScriptSerializer.Deserialize<Dictionary<string,string>>(selectedColumns);
            if (listColumn == null)
            {
                listColumn = new Dictionary<string, string>();
            }
            var result = "";
            List<ZNodeModel> znodeList = new List<ZNodeModel>();
            try
            {
                var rootNode = new ZNodeModel();
                rootNode.id = list.ID.ToString();
                rootNode.pId = "0";
                rootNode.name = list.Title;
                rootNode.isParent = true;
                znodeList.Add(rootNode);
                foreach (SPField field in list.Fields)
                {
                    var node = new ZNodeModel();
                    node.id = field.Id.ToString();
                    node.pId = list.ID.ToString();
                    node.name = field.Title+"("+field.InternalName+")";
                    if (node.name == "")
                    {
                        continue;
                    }
                    if (listColumn.ContainsKey(field.Id.ToString()))
                    {
                        node.@checked = true;
                    }
                    node.isParent = false;
                    znodeList.Add(node);
                }
                znodeList = znodeList.OrderBy(p => p.name).ToList();
                result = javaScriptSerializer.Serialize(znodeList);
            }
            catch (Exception ex)
            {
                ULSLogger.LogError(ex.ToString());
                znodeList = new List<ZNodeModel>();
                var node = new ZNodeModel();
                node.name = "failed,please try again";
                node.id = Guid.NewGuid().ToString();
                node.pId = list.ID.ToString();
                znodeList.Add(node);
                result = javaScriptSerializer.Serialize(znodeList);
            }
            return result;
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }

        protected void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string listGuid = Request.QueryString["List"];               
                SPWeb web = SPControl.GetContextWeb(this.Context);
                SPList list = web.Lists[new Guid(listGuid)];
                if (list != null)
                {
                    ListSetInfor listSetInfor = Global.GetListSetInfor(list);
                    //listSetInfor.BackupPath = BackupFilePath.Text;
                    //listSetInfor.BackupStatus = BackupFileCheck.Checked ? Global.StrEnabled : Global.StrDisabled;
                    listSetInfor.DeleteStatus = hiddenDeleteFileCheck.Value=="true" ? Global.StrEnabled : Global.StrDisabled;
                    listSetInfor.VersionsStatus =hiddenVersionsRMSCheckBox.Value=="true" ? Global.StrEnabled : Global.StrDisabled;
                    listSetInfor.BatchSuccess = true;
                    listSetInfor.ScheduleList = scheduleDiv.Value;
                    //update selected column
                    listSetInfor.SelectedColumns = UpdateSelectedColumn();
                    //if (BackupFileCheck.Checked && string.IsNullOrEmpty(Global.CheckBackupPath(web, listSetInfor.BackupPath)))
                    //{
                    //    string strError = BakupPathError;
                    //    if (!string.IsNullOrEmpty(listSetInfor.BackupPath) && listSetInfor.BackupPath.Length > (Global.LimitBackupFileUrl - Global.LimitBackupFileName - 1))
                    //    {
                    //        strError = BakupPathLongError;
                    //    }
                    //    listSetInfor.BatchStatus = Global.StrDisabled;
                    //    listSetInfor.BatchLog = strError + " At " + DateTime.Now.ToString();
                    //    Global.SetListSetInfor(list, listSetInfor);
                    //}
                    //else 
                    if (hiddenCAStatusCheckBox.Value=="true")
                    {
                        if (!CheckRunning(listSetInfor))
                        {
                            listSetInfor.BatchStatus = Global.StrEnabled;
                            listSetInfor.BatchLog = RunningBatchMode;
                            listSetInfor.RunningTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            Global.SetListSetInfor(list, listSetInfor);
                            BatchModeWorker worker = new BatchModeWorker(web.Url, listGuid);
                            Thread workerThread = new Thread(worker.Run);
                            workerThread.Start();
                        }
                    }
                    else
                    {
                        listSetInfor.BatchStatus = Global.StrDisabled;
                        listSetInfor.BatchLog = "Successfully saved the settings at:" + DateTime.Now.ToString();
                        Global.SetListSetInfor(list, listSetInfor);
                    }
                   
                }
                Response.Redirect(Request.RawUrl, false);
                this.Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }
        private string UpdateSelectedColumn()
        {
            string result = "";
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                List<ZNodeModel> listColumn = javaScriptSerializer.Deserialize<List<ZNodeModel>>(this.columnJson.Value);
                Dictionary<string, string> selectedColumns = new Dictionary<string, string>();
                foreach (var node in listColumn)
                {
                    if (node.@checked == true)
                    {
                        selectedColumns.Add(node.id, node.name);
                    }
                }
                result = javaScriptSerializer.Serialize(selectedColumns);
            }
            catch (Exception ex)
            {
                ULSLogger.LogError(ex.ToString());
            }
            return result;
        }
        //protected void BackupFileCheck_Changed(object sender, EventArgs e)
        //{
        //    if (BackupFileCheck.Checked)
        //    {
        //        BackupFilePath.Visible = true;
        //    }
        //    else
        //    {
        //        BackupFilePath.Visible = false;
        //    }
        //}

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            String ListGuid = Request.QueryString["List"];
            SPUtility.Redirect("listedit.aspx?List=" + ListGuid, SPRedirectFlags.RelativeToLayoutsPage, this.Context);
        }

        private bool CheckRunning(ListSetInfor listSetInfor)
        {
            if (listSetInfor.BatchStatus.Equals(Global.StrEnabled) && !string.IsNullOrEmpty(listSetInfor.RunningTime))
            {
                DateTime runningTime = DateTime.Now;
                bool bParse = DateTime.TryParse(listSetInfor.RunningTime, out runningTime);
                if (bParse && (DateTime.Now - runningTime).TotalSeconds < 10 * 60)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class BatchModeWorker
    {
        private string m_webURL;
        private string m_listGuid;

        public BatchModeWorker(string webUrl, string listGuid)
        {
            m_webURL = webUrl;
            m_listGuid = listGuid;
        }

        public void Run()
        {
            Global.DoBatchModeRMXEX(m_webURL, m_listGuid);
        }
    }
}
