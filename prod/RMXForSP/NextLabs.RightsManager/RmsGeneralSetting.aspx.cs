using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Web.Services;
using System.Web;

namespace NextLabs.RightsManager
{
    public class RmsGeneralSetting : LayoutsPageBase
    {
        // Java PC Configuration
        protected InputFormTextBox JavaPcHost;
        protected InputFormTextBox OAUTHHost;
        protected InputFormTextBox ClientSecureID;
        protected InputFormTextBox ClientSecureKey;
        protected Label CheckStatus;

        // RMJavaSDK Configuration
        protected InputFormTextBox RouterURL;
        protected InputFormTextBox AppId;
        protected InputFormTextBox AppKey;
        protected InputFormTextBox TenantName;
        protected InputFormTextBox SecureViewURL;
        protected FileUpload CertificateFile;
        protected TextBox CertificateFileFileName;      
        protected InputFormTextBox CertificatePassword;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    GeneralSetInfor generalInfor = Global.GetGeneralSetInfor();
                    JavaPcHost.Text = generalInfor.JavaPcHost;
                    OAUTHHost.Text = generalInfor.OAUTHHost;
                    ClientSecureID.Text = generalInfor.ClientSecureID;
                    ClientSecureKey.Text = generalInfor.ClientSecureKey;
                    ClientSecureKey.Attributes.Add("value", generalInfor.ClientSecureKey);
                    RouterURL.Text = generalInfor.RouterURL;
                    AppId.Text = generalInfor.AppId;
                    AppKey.Text = generalInfor.AppKey;
                    AppKey.Attributes.Add("value",generalInfor.AppKey);
                    TenantName.Text = generalInfor.TenantName;
                    CertificatePassword.Attributes.Add("value", generalInfor.CertificatePassword);
                    CertificateFileFileName.Text = generalInfor.CertificateFileName;
                  //  ColumnNames.Text = generalInfor.ColumnNames;
                    SecureViewURL.Text = generalInfor.SecureViewURL;
                    //SelectedAll.Checked = generalInfor.SelectAllColumns;
                   // ColumnsSetting.Visible = !SelectedAll.Checked;

                    CheckStatus.Text = "";
                    
                    CertificateFile.Attributes.Add("onchange", "document.getElementById('" + CertificateFileFileName.ClientID + "').value=document.getElementById('" + CertificateFile.ClientID + "').value");
                }
            }
            catch(Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }
       
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                GeneralSetInfor generalInfor = new GeneralSetInfor();
                generalInfor.JavaPcHost = JavaPcHost.Text;
                generalInfor.OAUTHHost = OAUTHHost.Text;
                generalInfor.ClientSecureID = ClientSecureID.Text;
                generalInfor.ClientSecureKey = ClientSecureKey.Text;
                generalInfor.RouterURL = RouterURL.Text;
                if (generalInfor.RouterURL.EndsWith("/")) generalInfor.RouterURL = generalInfor.RouterURL.TrimEnd('/');
                generalInfor.AppId = AppId.Text;
                generalInfor.AppKey = AppKey.Text;
                generalInfor.TenantName = TenantName.Text;
               // generalInfor.SelectAllColumns = SelectedAll.Checked;
               // generalInfor.ColumnNames = ColumnNames.Text;
                generalInfor.SecureViewURL = SecureViewURL.Text;
                if (generalInfor.SecureViewURL.EndsWith("/")) generalInfor.SecureViewURL = generalInfor.SecureViewURL.TrimEnd('/');

                generalInfor.CertificatePassword = CertificatePassword.Text;
                if (CertificateFile.HasFile && CertificateFile.FileContent.Length>0)
                {
                    byte[] byteCert = new byte[CertificateFile.FileContent.Length];
                    CertificateFile.FileContent.Position = 0;
                    CertificateFile.FileContent.Read(byteCert, 0, byteCert.Length);
                    generalInfor.CertificatePfxFileBase64 = System.Convert.ToBase64String(byteCert);
                    generalInfor.CertificateFileName = CertificateFile.FileName;
                }
                else
                {
                    generalInfor.CertificatePfxFileBase64 = Global.GeneralInfor.CertificatePfxFileBase64;
                    generalInfor.CertificateFileName = Global.GeneralInfor.CertificateFileName;
                }

                var serializer = new JavaScriptSerializer();
                string strInfor = serializer.Serialize(generalInfor);
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    SPWebApplication adminWebApp = SPAdministrationWebApplication.Local;
                    adminWebApp.Properties[Global.StrGeneralSetInfor] = strInfor;
                    adminWebApp.Update();
                });

                bool bConnect = CloudAZQuery.CheckConnection(JavaPcHost.Text, OAUTHHost.Text, ClientSecureID.Text, ClientSecureKey.Text);
                if (bConnect)
                {
                    CheckStatus.Text = "<br/><br/>The connection is successful.<br/>";
                }
                else
                {
                    ULSLogger.LogError("The connection is failed.");
                    CheckStatus.Text = "<br/><br/>The connection is failed.<br/>";
                }
            }
            catch(Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        protected void ReturnButton_Click(object sender, EventArgs e)
        {
            string CAURL = this.Context.Request.Url.ToString().Replace(this.Context.Request.Url.AbsolutePath, "");
            SPUtility.Redirect(CAURL, SPRedirectFlags.Trusted, this.Context);
        }
    }
}
