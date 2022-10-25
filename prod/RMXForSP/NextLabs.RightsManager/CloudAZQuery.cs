using System;
using System.Collections.Generic;
using QueryCloudAZSDK;
using QueryCloudAZSDK.CEModel;


namespace NextLabs.RightsManager
{
    public sealed class CloudAZQuery
    {
        private static volatile CloudAZQuery instance = null;
        
        private string m_strJPCHost;
        private string m_strOAUTHost;
        private string m_strClientId;
        private string m_strClientSecure;
        private CEQuery m_obCEQuery;

        private CloudAZQuery()        
        {
           InitParams();
        }

        private static object syncRoot = new Object();
        public static CloudAZQuery Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new CloudAZQuery();
                    }
                }
                return instance;
            }
        }
         
        private void InitParams()
        {
            GeneralSetInfor generalSetInfor = Global.GetGeneralSetInfor();
            m_strJPCHost = generalSetInfor.JavaPcHost;
            m_strOAUTHost = generalSetInfor.OAUTHHost;
            m_strClientId = generalSetInfor.ClientSecureID;
            m_strClientSecure = generalSetInfor.ClientSecureKey;
            m_obCEQuery = new CEQuery(m_strJPCHost, m_strOAUTHost, m_strClientId, m_strClientSecure);
        }

        public static bool CheckConnection(string strPCHost, string strOAuthServiceHost, string strClientId, string strClientSecret)
        {
            try
            {
                List<CEObligation> listObligation = new List<CEObligation>();
                PolicyResult emPolicyResult = PolicyResult.DontCare;
                CEQuery ceQuery = new CEQuery(strPCHost, strOAuthServiceHost, strClientId, strClientSecret);
                CEAttres ceAttres = new CEAttres();
                ceAttres.AddAttribute(new CEAttribute("url", "http://mytest", CEAttributeType.XacmlString));
                CERequest ceReq = CreateQueryReq("EDIT", "10.23.10.10", "http://mytest", ceAttres, "userId", "userName", new CEAttres());
                ceQuery.RefreshToken();
                QueryStatus emQueryRes = ceQuery.CheckResource(ceReq, out emPolicyResult, out listObligation);
                if (QueryStatus.S_OK == emQueryRes)
                {
                    return true;
                }
                else
                {
                    ULSLogger.LogError("Check Policy Server Connection Failed, Status: " + emQueryRes.ToString());
                }
            }
            catch(Exception exp)
            {
                //Windows Event Log
                ULSLogger.LogError("Check Policy Server Connection Failed. " + exp.ToString());
            }
            return false;
        }

        public static CERequest CreateQueryReq(string strAction, string remoteAddress, string srcName, 
            CEAttres ceSrcAttr, string userSid, string userName, CEAttres ceUserAttr)
        {
            if (!string.IsNullOrEmpty(strAction) && !string.IsNullOrEmpty(srcName) && !string.IsNullOrEmpty(userSid))
            {
                CERequest obReq = new CERequest();
                // Host
                if (!string.IsNullOrEmpty(remoteAddress) && !remoteAddress.Contains(":")) // George: Not support IPV6
                {
                    obReq.SetHost(remoteAddress, remoteAddress, null);
                }

                // Action
                obReq.SetAction(strAction);

                // User
                if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(userSid) || ceUserAttr != null)
                {
                    obReq.SetUser(userSid, userName, ceUserAttr);
                }

                // Resource
                ceSrcAttr.AddAttribute(new CEAttribute("url", srcName, CEAttributeType.XacmlString));
                obReq.SetSource(srcName, "rmx", ceSrcAttr);

                // App
                obReq.SetApp("Sharepoint Rights Management", null, null, null);

                // Environment: set Dont Care case.
                {
                    CEAttres envAttrs = new CEAttres();
                    envAttrs.AddAttribute(new CEAttribute("dont-care-acceptable", "yes", QueryCloudAZSDK.CEModel.CEAttributeType.XacmlString));
                    obReq.SetEnvAttributes(envAttrs);
                }
                return obReq;
            }
            return null;
        }

        public QueryStatus QueryCloudAZPC(CERequest obReq, ref List<CEObligation> listObligation, ref PolicyResult emPolicyResult)
        {
            QueryStatus emQueryRes = QueryStatus.S_OK;
            emPolicyResult = PolicyResult.DontCare;
            if (obReq != null)
            {
                try
                {
                    emQueryRes = m_obCEQuery.CheckResource(obReq, out emPolicyResult, out listObligation);

                    if (emQueryRes == QueryStatus.E_Unauthorized)
                    {
                        m_obCEQuery.RefreshToken();
                        emQueryRes = m_obCEQuery.CheckResource(obReq, out emPolicyResult, out listObligation);
                    }
                }
                catch (Exception exp)
                {
                    ULSLogger.LogError(exp.ToString());
                }
            }
            return emQueryRes;
        }

        public QueryStatus MultipleQueryColuAZPC(List<CERequest> ceRequests, out List<PolicyResult> listPolicyResults, out List<List<CEObligation>> listObligations)
        {
            QueryStatus emQueryRes = QueryStatus.S_OK;

            emQueryRes = m_obCEQuery.CheckMultipleResources(ceRequests, out listPolicyResults, out listObligations);

            if (emQueryRes == QueryStatus.E_Unauthorized)
            {
                m_obCEQuery.RefreshToken();
                emQueryRes = m_obCEQuery.CheckMultipleResources(ceRequests, out listPolicyResults, out listObligations);
            }

            return emQueryRes;
        }
     }
}
