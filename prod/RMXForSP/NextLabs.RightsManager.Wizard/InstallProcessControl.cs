using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using System.IO;
using NextLabs.RightsManager.Wizard.Resources;
using System.Security;

namespace NextLabs.RightsManager.Wizard
{
    public partial class InstallProcessControl : InstallerControl
    {
        private static readonly MessageCollector log = new MessageCollector(LogManager.GetLogger());
        private static readonly TimeSpan JobTimeout = TimeSpan.FromMinutes(15);

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private CommandList executeCommands;
        private CommandList rollbackCommands;
        private int nextCommand;
        private bool completed;
        private bool requestCancel;
        private int errors;
        private int rollbackErrors;

        public InstallProcessControl()
        {
            InitializeComponent();

            errorPictureBox.Visible = false;
            errorDetailsTextBox.Visible = false;

            this.Load += new EventHandler(InstallProcessControl_Load);
        }

        #region Event Handlers
        private void InstallProcessControl_Load(object sender, EventArgs e)
        {
            switch (Form.Operation)
            {
                case InstallOperation.Install:
                    Form.SetTitle(CommonUIStrings.installTitle);
                    Form.SetSubTitle(InstallConfiguration.FormatString(CommonUIStrings.installSubTitle));
                    break;

                case InstallOperation.Uninstall:
                    Form.SetTitle(CommonUIStrings.uninstallTitle);
                    Form.SetSubTitle(InstallConfiguration.FormatString(CommonUIStrings.uninstallSubTitle));
                    break;
            }
            Form.PrevButton.Enabled = false;
            Form.NextButton.Enabled = false;
        }

        private void TimerEventInstall(Object myObject, EventArgs myEventArgs)
        {
            timer.Stop();
            if (requestCancel)
            {
                descriptionLabel.Text = Resources.CommonUIStrings.descriptionLabelTextOperationCanceled;
                InitiateRollback();
            }
            else if (nextCommand < executeCommands.Count)
            {
                Command command = executeCommands[nextCommand];
                bool ret = true;
                try
                {
                    ret = command.Execute();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("***TimerEventInstall Exception: " + ex);
                    if (ex.Message.Contains("Access to the path") && ex.Message.Contains("The removal of this file"))
                    {
                        ret = true;
                    }
                    else
                    {
                        log.Error(CommonUIStrings.logError);
                        log.Error(ex.Message, ex);

                        errors++;
                        errorPictureBox.Visible = true;
                        errorDetailsTextBox.Visible = true;
                        errorDetailsTextBox.Text = ex.Message;

                        descriptionLabel.Text = Resources.CommonUIStrings.descriptionLabelTextErrorsDetected;
                        InitiateRollback();
                        return;
                    }
                }
                if (ret)
                {
                    ++nextCommand;
                    progressBar.PerformStep();

                    if (nextCommand < executeCommands.Count)
                    {
                        descriptionLabel.Text = executeCommands[nextCommand].Description;
                    }
                }
                else
                {
                    descriptionLabel.Text = executeCommands[nextCommand].Description;
                }
                timer.Start();
            }
            else
            {
                descriptionLabel.Text = Resources.CommonUIStrings.descriptionLabelTextSuccess;
                HandleCompletion();
            }
        }

        private void TimerEventRollback(Object myObject, EventArgs myEventArgs)
        {
            timer.Stop();
            if (nextCommand < rollbackCommands.Count)
            {
                try
                {
                    Command command = rollbackCommands[nextCommand];
                    if (command.Rollback())
                    {
                        nextCommand++;
                        progressBar.PerformStep();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(CommonUIStrings.logError);
                    log.Error(ex.Message, ex);

                    rollbackErrors++;
                    nextCommand++;
                    progressBar.PerformStep();
                }
                timer.Start();
            }
            else
            {
                if (rollbackErrors == 0)
                {
                    progressBar.Step = 1;
                    progressBar.Maximum = 1;
                    progressBar.Value = 1;
                    progressBar.PerformStep();
                    descriptionLabel.Text = Resources.CommonUIStrings.descriptionLabelTextRollbackSuccess;
                }
                else
                {
                    descriptionLabel.Text = string.Format(Resources.CommonUIStrings.descriptionLabelTextRollbackError, rollbackErrors);
                }
                HandleRollBackCompletion();
            }
        }
        #endregion

        #region Protected Methods
        protected internal override void RequestCancel()
        {
            if (completed)
            {
                base.RequestCancel();
            }
            else
            {
                requestCancel = true;
                Form.AbortButton.Enabled = false;
                Form.NextButton.Enabled = false;
            }
        }

        protected internal override void Open(InstallOptions options)
        {
            executeCommands = new CommandList();
            rollbackCommands = new CommandList();
            nextCommand = 0;
            switch (Form.Operation)
            {
                case InstallOperation.Install:
                    executeCommands.Add(new AddSolutionCommand(this));
                    executeCommands.Add(new CreateDeploymentJobCommand(this, options.WebApplicationTargets));
                    executeCommands.Add(new WaitForJobCompletionCommand(this, CommonUIStrings.waitForSolutionDeployment));
                    executeCommands.Add(new RegisterVersionNumberCommand(this, options.WebApplicationTargets));

                    for (int i = executeCommands.Count - 1; i <= 0; i--)
                    {
                        rollbackCommands.Add(executeCommands[i]);
                    }
                    break;

                case InstallOperation.Uninstall:
                    executeCommands.Add(new CreateRetractionJobCommand(this));
                    executeCommands.Add(new WaitForJobCompletionCommand(this, CommonUIStrings.waitForSolutionRetraction));
                    executeCommands.Add(new RemoveSolutionCommand(this));
                    executeCommands.Add(new UnregisterVersionNumberCommand(this));
                    break;
            }

            progressBar.Maximum = executeCommands.Count;
            descriptionLabel.Text = executeCommands[0].Description;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(TimerEventInstall);
            timer.Start();
        }

        #endregion

        #region Private Methods

        private void HandleCompletion()
        {
            completed = true;

            Form.NextButton.Enabled = true;
            Form.AbortButton.Text = CommonUIStrings.abortButtonText;
            Form.AbortButton.Enabled = true;
            CompletionControl nextControl = new CompletionControl();
            foreach (string message in log.Messages)
            {
                nextControl.Details += message + "\r\n";
            }

            switch (Form.Operation)
            {
                case InstallOperation.Install:
                    nextControl.Title = errors == 0 ? CommonUIStrings.installSuccess : CommonUIStrings.installError;
                    break;

                case InstallOperation.Uninstall:
                    nextControl.Title = errors == 0 ? CommonUIStrings.uninstallSuccess : CommonUIStrings.uninstallError;
                    break;
            }

            Form.ContentControls.Add(nextControl);
        }

        private void HandleRollBackCompletion()
        {
            completed = true;
            Form.NextButton.Enabled = false;
            Form.AbortButton.Text = CommonUIStrings.abortButtonText;
            Form.AbortButton.Enabled = true;
        }

        private void InitiateRollback()
        {
            Form.AbortButton.Enabled = false;
            progressBar.Maximum = rollbackCommands.Count;
            progressBar.Value = rollbackCommands.Count;
            nextCommand = 0;
            rollbackErrors = 0;
            progressBar.Step = -1;

            // Create and start new timer.
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(TimerEventRollback);
            timer.Start();
        }

        private bool IsSolutionRenamed()
        {
            SPFarm farm = SPFarm.Local;
            SPSolution solution = farm.Solutions[InstallConfiguration.SolutionId];
            if (solution == null) return false;
            string filename = InstallConfiguration.SolutionFile;
            FileInfo solutionFileInfo = new FileInfo(filename);
            return !solution.Name.Equals(solutionFileInfo.Name, StringComparison.OrdinalIgnoreCase);
        }

        private Collection<SPWebApplication> GetDeployedApplications()
        {
            SPFarm farm = SPFarm.Local;
            SPSolution solution = farm.Solutions[InstallConfiguration.SolutionId];
            if (solution.ContainsWebApplicationResource)
            {
                return solution.DeployedWebApplications;
            }
            return null;
        }

        #endregion

        #region Command Classes

        /// <summary>
        /// The base class of all installation commands.
        /// </summary>
        private abstract class Command
        {
            private readonly InstallProcessControl parent;

            protected Command(InstallProcessControl parent)
            {
                this.parent = parent;
            }

            internal InstallProcessControl Parent
            {
                get { return parent; }
            }

            internal abstract string Description { get; }

            protected internal virtual bool Execute() { return true; }

            protected internal virtual bool Rollback() { return true; }
        }

        private class CommandList : List<Command>
        {
        }

        /// <summary>
        /// The base class of all SharePoint solution related commands.
        /// </summary>
        private abstract class SolutionCommand : Command
        {
            protected SolutionCommand(InstallProcessControl parent) : base(parent) { }

            protected void RemoveSolution()
            {
                try
                {
                    SPFarm farm = SPFarm.Local;
                    SPSolution solution = farm.Solutions[InstallConfiguration.SolutionId];
                    if (solution != null)
                    {
                        if (!solution.Deployed)
                        {
                            solution.Delete();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Command for adding the SharePoint solution.
        /// </summary>
        private class AddSolutionCommand : SolutionCommand
        {
            internal AddSolutionCommand(InstallProcessControl parent) : base(parent)
            {
            }

            internal override string Description
            {
                get
                {
                    return CommonUIStrings.addSolutionCommand;
                }
            }

            protected internal override bool Execute()
            {
                string filename = InstallConfiguration.SolutionFile;
                if (String.IsNullOrEmpty(filename))
                {
                    throw new InstallException(CommonUIStrings.installExceptionConfigurationNoWsp);
                }
                try
                {
                    SPFarm farm = SPFarm.Local;
                    SPSolution solution = farm.Solutions.Add(filename);
                    return true;
                }
                catch (SecurityException ex)
                {
                    string message = CommonUIStrings.addSolutionAccessError;
                    throw new InstallException(message, ex);
                }
                catch (IOException ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
                catch (ArgumentException ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
                catch (SqlException ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
            }

            protected internal override bool Rollback()
            {
                RemoveSolution();
                return true;
            }
        }

        /// <summary>
        /// Command for removing the SharePoint solution.
        /// </summary>
        private class RemoveSolutionCommand : SolutionCommand
        {
            internal RemoveSolutionCommand(InstallProcessControl parent) : base(parent) { }

            internal override string Description
            {
                get
                {
                    return CommonUIStrings.removeSolutionCommand;
                }
            }

            protected internal override bool Execute()
            {
                RemoveSolution();
                return true;
            }
        }

        private abstract class JobCommand : Command
        {
            protected JobCommand(InstallProcessControl parent) : base(parent) { }

            protected static void RemoveExistingJob(SPSolution solution)
            {
                if (solution.JobStatus == SPRunningJobStatus.Initialized)
                {
                    throw new InstallException(CommonUIStrings.installExceptionDuplicateJob);
                }

                SPJobDefinition jobDefinition = GetSolutionJob(solution);
                if (jobDefinition != null)
                {
                    jobDefinition.Delete();
                    Thread.Sleep(500);
                }
            }

            private static SPJobDefinition GetSolutionJob(SPSolution solution)
            {
                SPFarm localFarm = SPFarm.Local;
                SPTimerService service = localFarm.TimerService;
                foreach (SPJobDefinition definition in service.JobDefinitions)
                {
                    if (definition.Title != null && definition.Title.Contains(solution.Name))
                    {
                        return definition;
                    }
                }
                return null;
            }

            protected static DateTime GetImmediateJobTime()
            {
                return DateTime.Now - TimeSpan.FromDays(1);
            }
        }

        /// <summary>
        /// Command for creating a deployment job.
        /// </summary>
        private class CreateDeploymentJobCommand : JobCommand
        {
            private readonly Collection<SPWebApplication> m_obWebApplicationTargets = null;

            internal CreateDeploymentJobCommand(InstallProcessControl parent, IList<SPWebApplication> obWebApplicationTargets) : base(parent)
            {
				if (null == obWebApplicationTargets)
				{
                    m_obWebApplicationTargets = null;
                }
				else
				{
                    m_obWebApplicationTargets = new Collection<SPWebApplication>();
					foreach (SPWebApplication application in obWebApplicationTargets)
					{
                        m_obWebApplicationTargets.Add(application);
					}				
				}
            }

            internal override string Description
            {
                get
                {
                    return CommonUIStrings.createDeploymentJobCommand;
                }
            }

            protected internal override bool Execute()
            {
                try
                {
                    SPSolution installedSolution = SPFarm.Local.Solutions[InstallConfiguration.SolutionId];
                    //
                    // Remove existing job, if any. 
                    //
                    if (installedSolution.JobExists)
                    {
                        RemoveExistingJob(installedSolution);
                    }

                    log.Info("***** SOLUTION DEPLOYMENT *****");
                    // ContainsWebApplicationResource: if the solution support to deploy to specify web applications, user can select we applications to deploy
                    if (installedSolution.ContainsWebApplicationResource && m_obWebApplicationTargets != null && m_obWebApplicationTargets.Count > 0)
					{
                        log.Info("Deployment applications:");
                        foreach (SPWebApplication spDeployApplication in m_obWebApplicationTargets)
                        {
                            log.Info("\t" + spDeployApplication.Name);
                        }
                        installedSolution.Deploy(GetImmediateJobTime(), true, m_obWebApplicationTargets, true);
                    }
                    else
					{
						installedSolution.Deploy(GetImmediateJobTime(), true, true);
    				}
                    return true;
                }
                catch (SPException ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
                catch (SqlException ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
            }

            protected internal override bool Rollback()
            {
                SPSolution installedSolution = SPFarm.Local.Solutions[InstallConfiguration.SolutionId];
                if (installedSolution != null)
                {
                    //
                    // Remove existing job, if any. 
                    //
                    if (installedSolution.JobExists)
                    {
                        RemoveExistingJob(installedSolution);
                    }

                    log.Info("***** SOLUTION RETRACTION *****");
                    installedSolution.Retract(GetImmediateJobTime());
                }
                return true;
            }
        }

        /// <summary>
        /// Command for creating a retraction job.
        /// </summary>
        private class CreateRetractionJobCommand : JobCommand
        {
            internal CreateRetractionJobCommand(InstallProcessControl parent) : base(parent)
            {
            }

            internal override string Description
            {
                get
                {
                    return CommonUIStrings.createRetractionJobCommand;
                }
            }

            protected internal override bool Execute()
            {
                try
                {
                    SPSolution installedSolution = SPFarm.Local.Solutions[InstallConfiguration.SolutionId];

                    //
                    // Remove existing job, if any. 
                    //
                    if (installedSolution.JobExists)
                    {
                        RemoveExistingJob(installedSolution);
                    }

                    if (installedSolution.Deployed)
                    {
                        // Remove Nxl icon and unregistry dll
                        RegNxlConfig.UnRegNxl();

                        log.Info(CommonUIStrings.logRetract);
                        installedSolution.Retract(GetImmediateJobTime());
                    }
                    return true;
                }

                catch (SqlException ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
            }
        }

        private class WaitForJobCompletionCommand : Command
        {
            private bool m_bIsInstallCase = false;
            private int m_nWaitTimes = 0;
            private readonly string[] m_RuningFlags = new string[] { " --", " \\", " |", " /" };
            private readonly string descriptionBaseInfo;
            private string description;
            private DateTime startTime;
            private bool first = true;

            internal WaitForJobCompletionCommand(InstallProcessControl parent, string description) : base(parent)
            {
                descriptionBaseInfo = description;
                description = descriptionBaseInfo;

                if (descriptionBaseInfo == CommonUIStrings.waitForSolutionDeployment)
                {
                    m_bIsInstallCase = true;
                }
            }

            internal override string Description
            {
                get
                {
                    return description;
                }
            }

            protected internal override bool Execute()
            {
                try
                {
                    ++m_nWaitTimes;

                    SPSolution installedSolution = SPFarm.Local.Solutions[InstallConfiguration.SolutionId];
                    if (first)
                    {
                        if (!installedSolution.JobExists) return true;
                        startTime = DateTime.Now;
                        first = false;
                    }
                    //
                    // Wait for job to end
                    //
                    if (installedSolution.JobExists)
                    {
                        //if (DateTime.Now > startTime.Add(JobTimeout))
                        //{
                        //    throw new InstallException(CommonUIStrings.installExceptionTimeout);
                        //}
                        
                        description = descriptionBaseInfo + m_RuningFlags[m_nWaitTimes % m_RuningFlags.Length];
                        return false;
                    }
                    else
                    {
                        log.Info(installedSolution.LastOperationDetails);
                        if (m_bIsInstallCase)
                        {
                            if (!installedSolution.Deployed)
                            {
                                log.Error("Solution is deployed failed, please restart IIS and try again.");
                            }
                        } 
                        else
                        {
                            if (installedSolution.Deployed)
                            {
                                log.Error("Solution is retracted failed, please restart IIS and try again.");
                            }
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new InstallException(ex.Message, ex);
                }
            }

            protected internal override bool Rollback()
            {
                SPSolution installedSolution = SPFarm.Local.Solutions[InstallConfiguration.SolutionId];

                //
                // Wait for job to end
                //
                if (installedSolution != null)
                {
                    if (installedSolution.JobExists)
                    {
                        if (DateTime.Now > startTime.Add(JobTimeout))
                        {
                            throw new InstallException(CommonUIStrings.installExceptionTimeout);
                        }
                        return false;
                    }
                    else
                    {
                        log.Info(installedSolution.LastOperationDetails);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Command that registers the version number of a solution.
        /// </summary>
        private class RegisterVersionNumberCommand : Command
        {
            private Version oldVersion;
            private IList<SPWebApplication> webApps;

            internal RegisterVersionNumberCommand(InstallProcessControl parent, IList<SPWebApplication> applications) : base(parent)
            {
                webApps = applications;
            }
            internal RegisterVersionNumberCommand(InstallProcessControl parent) : base(parent)
            {
                webApps = null;
            }

            internal override string Description
            {
                get
                {
                    return CommonUIStrings.registerVersionNumberCommand;
                }
            }

            protected internal override bool Execute()
            {
                oldVersion = InstallConfiguration.InstalledVersion;
                InstallConfiguration.InstalledVersion = InstallConfiguration.SolutionVersion;
                
                // Add Nxl icon and registry dll
                RegNxlConfig.RegNxl();

                // Store the selected web applications
                if (webApps != null && webApps.Count > 0)
                {
                    List<string> listWebAppIds = new List<string>();
                    foreach (SPWebApplication webApp in webApps)
                    {
                        if (!webApp.IsAdministrationWebApplication)
                        {
                            listWebAppIds.Add(webApp.Id.ToString());
                        }
                    }
                    if (listWebAppIds.Count > 0)
                    {
                        string strIds = string.Join(";", listWebAppIds);
                        SPSecurity.RunWithElevatedPrivileges(delegate ()
                        {
                            SPWebApplication adminApp = SPAdministrationWebApplication.Local;
                            adminApp.Properties["DeployedWebApps"] = strIds;
                            adminApp.Update();
                        });
                    }
                }
                return true;
            }

            protected internal override bool Rollback()
            {
                InstallConfiguration.InstalledVersion = oldVersion;
                return true;
            }
        }

        /// <summary>
        /// Command that unregisters the version number of a solution.
        /// </summary>
        private class UnregisterVersionNumberCommand : Command
        {
            internal UnregisterVersionNumberCommand(InstallProcessControl parent) : base(parent) { }

            internal override string Description
            {
                get
                {
                    return CommonUIStrings.unregisterVersionNumberCommand;
                }
            }

            protected internal override bool Execute()
            {
                InstallConfiguration.InstalledVersion = null;
                return true;
            }
        }
        #endregion

        #region ILog Wrapper

        private class MessageList : List<string>
        {
        }

        private class MessageCollector : ILog
        {
            private readonly ILog wrappee;
            private readonly MessageList messages = new MessageList();

            internal MessageCollector(ILog wrappee)
            {
                this.wrappee = wrappee;
            }

            public MessageList Messages
            {
                get { return messages; }
            }

            public void Info(object message)
            {
                messages.Add(message.ToString());
                wrappee.Info(message);
            }

            public void Info(object message, Exception t)
            {
                messages.Add(message.ToString());
                messages.Add(t.ToString());
                wrappee.Info(message, t);
            }

            public void Warn(object message)
            {
                wrappee.Warn(message);
            }

            public void Warn(object message, Exception t)
            {
                wrappee.Warn(message, t);
            }

            public void Error(object message)
            {
                messages.Add(message.ToString());
                wrappee.Error(message);
            }

            public void Error(object message, Exception t)
            {
                messages.Add(message.ToString());
                messages.Add(t.ToString());
                wrappee.Error(message, t);
            }

            public void Fatal(object message)
            {
                wrappee.Fatal(message);
            }

            public void Fatal(object message, Exception t)
            {
                wrappee.Fatal(message, t);
            }
        }
#endregion
    }
}
