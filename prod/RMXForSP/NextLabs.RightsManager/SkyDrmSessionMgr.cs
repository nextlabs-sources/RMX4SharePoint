using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using SkyDrmRestHelp;
using System.Threading;
using System.Net;
using Microsoft.SharePoint;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;


namespace NextLabs.RightsManager
{
    public class SkyDrmSessionMgr
    {
        static public readonly string m_strSkyDrmSessionKey = "skydrmsession";
        static public readonly string m_strSkyDrmCookiePrepend = "skydrm_";
        static public readonly string m_strSkyDrmCookieClientID = "clientId";
        static public readonly string m_strSkyDrmCookiePlatformID = "platformId";
        static public readonly string m_strSkyDrmCookieUserID= "userId";
        static public readonly string m_strSkyDrmCookieUserTicket = "ticket";
        static private byte[] m_byteSkydrmLoginCertificate = null;
        static private Dictionary<string, LoginData> m_dicSkydrmSession = new Dictionary<string, LoginData>(StringComparer.OrdinalIgnoreCase);
        static private ReaderWriterLockSlim m_rwlSession = new ReaderWriterLockSlim();
        static private SkyDrmRestMgr m_skydrmRestMgr = new SkyDrmRestMgr();

        [DataContract]
        private class RMXLoginPara
        {
            [DataMember]
            public int appId { get; set; }
            [DataMember]
            public long ttl { get; set; }
            [DataMember]
            public string nonce { get; set; }
            [DataMember]
            public string email { get; set; }
            [DataMember]
            public string userAttributes { get; set; }
            [DataMember]
            public string signature { get; set; }
        }

        [DataContract]
        private class LoginPara
        {
            [DataMember]
            public RMXLoginPara parameters { get; set; }
        }

        static SkyDrmSessionMgr()
        {
            Init();
        }

        public static bool Init()
        {
            try
            {
                GeneralSetInfor generalSetInfor = Global.GetGeneralSetInfor();
                m_skydrmRestMgr.Init(generalSetInfor.SecureViewURL);
            }
            catch (Exception ee)
            {
                Logger.LogError(ee.ToString());
            }
            return true;
        }

        public static ClassificationData GetClassifyData()
        {
            return null;
        }

        public static bool IsUnAuth(int nStatusCode)
        {
            return nStatusCode == (int)HttpStatusCode.Unauthorized;
        }

        private static void RedirectToLoginPage(HttpContext context, HttpResponse response, string siteUrl, string strAction, string strListId, string strItemId)
        {
            if (string.IsNullOrEmpty(siteUrl) || string.IsNullOrEmpty(strAction) || string.IsNullOrEmpty(strListId) || string.IsNullOrEmpty(strItemId))
            {
                //ULS Log
                ULSLogger.LogError("Parameter Error.");
                return;
            }
            string strParam = string.Format("action={0}&itemId={1}&listId={2}", strAction, strItemId, strListId);
            string strRedirUrl = string.Format("{0}/_layouts/15/NextLabs.RightsManager/SkyDrmLogin.aspx?{1}",
                siteUrl, strParam);
            //endResponseshall be false,be true will throw ex
            response.Redirect(strRedirUrl, false);
            context.ApplicationInstance.CompleteRequest();
        }

        public static RemoteViewResult RemoteView(string strFileName, byte[] data, LoginData ld)
        {
            try
            {
                //m_skydrmRestMgr.RemoteView("", null, null); throw exception
                RemoteViewResult rvRes = m_skydrmRestMgr.RemoteView(strFileName, data, ld);
                return rvRes;
            }
            catch(Exception exp)
            {
                //Windows Event Log
                Logger.LogError(exp.ToString());
                return null;
            }
        }

        public static string SignSkyDrmLoginData(string strData)
        {
            if (m_byteSkydrmLoginCertificate==null)
            {
                if (!string.IsNullOrWhiteSpace(Global.GeneralInfor.CertificatePfxFileBase64))
                {
                    m_byteSkydrmLoginCertificate = System.Convert.FromBase64String(Global.GeneralInfor.CertificatePfxFileBase64);
                }
                else
                {
                    ULSLogger.LogError("SignSkyDrmLoginData Certificate not uploaded");
                    return string.Empty;
                }
            }

            if (m_byteSkydrmLoginCertificate!=null)
            {
                try
                {
                    X509Certificate2 privateCert = new X509Certificate2(m_byteSkydrmLoginCertificate, Global.GeneralInfor.CertificatePassword, X509KeyStorageFlags.Exportable);
                    RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider)privateCert.PrivateKey;
                    using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                    {
                        RSA.ImportParameters(privateKey.ExportParameters(true));
                        byte[] byteData = System.Text.Encoding.ASCII.GetBytes(strData);
                        byte[] Signature = RSA.SignData(byteData, "SHA256");
                        string strSignature = System.Convert.ToBase64String(Signature);
                        return strSignature;
                    }
                }
                catch (System.Exception ex)
                {
                    ULSLogger.LogError("SignSkyDrmLoginData exception:" + ex.ToString());
                }   
            }

            return string.Empty;
            
        }

        private static string GetLoginNonce(string strAppID, string strAppKey)
        {
            NonceResult nonce = m_skydrmRestMgr.GetRmxLoginNonce(strAppID, strAppKey);
            if (nonce!=null && nonce.results!=null)
            {
                return nonce.results.nonce;
            }
            return string.Empty;
        }

        public static LoginData LoginSkyDrmByTrustApp(SPWeb web, HttpResponse Response)
        {
            LoginData ld = null;
            try
            {
                //get nonce

                string strNonce = GetLoginNonce(Global.GeneralInfor.AppId, Global.GeneralInfor.AppKey);
                if (string.IsNullOrEmpty(strNonce))
                {
                    ULSLogger.LogError("LoginSkyDrmByTrustApp, get nonce failed.");
                    return ld;
                }

                long nTTL = 6 * 3600 * 1000;
                //get user object
                SPUser user = web.CurrentUser;

                if (user.LoginName.Equals(@"SharePoint\system", StringComparison.OrdinalIgnoreCase))
                {//if user is system account, we get the user from thread
                    try {
                        string strName = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                        web.AllowUnsafeUpdates = true;
                        user = web.EnsureUser("i:" + strName);
                    }
                    catch (System.Exception ex)  {
                        ULSLogger.LogError("testx Exception on LoginSkyDrmByTrustApp::EnsureUser:" + ex.ToString());
                    }     
                }

                //get user email
                string strUserEmail = "";
                if (String.IsNullOrEmpty(Global.GeneralInfor.TenantName))
                {
                    // old, using share point login user
                    strUserEmail = user.Name;
                    if (!string.IsNullOrWhiteSpace(user.Email))
                    {
                        strUserEmail = user.Email;
                    }
                    else if ((user.UserId != null) && (!string.IsNullOrWhiteSpace(user.UserId.NameId)))
                    {
                        strUserEmail = user.UserId.NameId;
                    }
                }
                else
                {
                    strUserEmail = Global.GeneralInfor.TenantName;
                }


                //get user attribute from UserProfile
                Dictionary<string, string[]> dicUserAttr = Global.GetUserAttributeFromProfile(user, web.Site);
                string jsonUserAttr = Global.JsonSerializeObject(dicUserAttr);

                //calculate signature
                string strSignData = Global.GeneralInfor.AppId + "." +
                                       Global.GeneralInfor.AppKey + "." +
                                       strUserEmail + "." +
                                       nTTL.ToString() + "." +
                                       strNonce + "." +
                                       jsonUserAttr;
                string strSignature = SignSkyDrmLoginData(strSignData);

                //construct login parameters
                LoginPara para = new LoginPara();
                para.parameters = new RMXLoginPara()
                {
                    appId = int.Parse(Global.GeneralInfor.AppId),
                    email = strUserEmail,
                    ttl = nTTL,
                    nonce = strNonce,
                    userAttributes = jsonUserAttr,
                    signature = strSignature
                };
                string jsonLoginPara = Global.JsonSerializeObject(para);
                System.Diagnostics.Trace.TraceInformation("TrustAppLoginRMS:\n\tlogin data:[{0}]\n\tsign data:[{1}]", jsonLoginPara, strSignData);

                //login
                LoginResult ls = m_skydrmRestMgr.TrustAppLoginRMS(jsonLoginPara);

                if (ls != null)
                {
                    ld = ls.loginData;
                    if (ld != null)
                    {
                        string strSessionGuid = Guid.NewGuid().ToString();
                        AddedSkyDrmSessionInfo(strSessionGuid, ld);

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
                    }
                }
            }
            catch (System.Exception ex)
            {
                ULSLogger.LogError("Exception on LoginSkyDrmByTrustApp:" + ex.ToString() );
            }
           

            return ld;
        }

        public static LoginResult LoginToSkyDrm(string userName, string passwd)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passwd))
            {
                ULSLogger.LogError("Parameter Null.");
                return null;
            }
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(Encoding.Default.GetBytes(passwd));
            string strPwdMd5 = BitConverter.ToString(output).Replace("-", "");
            LoginResult ls = m_skydrmRestMgr.Login(0, userName, strPwdMd5);
            if(ls != null && ls.statusCode != 200) ULSLogger.LogWarning("Log in to SkyDrm failed, StatusCode: " + ls.statusCode);
            return ls;
        }

        public static void AddedSkyDrmSessionInfo(string strKey, LoginData ld)
        {
            try
            {
                m_rwlSession.EnterWriteLock();
                m_dicSkydrmSession.Add(strKey, ld);
            }
            finally
            {
                m_rwlSession.ExitWriteLock();
            }
        }

        private static LoginResult RetriveUserProfile(HttpRequest request)
        {
            //get session cookie from request
            string strUserID=null, strUserTicket=null, strClientID=null, strPlatfromId=null;

            //userId
            {
                string strCookieKey = m_strSkyDrmCookiePrepend + m_strSkyDrmCookieUserID;
                HttpCookie ck = request.Cookies.Get(strCookieKey);
                if (ck!=null)
                {
                    strUserID = ck.Value;
                }
            }

            //userTicket
            {
                string strCookieKey = m_strSkyDrmCookiePrepend + m_strSkyDrmCookieUserTicket;
                HttpCookie ck = request.Cookies.Get(strCookieKey);
                if (ck != null)
                {
                    strUserTicket = ck.Value;
                }
            }

            //clientId
            {
                string strCookieKey = m_strSkyDrmCookiePrepend + m_strSkyDrmCookieClientID;
                HttpCookie ck = request.Cookies.Get(strCookieKey);
                if (ck != null)
                {
                    strClientID = ck.Value;
                }
            }

            //platfromId
            {
                string strCookieKey = m_strSkyDrmCookiePrepend + m_strSkyDrmCookiePlatformID;
                HttpCookie ck = request.Cookies.Get(strCookieKey);
                if (ck != null)
                {
                    strPlatfromId = ck.Value;
                }
            }

            if (string.IsNullOrWhiteSpace(strUserID) || string.IsNullOrWhiteSpace(strUserTicket) ||
                string.IsNullOrWhiteSpace(strClientID) || string.IsNullOrWhiteSpace(strPlatfromId) )
            {
                System.Diagnostics.Trace.Write("RetriveUserProfile some param is empty");
                return null;
            }
            else
            {
                LoginResult loginRes = m_skydrmRestMgr.RetriveFullProfileV2(strUserID, strUserTicket, strClientID, strPlatfromId);
                return loginRes;
            }
        }

        public static LoginData GetSkyDrmLoginData(HttpRequest request)
        {
            LoginData ld = null;
            if (request == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return null;
            }
            //find session cookie
            HttpCookie ck = request.Cookies.Get(m_strSkyDrmSessionKey);
            if (ck!=null)
            {
                if( string.Equals(ck.Name, m_strSkyDrmSessionKey, StringComparison.OrdinalIgnoreCase) )
                {
                    //find login data
                    string value = ck.Value;

                    m_rwlSession.EnterReadLock();
                    if(m_dicSkydrmSession.ContainsKey(value))
                    {
                        ld = m_dicSkydrmSession[value];
                    }
                    m_rwlSession.ExitReadLock();

                    //if the login data, it may be login by other server(in Farm Environment)
                    if (ld == null)
                    {
                        LoginResult ls = RetriveUserProfile(request);
                        if (ls != null && ls.loginData != null)
                        {
                            ld = ls.loginData;

                            //cache the login data
                            SkyDrmSessionMgr.AddedSkyDrmSessionInfo(value, ld);
                        }
                    }
                }
            }
            return ld;
        }

        public static ClassificationResult GetClassificationResult(LoginData ld)
        {
            try
            {
                ClassificationResult result = m_skydrmRestMgr.GetClassificationProfile(ld, "");
                return result;
            }
            catch(Exception exp)
            {
                //Windows Event Log
                ULSLogger.LogError(exp.ToString());
            }
			return null;
}
    }
}
