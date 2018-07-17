namespace TMT.Enforcement.iAutoLog
{
    partial class StatsReport
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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.ReportSourceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportViewerMain = new Microsoft.Reporting.WinForms.ReportViewer();
            ((System.ComponentModel.ISupportInitialize)(this.ReportSourceBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // ReportSourceBindingSource
            // 
            this.ReportSourceBindingSource.DataSource = typeof(TMT.Enforcement.iAutoLog.ReportSource);
            // 
            // reportViewerMain
            // 
            this.reportViewerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = this.ReportSourceBindingSource;
            this.reportViewerMain.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewerMain.LocalReport.ReportEmbeddedResource = "TMT.iAutoLog.AutoLogStatsReport.rdlc";
            this.reportViewerMain.Location = new System.Drawing.Point(0, 0);
            this.reportViewerMain.Name = "reportViewerMain";
            this.reportViewerMain.Size = new System.Drawing.Size(984, 712);
            this.reportViewerMain.TabIndex = 0;
            // 
            // StatsReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 712);
            this.Controls.Add(this.reportViewerMain);
            this.Name = "StatsReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "StatsReport";
            this.Load += new System.EventHandler(this.StatsReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ReportSourceBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource ReportSourceBindingSource;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewerMain;
    }
}