using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.SharePoint.Administration;
using System.Threading;
using System.Threading.Tasks;

namespace NextLabs.RightsManager
{
    public class LogInfor
    {
        public string logMessage = "";
        public EventSeverity LogType = EventSeverity.Information;
        public LogInfor(string strMessage, EventSeverity type)
        {
            logMessage = strMessage;
            LogType = type;
        }
    }

    //SharePoint Event Log
    public class Logger : SPDiagnosticsServiceBase
    {
        public static string AreaName = "NextLabs RMX";
        public static string CategoryName = "NextLabs Category";
        private static object g_lock = new object();
        private static Logger g_Current = new Logger();
        private static Dictionary<string, LogInfor> g_dicLogs = new Dictionary<string, LogInfor>(StringComparer.OrdinalIgnoreCase);
        public static Logger Current()
        {
            return g_Current;
        }

        private Logger(): base("NextLabsRMX Logging Service", SPFarm.Local)
        {
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsCategory> listCategory = new List<SPDiagnosticsCategory>();
            listCategory.Add(new SPDiagnosticsCategory(CategoryName, TraceSeverity.Verbose, EventSeverity.Information));
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(AreaName, listCategory)
            };
            return areas;
        }

        public static void LogMessage(EventSeverity logType, string strMessage, bool bEnd = false)
        {
#if DEBUG
            Trace.WriteLine(logType.ToString() + "-------LogMessage----" + strMessage);
#endif
            string strThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
            Task.Run(() =>
            {
                AsyncLogMessage((int)logType, strThreadId, strMessage, bEnd);
            });
        }

        public static bool CheckWarningOrErrorLogs()
        {
            bool bRet = false;
            string strThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
            lock (g_lock)
            {
                if (g_dicLogs.ContainsKey(strThreadId))
                {
                    EventSeverity logType = g_dicLogs[strThreadId].LogType;
                    if (logType == EventSeverity.Error || logType == EventSeverity.Warning)
                    {
                        bRet = true;
                    }
                }
            }
            return bRet;
        }

        public static void ClearInformationLogs()
        {
            string strThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
            lock (g_lock)
            {
                if (g_dicLogs.ContainsKey(strThreadId))
                {
                    g_dicLogs.Remove(strThreadId);
                }
            }
        }

        private static void AsyncLogMessage(int nLogType, string strThreadId, string strMessage, bool bEnd)
        {
            EventSeverity logType = (EventSeverity)nLogType;
            LogInfor logInfor = null;
            lock (g_lock)
            {
                try
                {
                    if (g_dicLogs.ContainsKey(strThreadId))
                    {
                        logInfor = g_dicLogs[strThreadId];
                        if (logType < logInfor.LogType)
                        {
                            logInfor.LogType = logType;
                        }
                        logInfor.logMessage += ("\r\n" + strMessage);
                    }
                    else
                    {
                        logInfor = new LogInfor(strMessage, logType);
                        g_dicLogs.Add(strThreadId, logInfor);
                    }
                    if (bEnd)
                    {
                        g_dicLogs.Remove(strThreadId);
                        SPDiagnosticsCategory category = g_Current.Areas[AreaName].Categories[CategoryName];
                        g_Current.WriteEvent(1, category, logInfor.LogType, logInfor.logMessage);
                    }
                }
                catch(Exception exp)
                {
                    System.Diagnostics.Trace.WriteLine("NextLabs.RightsManager.Logger:: AsyncLogMessage: " + exp);
                }
            }
        }

        #region Real Time event log output
        public static void LogMessageRT(EventSeverity logType, string strMessage)
        {
//#if DEBUG
            Trace.WriteLine(logType.ToString() + "----EventLog----" + strMessage);
//#endif
            SPDiagnosticsCategory category = g_Current.Areas[AreaName].Categories[CategoryName];
            g_Current.WriteEvent(0, category, logType, strMessage);
        }
        //real time log single Error-Level trace
        public static void LogError(string strMessage)
        {
            LogMessageRT(EventSeverity.Error, LogUtil.GetErrorHeader() + strMessage);
        }
        ////real time log single Warning-Level trace
        public static void LogWarning(string strMessage)
        {
            LogMessageRT(EventSeverity.Warning, LogUtil.GetWarningHeader() + strMessage);
        }
        ////real time log single Information-Level trace
        public static void LogInformation(string strMessage)
        {
            LogMessageRT(EventSeverity.Information, LogUtil.GetInfoHeader() + strMessage);
        }
        #endregion
    }

    public class ULSLogInfor
    {
        public string logMessage = "";
        public TraceSeverity LogType = TraceSeverity.None;
        public ULSLogInfor(string strMessage, TraceSeverity type)
        {
            logMessage = strMessage;
            LogType = type;
        }
    }

    public class LogUtil
    {
        //n: the Nth layer function in function calling stack
        public static string GetCallerName(int n)
        {
            StackTrace trace = new StackTrace();
            string className = trace.GetFrame(n).GetMethod().ReflectedType.Name;
            System.Reflection.MethodBase methodName = trace.GetFrame(n).GetMethod();
            return string.Format("{0}.{1}", className, methodName.Name);
        }

        //used for Logger/ULSLogger, if diy log is needed, complete it by GetCallerName function.
        public static string GetWarningHeader()
        {
            return string.Format("{0} in {1}: ", "NextLabs RMX Warning", GetCallerName(3));
        }
        //used for Logger/ULSLogger, if diy log is needed, complete it by GetCallerName function.
        public static string GetErrorHeader()
        {
            return string.Format("{0} in {1}: ", "NextLabs RMX Error", GetCallerName(3));
        }
        //used for Logger/ULSLogger, if diy log is needed, complete it by GetCallerName function.
        public static string GetInfoHeader()
        {
            return string.Format("{0} in {1}: ", "NextLabs RMX Information", GetCallerName(3));
        }
    }

    //SharePoint ULS Log
    public class ULSLogger : SPDiagnosticsServiceBase
    {
        public static string AreaName = "NextLabs RMX";
        public static string CategoryName = "NextLabs Category";
        private static object g_lock = new object();
        private static ULSLogger g_Current = new ULSLogger();
        private static Dictionary<string, ULSLogInfor> g_dicLogs = new Dictionary<string, ULSLogInfor>();
        public static ULSLogger Current()
        {
            return g_Current;
        }

        private ULSLogger() : base("NextLabsRMX Logging Service", SPFarm.Local)
        {
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsCategory> listCategory = new List<SPDiagnosticsCategory>();
            listCategory.Add(new SPDiagnosticsCategory(CategoryName, TraceSeverity.Verbose, EventSeverity.Information));
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(AreaName, listCategory)
            };
            return areas;
        }
        //real time log single trace
        public static void LogMessage(TraceSeverity logType, string strMessage)
        {
//#if DEBUG
            Trace.WriteLine(logType.ToString() + "----ULSLog----" + strMessage);
//#endif
            SPDiagnosticsCategory category = g_Current.Areas[AreaName].Categories[CategoryName];
            g_Current.WriteTrace(0, category, logType, strMessage);
        }
        //real time log single Error-Level trace
        public static void LogError(string strMessage)
        {
            LogMessage(TraceSeverity.High, LogUtil.GetErrorHeader() + strMessage);
        }
        ////real time log single Warning-Level trace
        public static void LogWarning(string strMessage)
        {
            LogMessage(TraceSeverity.Medium, LogUtil.GetWarningHeader() + strMessage);
        }
        ////real time log single Information-Level trace
        public static void LogInformation(string strMessage)
        {
            LogMessage(TraceSeverity.Verbose, LogUtil.GetInfoHeader() + strMessage);
        }
    }
}
