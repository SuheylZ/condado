namespace SalesTool.Service.Installer
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
            this.SQSalesToolProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.SCSalesToolInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // SQSalesToolProcessInstaller
            // 
            this.SQSalesToolProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.SQSalesToolProcessInstaller.Password = null;
            this.SQSalesToolProcessInstaller.Username = null;
            // 
            // SCSalesToolInstaller
            // 
            this.SCSalesToolInstaller.Description = "This service hosts various tasks that are executed periodically. If this service " +
    "is stopped or disabled then these tasks will not be executed";
            this.SCSalesToolInstaller.DisplayName = "Condado Sales Tool ";
            this.SCSalesToolInstaller.ServiceName = "SalesTool.Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SQSalesToolProcessInstaller,
            this.SCSalesToolInstaller});
            this.Committed += new System.Configuration.Install.InstallEventHandler(this.ProjectInstaller_Committed);

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SQSalesToolProcessInstaller;
        private System.ServiceProcess.ServiceInstaller SCSalesToolInstaller;
    }
}