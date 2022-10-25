using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace NextLabs.RightsManager
{
    class DelayedItemMgr
    {
        private  static List<DelayedItem> m_lstDelayedItems = new List<DelayedItem>();
        private  static readonly object m_DelItemLock = new object();
        private static System.Threading.Thread m_delayedThread = null;

        private class DelayedItem
        {
            public string webUrl;
            public string fileUrl;
            public Dictionary<string, string> dicTags;
            public Guid listID;
            public int itemID;
            public bool bOneDriver;
            public ListSetInfor listSetInfor;
            public DateTime deleteTime;
        }

        private static bool FindAndUpdateDelayItemByUrl(string fileUrl, DateTime processTime)
        {
            lock (m_DelItemLock)
            {
                for (int i = 0; i < m_lstDelayedItems.Count; i++)
                {
                    DelayedItem di = m_lstDelayedItems[i];
                    if (di.fileUrl.Equals(fileUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        di.deleteTime = processTime;
                        return true;
                    }
                }
            }
            return false;
        }

        public static void AddedDelayedItem(string strweburl, Guid listID, int itemID,
            ListSetInfor listSetInfor, string strfileurl,Dictionary<string, string> dicTags,bool bOneDriver)
        {
            //log
            string strLog = string.Format("AddedDelayedItem webUrl:{2}, listid:{0}, itemID:{1}",
                listID, itemID, strweburl);

            DateTime processTime = DateTime.Now + (new TimeSpan(0, 5, 0)); 
            if (!FindAndUpdateDelayItemByUrl(strfileurl, processTime))
            {
                //added item
                DelayedItem delayedItem = new DelayedItem();
                delayedItem.webUrl = strweburl;
                delayedItem.listID = listID;
                delayedItem.itemID = itemID;
                delayedItem.fileUrl = strfileurl;
                delayedItem.dicTags = dicTags;
                delayedItem.bOneDriver = bOneDriver;
                delayedItem.listSetInfor = listSetInfor;
                delayedItem.deleteTime = processTime;

                lock (m_DelItemLock)
                {
                    m_lstDelayedItems.Add(delayedItem);
                }
            }
            
            //start thread
            if (m_delayedThread == null)
            {
                m_delayedThread = new System.Threading.Thread(DelayedItemWorker);
                m_delayedThread.Start();
            }

        }

        private static DelayedItem GetDelayDeleteItem(DateTime dt)
        {
            lock(m_DelItemLock)
            {
                for(int i=0; i< m_lstDelayedItems.Count; i++)
                {
                    DelayedItem di = m_lstDelayedItems[i];
                    if (di.deleteTime<dt)
                    {
                        m_lstDelayedItems.RemoveAt(i);
                        return di;
                    }
                }
            }
            return null;
        }


        public static void DelayedItemWorker()
        {
            List<DelayedItem> lstDelayedItems = new List<DelayedItem>();
			ItemHandler itemHandle = new ItemHandler();
            while (true)
            {

                System.Threading.Thread.Sleep(180 * 1000);
                itemHandle.DisableEventFiring();

                DelayedItem delItem = null;
                while ((delItem = GetDelayDeleteItem(DateTime.Now)) != null)
                {
                    if(!DoDelayedItem(delItem))   lstDelayedItems.Add(delItem);
                }
                itemHandle.EnableEventFiring();
                {
                    // re-add failed item
                    lock (m_DelItemLock)
                    {
                        m_lstDelayedItems.AddRange(lstDelayedItems);
                    }
                }
                lstDelayedItems.Clear();
            }
        }

        private static bool DoDelayedItem(DelayedItem delItem)
        {
            bool bOk = true;
            //get item
            try
            {
                //log
                string strLog = string.Format("DoDelayDeleteItem webUrl:{2}, listid:{0}, itemID:{1}",
                    delItem.listID, delItem.itemID, delItem.webUrl);

                //get config info
                ListSetInfor listSetInfor = delItem.listSetInfor;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(delItem.webUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList spList = web.Lists.GetList(delItem.listID, false);
                            if (spList != null)
                            {
                                SPListItem item = spList.Items.GetItemById(delItem.itemID);
                                if (item != null && item.File != null)
                                {
                                    //check lock status first and only loop if file is locked
                                    if (!string.IsNullOrEmpty(item.File.LockId)) bOk = false;
                                    else if (item.File.CheckOutType == SPFile.SPCheckOutType.None)
                                    {
                                        RmxModule.EncryptItem(item, item.File, delItem.fileUrl, delItem.dicTags, listSetInfor);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (System.Exception ex)
            {
                ULSLogger.LogError(ex.ToString());
            }
            return bOk;
        }
    }
}
