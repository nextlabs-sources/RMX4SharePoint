using System.Collections.Generic;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;


namespace NextLabs.RightsManager.Wizard
{
    public sealed class InstallOptions
    {
        private readonly IList<SPWebApplication> webApplicationTargets;

        public InstallOptions()
        {
            this.webApplicationTargets = new List<SPWebApplication>();
        }

        public IList<SPWebApplication> WebApplicationTargets
        {
            get { return webApplicationTargets; }
        }
	}
}
