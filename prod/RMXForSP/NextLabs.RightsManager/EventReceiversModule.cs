using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace NextLabs.RightsManager
{
    class EventReceiversModule
    {
        // Event Receiver
        public const string StrRmsFeatureGuid = "4f007acf-75f2-465e-959e-1ecff1bc20fe";
        public static List<int> SupportedLibraryTypes = new List<int> { 101, 109, 115, 119, 700, 850, 851, 1302 };
        public static List<int> SupportedListTypes = new List<int> { 100, 102, 103, 104, 105, 106, 107, 108, 120, 170, 171, 1100 };
        public const string StrAssemblyName = "NextLabs.RightsManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c98953f573c68e1d";
        public const string StrItemHandlerClassName = "NextLabs.RightsManager.ItemHandler";
        public const string StrListHandlerClassName = "NextLabs.RightsManager.ListHandler";
        public const string StrWebHandlerClassName = "NextLabs.RightsManager.WebHandler";
        public const string StrItemAddedName = "ItemAddedEventHandler";
        public const string StrItemUpdatedName = "ItemUpdatedEventHandler";
        public const string StrItemFileMovedName = "ItemFileMovedEventHandler";
        public const string StrItemAttachmentAddedName = "ItemAttachmentAddedEventHandler";
        public const string StrListAddedName = "ListAddedEventHandler";
        public const string StrWebProvisionedName = "WebProvisionedEventHandler";
        public const int SequenceNumber = 21000;

        public static void AddWebReceiver(SPWeb web)
        {
            try
            {
                SPEventReceiverDefinitionCollection eventReceivers = web.EventReceivers;
                SPEventReceiverDefinition receiverDefinition = eventReceivers.Add();
                receiverDefinition.Name = StrWebProvisionedName;
                receiverDefinition.Type = SPEventReceiverType.WebProvisioned;
                receiverDefinition.Synchronization = SPEventReceiverSynchronization.Asynchronous;
                receiverDefinition.SequenceNumber = SequenceNumber;
                receiverDefinition.Assembly = StrAssemblyName;
                receiverDefinition.Class = StrWebHandlerClassName;
                receiverDefinition.Update();
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        public static void AddListReceiver(SPWeb web)
        {
            try
            {
                SPEventReceiverDefinitionCollection eventReceivers = web.EventReceivers;
                SPEventReceiverDefinition receiverDefinition = eventReceivers.Add();
                receiverDefinition.Name = StrListAddedName;
                receiverDefinition.Type = SPEventReceiverType.ListAdded;
                receiverDefinition.Synchronization = SPEventReceiverSynchronization.Asynchronous;
                receiverDefinition.SequenceNumber = SequenceNumber;
                receiverDefinition.Assembly = StrAssemblyName;
                receiverDefinition.Class = StrListHandlerClassName;
                receiverDefinition.Update();
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        public static void AddItemReceiver(SPList splist)
        {
            try
            {
                int type = (int)splist.BaseTemplate;
                if (SupportedLibraryTypes.Contains(type))
                {
                    AddItemReceiver(splist, SPEventReceiverType.ItemAdded, StrItemAddedName);
                    AddItemReceiver(splist, SPEventReceiverType.ItemUpdated, StrItemUpdatedName);
                    AddItemReceiver(splist, SPEventReceiverType.ItemFileMoved, StrItemFileMovedName);
                }
                else if (SupportedListTypes.Contains(type))
                {
                    AddItemReceiver(splist, SPEventReceiverType.ItemAttachmentAdded, StrItemAttachmentAddedName);
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }

        }

        public static void AddItemReceiver(SPList splist, SPEventReceiverType ReceiverType, string ReceiverName)
        {
            try
            {
                SPEventReceiverDefinitionCollection eventReceivers = splist.EventReceivers;
                SPEventReceiverDefinition receiverDefinition = eventReceivers.Add();
                receiverDefinition.Name = ReceiverName;
                receiverDefinition.Type = ReceiverType;
                receiverDefinition.Synchronization = SPEventReceiverSynchronization.Asynchronous;
                receiverDefinition.SequenceNumber = SequenceNumber;
                receiverDefinition.Assembly = StrAssemblyName;
                receiverDefinition.Class = StrItemHandlerClassName;
                receiverDefinition.Update();
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        public static bool CheckWebReceiverExisted(SPWeb web)
        {
            bool bRet = false;
            try
            {
                SPEventReceiverDefinitionCollection eventReceivers = web.EventReceivers;
                for (int i = eventReceivers.Count - 1; i >= 0; i--)
                {
                    SPEventReceiverDefinition def = eventReceivers[i];
                    if (def.Assembly.Equals(StrAssemblyName, StringComparison.OrdinalIgnoreCase)
                        && def.Class.Equals(StrWebHandlerClassName, StringComparison.OrdinalIgnoreCase)
                        && def.Type.Equals(SPEventReceiverType.WebProvisioned)
                        && def.Name.Equals(StrWebProvisionedName, StringComparison.OrdinalIgnoreCase))
                    {
                        bRet = true;
                        break;
                    }
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
            return bRet;
        }

        public static void RemoveWebReceiver(SPWeb web)
        {
            try
            {
                SPEventReceiverDefinitionCollection eventReceivers = web.EventReceivers;
                for (int i = eventReceivers.Count - 1; i >= 0; i--)
                {
                    SPEventReceiverDefinition def = eventReceivers[i];
                    if (def.Assembly.Equals(StrAssemblyName, StringComparison.OrdinalIgnoreCase) &&
                        def.Class.Equals(StrWebHandlerClassName, StringComparison.OrdinalIgnoreCase))
                    {
						def.Delete();
                        break;
                    }
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        public static void RemoveListReceiver(SPWeb web)
        {
            try
            {
                SPEventReceiverDefinitionCollection eventReceivers = web.EventReceivers;
                for (int i = eventReceivers.Count - 1; i >= 0; i--)
                {
                    SPEventReceiverDefinition def = eventReceivers[i];
                    if (def.Assembly.Equals(StrAssemblyName, StringComparison.OrdinalIgnoreCase) &&
                        def.Class.Equals(StrListHandlerClassName, StringComparison.OrdinalIgnoreCase))
                    {
						def.Delete();
                        break;
                    }
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        public static void RemoveItemReceivers(SPList splist)
        {
            try
            {
                SPEventReceiverDefinitionCollection eventReceivers = splist.EventReceivers;
                for (int i = eventReceivers.Count - 1; i >= 0; i--)
                {
                    SPEventReceiverDefinition def = eventReceivers[i];
                    if (def.Assembly.Equals(StrAssemblyName, StringComparison.OrdinalIgnoreCase) &&
                        def.Class.Equals(StrItemHandlerClassName, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            def.Delete();
                        }
                        catch (Exception ex)
                        {
                            ULSLogger.LogError(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                ULSLogger.LogError(exp.ToString());
            }
        }

        public static void AddOrRemoveFeatureForWebApp(SPWebApplication webApp, List<string> selectedSiteIDs, bool bActive)
        {
            if (webApp != null)
            {
                string strAction = bActive ? "Active" : "Deactive";
                Guid rmsGuid = new Guid(StrRmsFeatureGuid);
                foreach (SPSite site in webApp.Sites)
                {
                    try // use try-catch to avoid to block workflow.
                    {
                        using (site)
                        {
                            if (site == null || site.ReadOnly)
                            {
                                continue; // Don't care read-only site colletion.
                            }
                            if (bActive)
                            {
                                if (selectedSiteIDs.Contains(site.ID.ToString()))
                                {
                                    AddOrRemoveFeatureForWebs(site, rmsGuid, true);
                                }
                                else
                                {
                                    AddOrRemoveFeatureForWebs(site, rmsGuid, false);
                                }
                            }
                            else
                            {
                                AddOrRemoveFeatureForWebs(site, rmsGuid, false);
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        ULSLogger.LogError(exp.ToString());
                    }
                }
            }
        }

        public static void AddOrRemoveFeatureForWebs(SPSite site, Guid featureId, bool bAdd)
        {
            if (site == null || featureId == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return;
            }
            foreach (SPWeb web in site.AllWebs)
            {

                if (web == null)
                {
                    continue;
                }
                using (web)
                {
                    bool bOld = web.AllowUnsafeUpdates;
                    web.AllowUnsafeUpdates = true;
                    try // use try-catch to avoid to block workflow.
                    {
                        if (bAdd)
                        {
                            if (web.Features[featureId] == null)
                            {
                                web.Features.Add(featureId, true);
                                web.Update();
                            }
                        }
                        else
                        {
                            if (web.Features[featureId] != null)
                            {
                                web.Features.Remove(featureId, true);
                                web.Update();
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        string enforceAction = bAdd ? "Add" : "Remove";
                        ULSLogger.LogError(exp.ToString());
                    }
                    web.AllowUnsafeUpdates = bOld;
                }
            }
        }

        // Check added/removed features and events is completely.
        public static bool CheckFeaturesAndEvents(SPWebApplication webApp, List<string> selectedSiteIDs, bool bActive, List<string> activedSiteIDs)
        {
            bool bRet = true;
            if (webApp != null)
            {
                Guid rmsGuid = new Guid(StrRmsFeatureGuid);
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    foreach (SPSite site in webApp.Sites)
                    {
                        try // use try-catch to avoid to block workflow.
                        {
                            using (site)
                            {
                                if (site == null || site.ReadOnly)
                                {
                                    continue; // Don't care read-only site colletion.
                                }
                                if (bActive)
                                {
                                    if (selectedSiteIDs.Contains(site.ID.ToString()))
                                    {
                                        if (!CheckFeatureForWebs(webApp, rmsGuid, site, true))
                                        {
                                            bRet = false;
                                            ULSLogger.LogError("Add Feature Failed, Url: " + site.Url);
                                        }
                                        else
                                        {
                                            activedSiteIDs.Add(site.ID.ToString());
                                        }
                                    }
                                    else
                                    {
                                        if (!CheckFeatureForWebs(webApp, rmsGuid, site, false))
                                        {
                                            bRet = false;
                                            activedSiteIDs.Add(site.ID.ToString());
                                            ULSLogger.LogWarning("Remove Feature Failed, Url: " + site.Url);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!CheckFeatureForWebs(webApp, rmsGuid, site, false))
                                    {
                                        bRet = false;
                                        activedSiteIDs.Add(site.ID.ToString());
                                        ULSLogger.LogWarning("Remove Feature Failed, Url: " + site.Url);
                                    }
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            bRet = false;
                            ULSLogger.LogError("Site URL [" + site.Url + "]: " + exp.ToString());
                        }
                    }
                });
            }
            return bRet;
        }

        public static bool CheckFeatureForWebs(SPWebApplication webApp, Guid featureId, SPSite site, bool bActive)
        {
            bool bRet = true;
            if (featureId == null || site == null)
            {
                ULSLogger.LogError("Parameter Null.");
                return false;
            }
            foreach (SPWeb web in site.AllWebs)
            {
                try // use try-catch to avoid to block workflow.
                {
                    using (web)
                    {
                        if (web == null)
                        {
                            continue;
                        }
                        if (bActive)
                        {
                            if (web.Features[featureId] == null)
                            {
                                bRet = false;
                                ULSLogger.LogError("Web Add feature Failed, Url: " + web.Url);
                            }
                        }
                        else
                        {
                            if (web.Features[featureId] != null)
                            {
                                bRet = false;
                                ULSLogger.LogError("Web Remove feature Failed, Url: " + web.Url);
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    ULSLogger.LogError("Site URL [" + web.Url + "]: " + exp.ToString());
                }
            }
            return bRet;
        }
    }
}
