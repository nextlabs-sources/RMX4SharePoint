using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;

namespace NextLabs.RightsManager.Features.NextLabs.RightsManager.RMS
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("669445e8-6895-47fe-bac1-a4fe989103fb")]
    public class NextLabsRightsManagerEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            if (properties != null)
            {
                try
                {
                    SPWeb web = (SPWeb)properties.Feature.Parent;
                    EventReceiversModule.AddListReceiver(web);

                    if (!EventReceiversModule.CheckWebReceiverExisted(web))
                    {
                        EventReceiversModule.AddWebReceiver(web);
                    }

                    for (int i = 0; i < web.Lists.Count; i++)
                    {
                        SPList splist = web.Lists[i];
                        EventReceiversModule.AddItemReceiver(splist);
                    }
                }
                catch (Exception exp)
                {
                    ULSLogger.LogError(exp.ToString());
                }
            }
        }

        // Uncomment the method below to handle the event raised before a feature is deactivated.
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            if (properties != null)
            {
                try
                {
                    SPWeb web = (SPWeb)properties.Feature.Parent;
                    EventReceiversModule.RemoveListReceiver(web);
                    for (int i = 0; i < web.Lists.Count; i++)
                    {
                        SPList splist = web.Lists[i];
                        Global.RemoveBatchModeLog(splist);
                        EventReceiversModule.RemoveItemReceivers(splist);
                    }
                    EventReceiversModule.RemoveWebReceiver(web);
                }
                catch (Exception exp)
                {
                    ULSLogger.LogError(exp.ToString());
                }
            }
        }

        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
