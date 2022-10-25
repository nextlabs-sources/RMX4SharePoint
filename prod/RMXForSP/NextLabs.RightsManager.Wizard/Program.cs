using System;
using System.Windows.Forms;


namespace NextLabs.RightsManager.Wizard
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0].Equals("/reg", StringComparison.OrdinalIgnoreCase))
                {
                    RegNxlConfig.RegNxl();
                }
                else if (args[0].Equals("/unreg", StringComparison.OrdinalIgnoreCase))
                {
                    RegNxlConfig.UnRegNxl();
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                InstallerForm form = new InstallerForm();
                form.Text = InstallConfiguration.FormatString("{SolutionTitle}");
                form.ContentControls.Add(CreateWelcomeControl());
                form.ContentControls.Add(CreateSystemCheckControl());

                Application.Run(form);
            }
        }

        private static InstallerControl CreateWelcomeControl()
        {
            WelcomeControl control = new WelcomeControl();
            control.Title = InstallConfiguration.FormatString(Resources.CommonUIStrings.controlTitleWelcome);
            control.SubTitle = InstallConfiguration.FormatString(Resources.CommonUIStrings.controlSubTitleWelcome);
            return control;
        }

        private static InstallerControl CreateSystemCheckControl()
        {
            SystemCheckControl control = new SystemCheckControl();
            control.Title = Resources.CommonUIStrings.controlTitleSystemCheck;
            if (SystemCheckControl.IsSolutionDeployed())
            {
                control.SubTitle = InstallConfiguration.FormatString(Resources.CommonUIStrings.controlSubTitleOptionsUninstall);
            }
            else
            {
                control.SubTitle = InstallConfiguration.FormatString(Resources.CommonUIStrings.controlSubTitleSystemCheck);
            }
            return control;
        }

        internal static InstallerControl CreateUpgradeControl()
        {
            UpgradeControl control = new UpgradeControl();
            control.Title = Resources.CommonUIStrings.controlTitleRemove;
            control.SubTitle = Resources.CommonUIStrings.controlSubTitleSelectOperation;
            return control;
        }
        
        internal static InstallerControl CreateDeploymentTargetsControl()
        {
            InstallerControl control = null;
            control = new DeploymentTargetsControl();
            control.Title = Resources.CommonUIStrings.controlTitleFarmDeployment;
            control.SubTitle = Resources.CommonUIStrings.controlSubTitleFarmDeployment;
            return control;
        }

        internal static InstallProcessControl CreateProcessControl()
        {
            InstallProcessControl control = new InstallProcessControl();
            control.Title = Resources.CommonUIStrings.controlTitleInstalling;
            control.SubTitle = Resources.CommonUIStrings.controlSubTitleInstalling;
            return control;
        }

    }
}
