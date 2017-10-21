namespace Fujitsu.AFC.Operations.WindowsService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OperationsProcessServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.OperationsProcessorService = new System.ServiceProcess.ServiceInstaller();
            // 
            // OperationsProcessServiceInstaller
            // 
            this.OperationsProcessServiceInstaller.Password = null;
            this.OperationsProcessServiceInstaller.Username = null;
            // 
            // OperationsProcessorService
            // 
            this.OperationsProcessorService.ServiceName = "Fujitsu.AFC.OperationsProcessor";
            this.OperationsProcessorService.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.OperationsProcessServiceInstaller,
            this.OperationsProcessorService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller OperationsProcessServiceInstaller;
        private System.ServiceProcess.ServiceInstaller OperationsProcessorService;
    }
}