namespace OLEBridge.Forms
{
    partial class AppForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppForm));
            this.AppIconTaskbar = new System.Windows.Forms.NotifyIcon(this.components);
            this.AppMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuLog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AppMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // AppIconTaskbar
            // 
            this.AppIconTaskbar.ContextMenuStrip = this.AppMenu;
            this.AppIconTaskbar.Icon = ((System.Drawing.Icon)(resources.GetObject("AppIconTaskbar.Icon")));
            this.AppIconTaskbar.Text = "AppIconTaskbar";
            this.AppIconTaskbar.Visible = true;
            this.AppIconTaskbar.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.AppIconTaskbar_DoubleClick);
            // 
            // AppMenu
            // 
            this.AppMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLog,
            this.toolStripMenuItem1,
            this.mnuExit});
            this.AppMenu.Name = "AppMenu";
            this.AppMenu.Size = new System.Drawing.Size(104, 54);
            // 
            // mnuLog
            // 
            this.mnuLog.Name = "mnuLog";
            this.mnuLog.Size = new System.Drawing.Size(103, 22);
            this.mnuLog.Text = "Show";
            this.mnuLog.Click += new System.EventHandler(this.mnuLog_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(103, 22);
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(96, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 13);
            this.label1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current User:";
            // 
            // AppForm
            // 
            this.ClientSize = new System.Drawing.Size(469, 60);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AppForm";
            this.Text = "OLE Bridge";
            this.AppMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon AppIconTaskbar;
        private System.Windows.Forms.ContextMenuStrip AppMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuLog;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        //private System.Windows.Forms.ToolStripMenuItem showLogToolStripMenuItem;
        //private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        //private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        //private System.Windows.Forms.NotifyIcon notifyIcon1;
        //private System.Windows.Forms.ContextMenuStrip rip1;
    }
}