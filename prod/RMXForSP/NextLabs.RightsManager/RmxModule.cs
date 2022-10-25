using System;
using System.Collections.Generic;
using SDKWrapper4RMXLib;
using Microsoft.SharePoint;
using System.IO;

namespace NextLabs.RightsManager
{
    public enum FileMethod
    {
        Save,
        Delete,
        MoveTo
    }

    public class RmxModule
    {
        //
        private static System.Threading.ReaderWriterLockSlim m_LockSlim = new System.Threading.ReaderWriterLockSlim();
        private static System.Collections.Hashtable m_nlist = new System.Collections.Hashtable();
        //
        public static string m_strCheckInCommnet = "";

        static RmxModule()
        {
#if SP2013
            string commonPath = "c:\\program files\\common files\\microsoft shared\\web server extensions\\15\\TEMPLATE\\LAYOUTS\\NextLabs.RightsManager\\";
#else
            string commonPath = "c:\\program files\\common files\\microsoft shared\\web server extensions\\16\\TEMPLATE\\LAYOUTS\\NextLabs.RightsManager\\";
#endif 
            IGlobalConfig theGlobalConfig = new GlobalConfig();
            theGlobalConfig.SetCommonLibPath(commonPath);
        }
        public const int SuccessStatus = 0;
        public static bool DoRMX(string sourcePath, string destPath, Dictionary<string, string> dicSetTags)
        {
            if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destPath)) return false;
            bool bInitClass = false; 
            try
            {
                bool bSetTag = false;
                INLRightsManager rm = new NLRightsManager();
                rm.InitializeClass(Global.GeneralInfor.RouterURL, Global.GeneralInfor.AppKey, int.Parse(Global.GeneralInfor.AppId));
                bInitClass = true;
                if (!IsNxlFileFormat(rm, sourcePath))
                {
                    List<string> lstTagKey = new List<string>();
                    List<string> lstTagValue = new List<string>();
                    if (dicSetTags != null && dicSetTags.Count > 0)
                    {
                        List<string> ListLowValue = new List<string>();
                        foreach (KeyValuePair<string, string> keyValue in dicSetTags)
                        {
                            if (keyValue.Key.Length > Global.LimitTagSize || CheckInvalidTag(keyValue.Key)) continue;
                            string[] values = keyValue.Value.Split(new string[] { Global.TagSeparator }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string value in values)
                            {
                                if (!CheckInvalidTag(keyValue.Value) && value.Length <= Global.LimitTagSize && !ListLowValue.Contains(value.ToLower()))
                                {
                                    ListLowValue.Add(value.ToLower());
                                   // rm.NLSetTag(keyValue.Key, value);
                                    lstTagKey.Add(keyValue.Key);
                                    lstTagValue.Add(value);
                                    bSetTag = true;
                                }
                            }
                            ListLowValue.Clear();
                        }
                    }
                    if (!bSetTag)
                    {
                        // rm.NLSetTag("Classification", "RMX For SP");
                        lstTagKey.Add("Classification");
                        lstTagValue.Add("RMX For SP");
                    } 
                    int status = rm.NLEncryptTokenGroup(0,sourcePath, destPath, lstTagKey.ToArray(), lstTagValue.ToArray() );
                    if (status == SuccessStatus) return true;
                }
            }
            catch (Exception exp)
            {
                if(!bInitClass) Logger.LogError(exp.ToString());
                else ULSLogger.LogError(exp.ToString());
            }
            return false;
        }

        private static bool CheckInvalidTag(string strTag)
        {
            if (string.IsNullOrEmpty(strTag) || strTag.Contains("%") || strTag.Contains("'") || strTag.Contains("\""))
            {
                return true;
            }
            return false;
        }

        // Check one file is "nxl" file, default value is false.
        public static bool IsNxlFileFormat(string filePath)
        {
            try
            {
                INLRightsManager nlRManager = new NLRightsManager();
                nlRManager.InitializeClass(Global.GeneralInfor.RouterURL, Global.GeneralInfor.AppKey, int.Parse(Global.GeneralInfor.AppId));
                return IsNxlFileFormat(nlRManager, filePath);
            }
            catch (Exception exp)
            {
                //windows event log
                Logger.LogError(exp.ToString());
            }
            return false;
        }

        // Check one file is "nxl" file, default value is false.
        public static bool IsNxlFileFormat(INLRightsManager nlRManager, string filePath)
        {
            int iNxl = 0;
            int iRet = nlRManager.NLIsNxl(filePath, out iNxl);
            if (iRet == SuccessStatus && iNxl != 0) return true;// iNxl is 0, it means it is not nxl file.
            return false;
        }

        // Get the normal file tags(pdf and office file): the file tag name is lower, multiple file tag value use pointed separator to separate.  
        public static bool GetNormalFileTags(string filePath, Dictionary<string, string> dicTags, string separator)
        {
            bool bRet = false;
            if (string.IsNullOrEmpty(filePath) || dicTags == null) return bRet;
            int nCount = 0;
            int iRet = -1;
            IFileTagManager fileTagManager = null;
            try
            {
                fileTagManager = new FileTagManager();
                iRet = fileTagManager.GetTagsCount(filePath, out nCount);
            }
            catch (Exception exp)
            {
                //windows event log
                Logger.LogError(exp.ToString());
                return false;
            }
            if (iRet == SuccessStatus && nCount > 1) 
            {
                string tagName = null;
                string tagValue = null;
                string tagLowerName = null;
                for (int i = 0; i < nCount; i++)
                {
                    iRet = fileTagManager.GetTagByIndex(filePath, i, out tagName, out tagValue);
                    if (iRet == SuccessStatus)
                    {
                        if (!string.IsNullOrEmpty(tagName))
                        {
                            tagLowerName = tagName.ToLower();
                            if (dicTags.ContainsKey(tagLowerName))
                                dicTags[tagLowerName] += (separator + tagValue); // mutiple value, use separator to separate.
                            else
                                dicTags[tagLowerName] = tagValue;
                            bRet = true;
                        }
                    }
                }
            }
            return bRet;
        }

        public static string SaveDataToTemp(byte[] data, string fileUrl)
        {
            string strFilePath = "";
            strFilePath = Path.GetTempFileName();
            if (!string.IsNullOrEmpty(strFilePath) && File.Exists(strFilePath))
            {
                try
                {
                    File.SetAttributes(strFilePath, FileAttributes.Normal);
                    File.Delete(strFilePath);
                }
                catch (Exception exp)
                {
                    ULSLogger.LogWarning(exp.ToString() + ". When deleting [" + strFilePath + "]");
                }
            }
            int ext = fileUrl.LastIndexOf(".");
            if (ext > 0)
            {
                string extName = fileUrl.Substring(ext);
                strFilePath += extName;
            }
            FileStream dstStream = File.Create(strFilePath);
            BinaryWriter writer = new BinaryWriter(dstStream);
            writer.Write(data);
            dstStream.Close();
            return strFilePath;
        }

        public static bool UpdateFileContent(SPList list, SPFile file, byte[] filecontent)
        {
            if (file == null || filecontent == null) return false;
            bool bTrue = false;
            bool bCheckOutByCode = false;

            try
            {
                if (list.ForceCheckout)
                {
                    file.CheckOut();
                    bCheckOutByCode = true;
                }

                bTrue = UpdateSafeFile(file, FileMethod.Save, "", filecontent);
                if (bTrue)
                    file.Update();
            }
            catch(Exception)
            {
                bTrue = false;
            }
            finally
            {
                if (bCheckOutByCode)
                {
                    file.CheckIn(m_strCheckInCommnet);
                } 	
            }
            
            return bTrue;
        }


        public static SPFile GetFileByUrl(SPWeb web, string fileUrl)
        {
            SPFile file = null;
            try
            {
                file = web.GetFile(fileUrl);
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
                file = null;
            }
            return file;
        }

        // catch exception into this function ,so invoker can determinate if need try again.
        public static bool EncryptItemVerstions(SPListItem item, SPFile itemFile,
            string fileUrl, Dictionary<string, string> dicTags,
            ListSetInfor listSetInfor)
        {
            bool bExisted = false;
            try
            {
                m_LockSlim.EnterWriteLock();
                if (m_nlist.Contains(fileUrl))     bExisted = true;
				else m_nlist.Add(fileUrl, "");
            }
            finally
            {
                m_LockSlim.ExitWriteLock();
            }
            // there is a thread has do it, do nothing at here.
            if (bExisted) return true;
            bool bProtect = EncryptItem(item, itemFile, fileUrl, dicTags, listSetInfor);
            try
            {
                m_LockSlim.EnterWriteLock();
                m_nlist.Remove(fileUrl);
            }
            finally
            {
                m_LockSlim.ExitWriteLock();
            }
            return bProtect;
        }

        // catch exception into this function ,so invoker can determinate if need try again.
        public static bool EncryptItem(SPListItem item, SPFile itemFile,
            string fileUrl, Dictionary<string, string> dicTags,
            ListSetInfor listSetInfor)
        {
            if (item == null || itemFile == null || listSetInfor == null || string.IsNullOrEmpty(fileUrl))
            {
                ULSLogger.LogError("Parameter Null");
                return false;
            }
            bool bEncryptSatus = true;
            bool bVersions = listSetInfor.VersionsStatus.Equals(Global.StrEnabled);
            bool bDeleteFile = listSetInfor.DeleteStatus.Equals(Global.StrEnabled);

            //bool bBackup = listSetInfor.BackupStatus.Equals(Global.StrEnabled);
            //string strBackupPath = listSetInfor.BackupPath.TrimEnd(Global.TrimFlag);
            if (itemFile.CheckOutType != SPFile.SPCheckOutType.None)
            {
                ULSLogger.LogWarning("Encrypt item [" + fileUrl + "] failed because the file is checkouted status.");
                return false;
            }
            if (!string.IsNullOrEmpty(itemFile.LockId))
            {
                ULSLogger.LogWarning("Encrypt item [" + fileUrl + "] failed because the file is locked status.");
                return false;
            }
            SPFile nxlFile = null;
            bool bExistedNxl = false;
            List<byte[]> listData = new List<byte[]>();
            try
            {
                nxlFile = GetFileByUrl(itemFile.Web, fileUrl + ".nxl");
                if (nxlFile != null && nxlFile.Exists)
                {
                    bExistedNxl = true;
                }

                // encrypt history version
                bool bListType = EventReceiversModule.SupportedListTypes.Contains((int)item.ParentList.BaseTemplate);
                if (!bExistedNxl && !bListType && bVersions && itemFile.Versions != null && itemFile.Versions.Count > 0)
                {
                    foreach (SPFileVersion fileVersion in itemFile.Versions)
                    {
                        if (fileVersion != null)
                        {
                            byte[] verData = fileVersion.OpenBinary();
                            if (!DoFileRMX(verData, fileUrl, dicTags, listData))
                            {
                                ULSLogger.LogWarning("[" + fileUrl + "] DoFileRMX Failed");
                                return false;
                            }
                        }
                    }
                }

                // encrypt current version
                byte[] fileData = itemFile.OpenBinary();
                if (!DoFileRMX(fileData, fileUrl, dicTags, listData))
                {
                    ULSLogger.LogWarning("[" + fileUrl + "] DoFileRMX Failed");
                    return false;
                }

                // encrypted data, update it to file
                if (listData.Count > 0)
                {
                    if (!bExistedNxl)
                    {
                        try
                        {
                            // here we can't use copy to directly, 2013 workflow status will be copy to nxl file directly
                            System.Collections.Hashtable hTable = new System.Collections.Hashtable();
                            System.Collections.Hashtable hPro = itemFile.Properties;
                            if (hPro != null && hPro.Keys.Count > 0)
                            {
                                foreach (string strkey in hPro.Keys)
                                {
                                    if (strkey.StartsWith("vti_") || strkey.StartsWith("_")) continue;
                                    if (strkey.Equals("ContentTypeId")) continue;

                                    string strValue = hPro[strkey].ToString();
                                    //bug58342,when value is null||empty,we dont sync column,but if column has default value,the nxl file's column wont be empty,will be default value,,won't fix 
                                    if (string.IsNullOrEmpty(strValue)) continue;
                                    if (strValue.Contains("_layouts/15/wrkstat.aspx?") || strValue.Contains("_layouts/16/wrkstat.aspx?")) continue;
                                    hTable[strkey] = hPro[strkey];
                                }
                            }
                            if (hTable.Count > 0) item.ParentList.RootFolder.Files.Add(fileUrl + ".nxl", listData[0], hTable);
                            else item.ParentList.RootFolder.Files.Add(fileUrl + ".nxl", listData[0]);
                            nxlFile = GetFileByUrl(itemFile.Web, fileUrl + ".nxl");

                            if (item.ParentList.ForceCheckout)
                            {//if the library Requrie checkout, the .nxl file created above is checked out ,we need check in first
                                nxlFile.CheckIn(m_strCheckInCommnet);
                            }
                        }
                        catch (Exception exp)
                        {
                            ULSLogger.LogError(exp.ToString());
                            return false;
                        }
                    }
                    if (nxlFile != null && nxlFile.Exists)
                    {
                        int i = 0;
                        if (!bExistedNxl)
                        {
                            i = 1;
                        }
                        for (; i < listData.Count; i++)
                        {
                            if (!UpdateFileContent(item.ParentList, nxlFile, listData[i]))
                            {
                                bEncryptSatus = false;
                                break;
                            }
                            if (i == listData.Count - 1 && bExistedNxl && EventReceiversModule.SupportedLibraryTypes.Contains((int)item.ParentList.BaseTemplate))
                            {
                                var newListItem = nxlFile.ListItemAllFields;
                                foreach(SPField field in item.Fields)
                                {
                                    object objValue = item[field.InternalName];
                                    if (objValue == null) continue;
                                    string strvalue = "";
                                    try
                                    {
                                        strvalue = field.GetFieldValueAsText(objValue);
                                        if (String.IsNullOrEmpty(strvalue) || String.IsNullOrWhiteSpace(strvalue)) continue;
                                        if (strvalue.Contains("_layouts/15/wrkstat.aspx?") || strvalue.Contains("_layouts/16/wrkstat.aspx?")) continue;
                                        //bug58290 should ignore field.InternalName.StartsWith("_"),won't fix 
                                        if (field.InternalName.StartsWith("vti_") || field.InternalName.StartsWith("_")) continue;
                                        if (field.InternalName.Equals("ContentTypeId")) continue;
                                        if (field.InternalName.Equals("FileLeafRef", StringComparison.OrdinalIgnoreCase)) continue;
                                        if (!field.ReadOnlyField && newListItem.Fields.ContainsField(field.InternalName))
                                        {
                                            newListItem[field.InternalName] = item[field.InternalName];
                                        }
                                    }
                                    catch
                                    { }
                                }
                                newListItem.Update();
                            }
                        }
                    }
                    if (!bEncryptSatus) return false;
                    if (bDeleteFile)
                    {
                        UpdateSafeFile(itemFile, FileMethod.Delete);
                    }
                    //DeleteOrBackupItem(item, itemFile, bBackup, strBackupPath, bVersions);         
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString() + " When processing file [" + fileUrl + "]");
            }
            finally
            {
                if(!bEncryptSatus)
                {
                    if (!bExistedNxl)
                    {
                        nxlFile.Delete();
                    }
					ULSLogger.LogError("[" + nxlFile.Name + "] update file content failed");
                }
            }
            return bEncryptSatus;
        }

        //public static void DeleteOrBackupItem(SPListItem item, SPFile itemFile, bool bBackup, string strBackupPath, bool bVersionsEnable)
        //{
        //    if (item == null || itemFile == null)
        //    {
        //        ULSLogger.LogError("Parameter Null");
        //        return;
        //    }
        //    string finalPath = Global.CheckBackupPath(item.Web, strBackupPath);
        //    if (bBackup && !string.IsNullOrEmpty(finalPath))
        //    {
        //        // Backup file after rights protection.
        //        string newName = itemFile.ServerRelativeUrl.Replace("/", "_");
        //        if (newName.Length > Global.LimitBackupFileName)
        //        {
        //            newName = newName.Substring(0, Global.LimitBackupFileName - Global.LimitRightFileName - 3) + "..." + newName.Substring(newName.Length - Global.LimitRightFileName, Global.LimitRightFileName);
        //        }
        //        string strBackupFile = finalPath + "/" + newName;
        //        SPFile backupFile = GetFileByUrl(itemFile.Web, strBackupFile);
        //        if (backupFile != null && backupFile.Exists)
        //        {
        //            //backup history version
        //            bool bListType = EventReceiversModule.SupportedListTypes.Contains((int)item.ParentList.BaseTemplate);
        //            if (!bListType && bVersionsEnable && itemFile.Versions != null && itemFile.Versions.Count > 0)
        //            {
        //                foreach (SPFileVersion fileVersion in itemFile.Versions)
        //                {
        //                    if (fileVersion != null)
        //                    {
        //                        byte[] verData = fileVersion.OpenBinary();
        //                        if (!UpdateFileContent(backupFile, verData))
        //                        {
        //                            ULSLogger.LogError("[" + backupFile.Name + "] backup failed");
        //                            return;
        //                        }
        //                    }
        //                }
        //            }

        //            //backup current version
        //            byte[] fileData = itemFile.OpenBinary();
        //            if (!UpdateFileContent(backupFile, fileData))
        //            {
        //                ULSLogger.LogError("[" + backupFile.Name + "] backup failed");
        //                return;
        //            }

        //            //delete the origin file
        //            UpdateSafeFile(itemFile, FileMethod.Delete);
        //        }
        //        else
        //        {
        //            UpdateSafeFile(itemFile, FileMethod.MoveTo, finalPath.TrimEnd(Global.TrimFlag) + "/" + newName);
        //        }
        //    }
        //    else
        //    {
        //        UpdateSafeFile(itemFile, FileMethod.Delete);
        //    }
        //}

        public static bool DoFileRMX(byte[] sourceData, string fileUrl, Dictionary<string, string> dicTags, List<byte[]> listData)
        {
            if (sourceData == null || string.IsNullOrEmpty(fileUrl))
            {
                ULSLogger.LogError("Parameter Null.");
                return false;
            }
            bool bRet = false;
            string sourcePath = "";
            string destPath = "";
            try
            {
                //Download the fileVersion
                sourcePath = SaveDataToTemp(sourceData, fileUrl);
                destPath = sourcePath + ".nxl";
                bool bEncrypt = DoRMX(sourcePath, destPath, dicTags);
                if (bEncrypt)
                {
                    byte[] data = File.ReadAllBytes(destPath);
                    if (data != null && data.Length >= 0)
                    {
                        listData.Add(data);
                        bRet = true;
                    }
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            finally
            {
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
                if (!string.IsNullOrEmpty(destPath) && File.Exists(destPath))
                {
                    try
                    {
                        File.SetAttributes(destPath, FileAttributes.Normal);
                        File.Delete(destPath);
                    }
                    catch (Exception exp)
                    {
                        ULSLogger.LogWarning(exp.ToString() + ". When deleting [" + destPath + "]");
                    }
                }
                if (!bRet)
                {
                    ULSLogger.LogWarning("DoFileRMX Error when processing [" + sourcePath + "]");
                }
            }
            return bRet;
        }

        private static bool UpdateSafeFile(SPFile file, FileMethod fileMethod, string destFileUrl = "", byte[] filecontent = null)
        {
            if (file == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return false;
            }
            bool bTrue = false;
            try
            {
                if (fileMethod == FileMethod.Save)
                {
                    if (filecontent != null) file.SaveBinary(filecontent);
                    else ULSLogger.LogError("Parameter Null.");
                }
                else if (fileMethod == FileMethod.Delete)
                {
                    file.Delete();
                }
                else if(fileMethod == FileMethod.MoveTo)
                {
                    if(!string.IsNullOrEmpty(destFileUrl)) file.MoveTo(destFileUrl);
                    else ULSLogger.LogError("Parameter Null.");
                }
                bTrue = true;
            }
            catch(Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            return bTrue;
        }
    }
}
