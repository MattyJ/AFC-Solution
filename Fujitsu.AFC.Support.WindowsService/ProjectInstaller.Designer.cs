namespace Fujitsu.AFC.Support.WindowsService
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
            this.SupportProcessServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.SupportProcessorService = new System.ServiceProcess.ServiceInstaller();
            // 
            // SupportProcessServiceInstaller
            // 
            this.SupportProcessServiceInstaller.Password = null;
            this.SupportProcessServiceInstaller.Username = null;
            // 
            // SupportProcessorService
            // 
            this.SupportProcessorService.ServiceName = "Fujitsu.AFC.SupportProcessor";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SupportProcessServiceInstaller,
            this.SupportProcessorService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SupportProcessServiceInstaller;
        private System.ServiceProcess.ServiceInstaller SupportProcessorService;
    }
}