using System;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Web;
using SkyDrmRestHelp;


namespace NextLabs.RightsManager
{
    public class SkyDrmLogin : UnsecuredLayoutsPageBase
    {
        protected TextBox UserName;
        protected TextBox SkyDrmPassword;
        protected Button LoginButton;
        protected Label LoginErrorTip;
        protected Label Action;
        protected Label ItemId;
        protected Label ListId;

        public static readonly string m_strActionEncrypt = "encrypt";
        public static readonly string m_strActionSecView = "secureview";

        protected void Page_Load(object sender, EventArgs e)
        {
            ItemId.Text = Request.Params["itemId"];
            ListId.Text = Request.Params["listId"];
            Action.Text = Request.Params["action"];

            LoginData ld = SkyDrmSessionMgr.GetSkyDrmLoginData(Request);
            if (ld != null)
            {
                LoginErrorTip.Text = "Unauthorized. Please login again.";
            }
        }

        public void Login(object sender, EventArgs e)
        {

            //get username, password and login
            string strUserName = UserName.Text;
            string strPwd = SkyDrmPassword.Text;

            LoginResult ls = SkyDrmSessionMgr.LoginToSkyDrm(strUserName, strPwd);
            if (ls != null)
            {
                LoginData ld = ls.loginData;
                if (ld != null)
                {
                    string strSessionGuid = Guid.NewGuid().ToString();
                    SkyDrmSessionMgr.AddedSkyDrmSessionInfo(strSessionGuid, ld);

                    //response cookie
                    {
                        HttpCookie ck = new HttpCookie(SkyDrmSessionMgr.m_strSkyDrmSessionKey);
                        ck.Value = strSessionGuid;
                        Response.Cookies.Add(ck);
                    }
                    {
                        //clientId
                        string strClientIDKey = SkyDrmSessionMgr.m_strSkyDrmCookiePrepend + SkyDrmSessionMgr.m_strSkyDrmCookieClientID;
                        HttpCookie ck = new HttpCookie(strClientIDKey);
                        ck.Value = ld.clientId;
                        Response.Cookies.Add(ck);
                    }
                    {
                        //platformId
                        string strPlatformIDKey = SkyDrmSessionMgr.m_strSkyDrmCookiePrepend + SkyDrmSessionMgr.m_strSkyDrmCookiePlatformID;
                        HttpCookie ck = new HttpCookie(strPlatformIDKey);
                        ck.Value = ld.platformId;
                        Response.Cookies.Add(ck);
                    }
                    {
                        //userId
                        string strUserIdKey = SkyDrmSessionMgr.m_strSkyDrmCookiePrepend + SkyDrmSessionMgr.m_strSkyDrmCookieUserID;
                        HttpCookie ck = new HttpCookie(strUserIdKey);
                        ck.Value = ld.userId.ToString();
                        Response.Cookies.Add(ck);
                    }
                    {
                        //userTicket
                        string strUserTicketKey = SkyDrmSessionMgr.m_strSkyDrmCookiePrepend + SkyDrmSessionMgr.m_strSkyDrmCookieUserTicket;
                        HttpCookie ck = new HttpCookie(strUserTicketKey);
                        ck.Value = ld.ticket;
                        Response.Cookies.Add(ck);
                    }


                    //do action
                    string strAction = Action.Text;
                    if (string.Equals(strAction, m_strActionEncrypt, StringComparison.OrdinalIgnoreCase))
                    {
                        //redirect to Protection page
                        string strParam = string.Format("itemId={0}&listId={1}", ItemId.Text, ListId.Text);

                        string strRedirUrl = string.Format("{0}/_layouts/15/NextLabs.RightsManager/NxlProtection.aspx?{1}",
                                this.Web.Url, strParam);
                        Response.Redirect(strRedirUrl, false);
                        this.Context.ApplicationInstance.CompleteRequest();
                    }
                    else if (string.Equals(strAction, m_strActionSecView, StringComparison.OrdinalIgnoreCase))
                    {
                        //redirect secure view page
                        string strParam = string.Format("itemId={0}&listId={1}", ItemId.Text, ListId.Text);

                        string strRedirUrl = string.Format("{0}/_layouts/15/NextLabs.RightsManager/NxlSecureView.aspx?{1}",
                                this.Web.Url, strParam);
                        Response.Redirect(strRedirUrl, false);
                        this.Context.ApplicationInstance.CompleteRequest();
                    }
                }
                else
                {
                    LoginErrorTip.Text = string.Format("Sorry, Log in failed. Status:{0}, Message:{1}", ls.statusCode, ls.message);
                }
            }
            else
            {
                LoginErrorTip.Text = "Sorry, Log in failed, Please try again later.";
            }
        }
    }
}
