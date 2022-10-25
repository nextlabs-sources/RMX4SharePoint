using Microsoft.SharePoint;
using System;
using System.Text.RegularExpressions;

namespace NextLabs.RightsManager
{
    public class ItemHandler : SPItemEventReceiver
    {
        new public void DisableEventFiring()
        {
            base.EventFiringEnabled = false;
        }

        new public void EnableEventFiring()
        {
            base.EventFiringEnabled = true;
        }

        private bool IsUnSupportedFile(SPListItem spItem)
        {
            if (spItem == null)
            {
                ULSLogger.LogError("Null Parameter.");
                return true;
            }
            if (Global.IsOneNoteItem(spItem)) return true;
            if (Global.IsIgnoredFileName(spItem.File)) return true;
            return false;
        }
        /// <summary>
        /// Asynchronous After event that occurs after a new item has been added to its containing object.
        /// </summary>
        /// <param name="properties">An Microsoft.SharePoint.SPItemEventProperties object that represents properties of the event handler.</param>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            SPListItem spItem = properties.ListItem;
            if (IsUnSupportedFile(spItem))
            {
                return;
            }
            this.EventFiringEnabled = false;
            Global.DoRMXEnforcer(properties, SPEventReceiverType.ItemAdded);
            base.ItemAdded(properties);
            this.EventFiringEnabled = true;

        }

        /// <summary>
        /// Asynchronous After event that occurs after an item has been updated.
        /// </summary>
        /// <param name="properties">An Microsoft.SharePoint.SPItemEventProperties object that represents properties of the event handler.</param>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            SPListItem spItem = properties.ListItem;
            if (IsUnSupportedFile(spItem))
            {
                return;
            }
            this.EventFiringEnabled = false;
            Global.DoRMXEnforcer(properties, SPEventReceiverType.ItemUpdated);
            base.ItemUpdated(properties);
            this.EventFiringEnabled = true;
        }

        /// <summary>
        /// Asynchronous After event that occurs after  a user adds an attachment to an item.
        /// </summary>
        /// <param name="properties">An Microsoft.SharePoint.SPItemEventProperties object that represents properties of the event handler.</param>
        public override void ItemAttachmentAdded(SPItemEventProperties properties)
        {
            this.EventFiringEnabled = false;
            Global.DoRMXEnforcer(properties, SPEventReceiverType.ItemAttachmentAdded);
            base.ItemAttachmentAdded(properties);
            this.EventFiringEnabled = true;
        }

        /// <summary>
        /// Asynchronous After event that occurs after a user move an item to new url.
        /// </summary>
        /// <param name="properties">An Microsoft.SharePoint.SPItemEventProperties object that represents properties of the event handler.</param>
        public override void ItemFileMoved(SPItemEventProperties properties)
        {
            // new excel and rename, will not do at here.
            if (properties.BeforeUrl.StartsWith("Book", StringComparison.OrdinalIgnoreCase) ||
                properties.BeforeUrl.StartsWith("New Microsoft Excel Worksheet", StringComparison.OrdinalIgnoreCase))
            {
                if (properties.BeforeUrl.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase)) return;
            }
            SPListItem spItem = properties.ListItem;
            if (IsUnSupportedFile(spItem))
            {
                return;
            }
            this.EventFiringEnabled = false;
            Global.DoRMXEnforcer(properties, SPEventReceiverType.ItemFileMoved);
            base.ItemFileMoved(properties);
            this.EventFiringEnabled = true;
        }
    }

    public class WebHandler : SPWebEventReceiver
    {
        public override void WebProvisioned(SPWebEventProperties properties)
        {
            this.EventFiringEnabled = false;
            try
            {
                SPWeb web = properties.Web;
                Guid rmsGuid = new Guid(EventReceiversModule.StrRmsFeatureGuid);
                if (web.Features[rmsGuid] == null)
                {
                    web.Features.Add(rmsGuid, true);
                    web.Update();
                }
            }
            catch(Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            finally
            {
                base.WebProvisioned(properties);
                this.EventFiringEnabled = true;
            }
        }
    }

    public class ListHandler : SPListEventReceiver
    {
        public override void ListAdded(SPListEventProperties properties)
        {
            this.EventFiringEnabled = false;
            try
            {
                SPList list = properties.List;
                EventReceiversModule.AddItemReceiver(list);
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            finally
            {
                base.ListAdded(properties);
                this.EventFiringEnabled = true;
            }
        }
    }
}
