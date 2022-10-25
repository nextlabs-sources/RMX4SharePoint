using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SkyDrmRestHelp;
using System.Web.UI.WebControls;
using System.DirectoryServices.ActiveDirectory;

namespace NextLabs.RightsManager
{
    public class NxlSecureView : UnsecuredLayoutsPageBase
    {
        protected Label ErrorTip;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                LoginData ld = SkyDrmSessionMgr.GetSkyDrmLoginData(Request);
                if (ld == null)
                {
                    ld = SkyDrmSessionMgr.LoginSkyDrmByTrustApp(this.Web, Response);
                    if (ld == null)
                    {
                        ErrorTip.Text = string.Format("Failed to log in to SkyDrm server.");
                        throw new Exception("LoginSkyDrmByTrustApp failed.");
                    }
                }
               

                {
                    //do secure view
                    string strItemId = Request.Params["itemId"];
                    string strListId = Request.Params["listId"];
                    SPList list = this.Web.Lists.GetList(new Guid(strListId), true);
                    if (list != null)
                    {
                        SPListItem listItem = list.GetItemById(int.Parse(strItemId));
                        if (listItem != null && listItem.File != null)
                        {
                            byte[] fileData = listItem.File.OpenBinary();

                            RemoteViewResult rvRes = SkyDrmSessionMgr.RemoteView(listItem.File.Name, fileData, ld);
                            if ((rvRes!=null) && (SkyDrmSessionMgr.IsUnAuth(rvRes.statusCode)))
                            {
                                ld = SkyDrmSessionMgr.LoginSkyDrmByTrustApp(this.Web, Response);
                                if (ld == null)
                                {
                                    ErrorTip.Text = string.Format("Failed to log in to SkyDrm server.");
                                    throw new Exception("LoginSkyDrmByTrustApp failed.");
                                }
                                else
                                {
                                    rvRes = SkyDrmSessionMgr.RemoteView(listItem.File.Name, fileData, ld);
                                }
                            }

                            if (rvRes != null)
                            {
                                RemoteViewData rvData = rvRes.remoteViewData;
                                if ((rvData != null) && (rvData.Cookie != null) && (rvData.viewUrl != null))
                                {
                                    //set cookie
                                    Domain domain = Domain.GetCurrentDomain();
                                    foreach (string ck in rvData.Cookie)
                                    {
                                        string strCk = ChangeCookieDomain(ck, domain.Name);
                                        Response.Headers.Add("Set-Cookie", strCk);
                                    }
                                    Response.Redirect(rvData.viewUrl, false);
                                    this.Context.ApplicationInstance.CompleteRequest();
                                    return;
                                }
                            }
                            ErrorTip.Text = string.Format("Failed to view the protected file: {0}, please try again later.", listItem.File.Name);
                            return;
                        }
                    }
                    ErrorTip.Text = string.Format("Failed to view the protected file, please try again later.");
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        protected string ChangeCookieDomain(string ckstr, string newdomain)
        {
            string ret = null;
            char[] sep = { ';' };
            char[] eq = { '=' };
            string[] strs = ckstr.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < strs.Length; ++idx)
            {
                string str = strs[idx];
                if (str.Contains("="))
                {
                    string[] kv = str.Split(eq, StringSplitOptions.RemoveEmptyEntries);
                    if (kv.Length >= 2)
                    {
                        if (kv[0].Trim().ToLower() == "Domain".ToLower())
                        {
                            kv[1] = newdomain;
                            strs[idx] = string.Join("=", kv);
                        }
                    }
                }
                else if ("Secure".ToLower() == str.Trim().ToLower())
                {
                    // to do
                    strs[idx] = "";
                }
            }
            ret = string.Join(";", strs);
            return ret;
        }
    }
}
