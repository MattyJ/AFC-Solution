namespace Fujitsu.AFC.Provisioning.WindowsService
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
            this.ProvisioningProcessServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ProvisioningProcessorService = new System.ServiceProcess.ServiceInstaller();
            // 
            // ProvisioningProcessServiceInstaller
            // 
            this.ProvisioningProcessServiceInstaller.Password = null;
            this.ProvisioningProcessServiceInstaller.Username = null;
            // 
            // ProvisioningProcessorService
            // 
            this.ProvisioningProcessorService.ServiceName = "Fujitsu.AFC.ProvisioningProcessor";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ProvisioningProcessServiceInstaller,
            this.ProvisioningProcessorService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ProvisioningProcessServiceInstaller;
        private System.ServiceProcess.ServiceInstaller ProvisioningProcessorService;
    }
}