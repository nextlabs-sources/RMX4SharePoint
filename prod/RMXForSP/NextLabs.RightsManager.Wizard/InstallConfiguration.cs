using System;
using System.Configuration;
using Microsoft.SharePoint.Administration;
using System.Data.SqlClient;
using NextLabs.RightsManager.Wizard.Resources;
using System.Diagnostics;
using System.Xml;
using System.Text;

namespace NextLabs.RightsManager.Wizard
{
    internal class InstallConfiguration
    {
        #region Constants
        public class BackwardCompatibilityConfigProps
        {
            // "Apllication" mispelled on purpose to match original mispelling released
            public const string RequireDeploymentToCentralAdminWebApllication = "RequireDeploymentToCentralAdminWebApllication";
            // Require="MOSS" = RequireMoss="true" 
            public const string Require = "Require";
        }

        public class ConfigProps
        {
            public const string SolutionId = "SolutionId";
            public const string SolutionFile = "SolutionFile";
            public const string SolutionTitle = "SolutionTitle";
            public const string SolutionVersion = "SolutionVersion";
        }
        #endregion

        #region Internal Static Properties
        internal static Guid SolutionId
        {
            get { return new Guid(ConfigurationManager.AppSettings[ConfigProps.SolutionId]); }
        }

        internal static string SolutionFile
        {
            get { return ConfigurationManager.AppSettings[ConfigProps.SolutionFile]; }
        }

        internal static string SolutionTitle
        {
            get { return ConfigurationManager.AppSettings[ConfigProps.SolutionTitle]; }
        }

        internal static Version SolutionVersion
        {
            get { return new Version(ConfigurationManager.AppSettings[ConfigProps.SolutionVersion]); }
        }

        internal static Version InstalledVersion
        {
            get
            {
                try
                {
                    SPFarm farm = SPFarm.Local;
                    string key = "Solution_" + SolutionId.ToString() + "_Version";
                    return farm.Properties[key] as Version;
                }

                catch (NullReferenceException ex)
                {
                    throw new InstallException(CommonUIStrings.installExceptionDatabase, ex);
                }

                catch (SqlException ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
            }

            set
            {
                try
                {
                    SPFarm farm = SPFarm.Local;
                    string key = "Solution_" + SolutionId.ToString() + "_Version";
                    farm.Properties[key] = value;
                    farm.Update();
                }
                catch (NullReferenceException ex)
                {
                    throw new InstallException(CommonUIStrings.installExceptionDatabase, ex);
                }
                catch (SqlException ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
            }
        }
        #endregion

        #region Internal Static Methods

        internal static string FormatString(string str)
        {
            return FormatString(str, null);
        }

        internal static string FormatString(string str, params object[] args)
        {
            string formattedStr = str;
            string solutionTitle = SolutionTitle;
            if (!String.IsNullOrEmpty(solutionTitle))
            {
                formattedStr = formattedStr.Replace("{SolutionTitle}", solutionTitle);
            }
            if (args != null)
            {
                formattedStr = String.Format(formattedStr, args);
            }
            return formattedStr;
        }
        #endregion
    }

    public enum InstallOperation
    {
        Install,
        Uninstall
    }

    // configure Nxl icon and registry dll
    public class RegNxlConfig
    {
        private static readonly ILog log = LogManager.GetLogger();

#if SP2013
        public const string DocionPath = "C:\\Program Files\\Common Files\\Microsoft Shared\\Web Server Extensions\\15\\TEMPLATE\\XML\\DOCICON.xml";
        public const string SdkPath = "C:\\Program Files\\Common Files\\Microsoft Shared\\Web Server Extensions\\15\\TEMPLATE\\Layouts\\NextLabs.RightsManager\\bin64\\SDKWrapper4RMX.dll";
#else
        public const string DocionPath = "C:\\Program Files\\Common Files\\Microsoft Shared\\Web Server Extensions\\16\\TEMPLATE\\XML\\DOCICON.xml";
        public const string SdkPath = "C:\\Program Files\\Common Files\\Microsoft Shared\\Web Server Extensions\\16\\TEMPLATE\\Layouts\\NextLabs.RightsManager\\bin64\\SDKWrapper4RMX.dll";
#endif 

        public static void RegNxl()
        {
            try
            {
                ModifyDociconXml();
                ExecuteCommand("Regsvr32.exe", "/s /i \"" + SdkPath + "\"");
            }
            catch (Exception exp)
            {
                log.Error("RegNxl with error: " + exp);
            }
        }

        public static void UnRegNxl()
        {
            try
            {
                RestoreDociconXml();
                ExecuteCommand("Regsvr32.exe", "/s /u \"" + SdkPath + "\"");
            }
            catch (Exception exp)
            {
                log.Error("UnRegNxl with error: " + exp);
            }
        }

        private static void ModifyDociconXml()
        {
            string filePath = DocionPath;
            if (System.IO.File.Exists(filePath) == false)
                return;
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.PreserveWhitespace = true;
            XmlDoc.Load(filePath);
            XmlNode byExtensionNode = null;

            byExtensionNode = XmlDoc.SelectSingleNode("/DocIcons/ByExtension");
            XmlNode mappingNode = XmlDoc.SelectSingleNode("/DocIcons/ByExtension/Mapping[@Key='nxl']");
            if (mappingNode != null)
            {
                byExtensionNode.RemoveChild(mappingNode);
            }

            XmlElement nodeToAdd = null;
            XmlAttribute keyAttribute = null;
            XmlAttribute valueAttribute = null;
            XmlAttribute openControlAttribute = null;

            nodeToAdd = XmlDoc.CreateElement("Mapping");

            keyAttribute = XmlDoc.CreateAttribute("Key");
            keyAttribute.Value = "nxl";
            nodeToAdd.Attributes.Append(keyAttribute);

            valueAttribute = XmlDoc.CreateAttribute("Value");
            valueAttribute.Value = "NextLabs.RightsManager/nxl.png";
            nodeToAdd.Attributes.Append(valueAttribute);

            openControlAttribute = XmlDoc.CreateAttribute("OpenControl");
            openControlAttribute.Value = "SharePoint.OpenDocuments";
            nodeToAdd.Attributes.Append(openControlAttribute);

            byExtensionNode.PrependChild(nodeToAdd);
            WriteXmlDocumentToFile(XmlDoc, filePath);
        }
        private static void RestoreDociconXml()
        {
            string filePath = DocionPath;
            if (System.IO.File.Exists(filePath) == false)
                return;
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.PreserveWhitespace = true;
            XmlDoc.Load(filePath);

            XmlNode byExtensionNode = XmlDoc.SelectSingleNode("/DocIcons/ByExtension");

            XmlNode mappingNode = XmlDoc.SelectSingleNode("/DocIcons/ByExtension/Mapping[@Key='nxl']");

            if (mappingNode != null)
            {
                byExtensionNode.RemoveChild(mappingNode);
            }

            WriteXmlDocumentToFile(XmlDoc, filePath);
        }

        private static void WriteXmlDocumentToFile(XmlDocument docToWrite, String filePath)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(filePath, Encoding.UTF8);
            docToWrite.WriteTo(xmlWriter);
            xmlWriter.Close();
        }

        private static void ExecuteCommand(string fileName, string args)
        {
            if (System.IO.File.Exists(SdkPath) == false)
            {
                return;
            }
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = fileName;
            start.Arguments = args;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardInput = false;
            start.UseShellExecute = false;
            Process p = Process.Start(start);
            p.WaitForExit();
            int result = p.ExitCode;
            p.Close();
        }
    }
}
