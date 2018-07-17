namespace TMT.Enforcement.iLog
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDirectory = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.treePanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lsvSessions = new System.Windows.Forms.ListView();
            this.columnHeaderGroupingType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCreateDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCamera = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderHasErrors = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lsvFiles = new System.Windows.Forms.ListView();
            this.columnHeaderFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOffenceDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderZone = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSpeed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderError = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderErrorDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSelect = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnLog = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblLogin = new System.Windows.Forms.Label();
            this.btnLoginLogout = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMachineName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.btnViewStats = new System.Windows.Forms.Button();
            this.dateTimePickerFilterAfter = new System.Windows.Forms.DateTimePicker();
            this.chkFilter = new System.Windows.Forms.CheckBox();
            this.btnZoom = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnViewReport = new System.Windows.Forms.Button();
            this.btnSyncFilm = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.chkSplitter = new System.Windows.Forms.CheckBox();
            this.chkAutoSizeToFrame = new System.Windows.Forms.CheckBox();
            this.btnFieldSheet = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.panel1.Controls.Add(this.btnDirectory);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtDirectory);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(293, 57);
            this.panel1.TabIndex = 2;
            // 
            // btnDirectory
            // 
            this.btnDirectory.Location = new System.Drawing.Point(258, 27);
            this.btnDirectory.Name = "btnDirectory";
            this.btnDirectory.Size = new System.Drawing.Size(30, 21);
            this.btnDirectory.TabIndex = 1;
            this.btnDirectory.Text = "...";
            this.btnDirectory.Click += new System.EventHandler(this.btnDirectory_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Browse: Camera Images Parent Directory:";
            // 
            // txtDirectory
            // 
            this.txtDirectory.Location = new System.Drawing.Point(9, 27);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(240, 21);
            this.txtDirectory.TabIndex = 0;
            this.txtDirectory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDirectory_KeyDown);
            // 
            // treePanel
            // 
            this.treePanel.Location = new System.Drawing.Point(3, 98);
            this.treePanel.Name = "treePanel";
            this.treePanel.Size = new System.Drawing.Size(293, 256);
            this.treePanel.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 361);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Sessions Found from Selection Above:";
            // 
            // lsvSessions
            // 
            this.lsvSessions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lsvSessions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lsvSessions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderGroupingType,
            this.columnHeaderCreateDate,
            this.columnHeaderCamera,
            this.columnHeaderHasErrors});
            this.lsvSessions.FullRowSelect = true;
            this.lsvSessions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lsvSessions.HideSelection = false;
            this.lsvSessions.Location = new System.Drawing.Point(3, 381);
            this.lsvSessions.Name = "lsvSessions";
            this.lsvSessions.Size = new System.Drawing.Size(293, 310);
            this.lsvSessions.TabIndex = 7;
            this.lsvSessions.UseCompatibleStateImageBehavior = false;
            this.lsvSessions.View = System.Windows.Forms.View.Details;
            this.lsvSessions.SelectedIndexChanged += new System.EventHandler(this.lsvSessions_SelectedIndexChanged);
            // 
            // columnHeaderGroupingType
            // 
            this.columnHeaderGroupingType.Text = "Grouping";
            this.columnHeaderGroupingType.Width = 150;
            // 
            // columnHeaderCreateDate
            // 
            this.columnHeaderCreateDate.Text = "Date";
            this.columnHeaderCreateDate.Width = 100;
            // 
            // columnHeaderCamera
            // 
            this.columnHeaderCamera.Text = "Camera";
            this.columnHeaderCamera.Width = 125;
            // 
            // columnHeaderHasErrors
            // 
            this.columnHeaderHasErrors.Text = "HasErrors";
            // 
            // lsvFiles
            // 
            this.lsvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lsvFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lsvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFileName,
            this.columnHeaderOffenceDate,
            this.columnHeaderZone,
            this.columnHeaderSpeed,
            this.columnHeaderError,
            this.columnHeaderErrorDesc});
            this.lsvFiles.FullRowSelect = true;
            this.lsvFiles.HideSelection = false;
            this.lsvFiles.Location = new System.Drawing.Point(298, 30);
            this.lsvFiles.MultiSelect = false;
            this.lsvFiles.Name = "lsvFiles";
            this.lsvFiles.Size = new System.Drawing.Size(322, 661);
            this.lsvFiles.TabIndex = 9;
            this.lsvFiles.UseCompatibleStateImageBehavior = false;
            this.lsvFiles.View = System.Windows.Forms.View.Details;
            this.lsvFiles.SelectedIndexChanged += new System.EventHandler(this.lsvFiles_SelectedIndexChanged);
            // 
            // columnHeaderFileName
            // 
            this.columnHeaderFileName.Text = "FileName";
            this.columnHeaderFileName.Width = 75;
            // 
            // columnHeaderOffenceDate
            // 
            this.columnHeaderOffenceDate.Text = "Date";
            this.columnHeaderOffenceDate.Width = 125;
            // 
            // columnHeaderZone
            // 
            this.columnHeaderZone.Text = "Zone";
            this.columnHeaderZone.Width = 45;
            // 
            // columnHeaderSpeed
            // 
            this.columnHeaderSpeed.Text = "Speed";
            this.columnHeaderSpeed.Width = 45;
            // 
            // columnHeaderError
            // 
            this.columnHeaderError.Text = "Errors";
            this.columnHeaderError.Width = 45;
            // 
            // columnHeaderErrorDesc
            // 
            this.columnHeaderErrorDesc.Text = "Error Description";
            this.columnHeaderErrorDesc.Width = 500;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(223, 65);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(73, 27);
            this.btnSelect.TabIndex = 6;
            this.btnSelect.Text = "Apply";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.ForeColor = System.Drawing.Color.Red;
            this.lblInfo.Location = new System.Drawing.Point(505, 702);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(103, 16);
            this.lblInfo.TabIndex = 16;
            this.lblInfo.Text = "Busy...Be Patient";
            // 
            // btnLog
            // 
            this.btnLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLog.Location = new System.Drawing.Point(408, 697);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(90, 27);
            this.btnLog.TabIndex = 10;
            this.btnLog.Text = "Log Film";
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(122, 69);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(155, 21);
            this.txtPassword.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLogin);
            this.groupBox1.Controls.Add(this.btnLoginLogout);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtMachineName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Location = new System.Drawing.Point(622, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 125);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "iLog User Detail";
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.ForeColor = System.Drawing.Color.Red;
            this.lblLogin.Location = new System.Drawing.Point(6, 100);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(131, 13);
            this.lblLogin.TabIndex = 26;
            this.lblLogin.Text = "Login to Log to Database!";
            // 
            // btnLoginLogout
            // 
            this.btnLoginLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoginLogout.Location = new System.Drawing.Point(205, 93);
            this.btnLoginLogout.Name = "btnLoginLogout";
            this.btnLoginLogout.Size = new System.Drawing.Size(73, 27);
            this.btnLoginLogout.TabIndex = 5;
            this.btnLoginLogout.Text = "Login";
            this.btnLoginLogout.Click += new System.EventHandler(this.btnLoginLogout_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Machine Name:";
            // 
            // txtMachineName
            // 
            this.txtMachineName.Enabled = false;
            this.txtMachineName.Location = new System.Drawing.Point(122, 15);
            this.txtMachineName.Name = "txtMachineName";
            this.txtMachineName.Size = new System.Drawing.Size(155, 21);
            this.txtMachineName.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "User Name:";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(122, 42);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(155, 21);
            this.txtUserName.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Password:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = global::TMT.Enforcement.iLog.Properties.Resources.Legend;
            this.pictureBox1.Image = global::TMT.Enforcement.iLog.Properties.Resources.Legend;
            this.pictureBox1.Location = new System.Drawing.Point(622, 135);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(281, 60);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(904, 9);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblVersion.Size = new System.Drawing.Size(93, 13);
            this.lblVersion.TabIndex = 22;
            this.lblVersion.Text = "Version x.x.x.xxx";
            this.toolTip.SetToolTip(this.lblVersion, "Version");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(309, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Decrypted Files for Session:";
            // 
            // btnViewStats
            // 
            this.btnViewStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewStats.Location = new System.Drawing.Point(3, 697);
            this.btnViewStats.Name = "btnViewStats";
            this.btnViewStats.Size = new System.Drawing.Size(104, 27);
            this.btnViewStats.TabIndex = 23;
            this.btnViewStats.Text = "Stats File";
            this.btnViewStats.Click += new System.EventHandler(this.btnViewStats_Click);
            // 
            // dateTimePickerFilterAfter
            // 
            this.dateTimePickerFilterAfter.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerFilterAfter.Location = new System.Drawing.Point(7, 68);
            this.dateTimePickerFilterAfter.Name = "dateTimePickerFilterAfter";
            this.dateTimePickerFilterAfter.Size = new System.Drawing.Size(128, 21);
            this.dateTimePickerFilterAfter.TabIndex = 24;
            // 
            // chkFilter
            // 
            this.chkFilter.AutoSize = true;
            this.chkFilter.Checked = true;
            this.chkFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFilter.Location = new System.Drawing.Point(141, 72);
            this.chkFilter.Name = "chkFilter";
            this.chkFilter.Size = new System.Drawing.Size(50, 17);
            this.chkFilter.TabIndex = 25;
            this.chkFilter.Text = "Filter";
            this.chkFilter.UseVisualStyleBackColor = true;
            this.chkFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // btnZoom
            // 
            this.btnZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoom.Location = new System.Drawing.Point(948, 697);
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.Size = new System.Drawing.Size(73, 27);
            this.btnZoom.TabIndex = 26;
            this.btnZoom.Text = "Zoom";
            this.btnZoom.Click += new System.EventHandler(this.btnZoom_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(903, 179);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "TO PAN - Use Arrows";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnViewReport
            // 
            this.btnViewReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewReport.Location = new System.Drawing.Point(111, 697);
            this.btnViewReport.Name = "btnViewReport";
            this.btnViewReport.Size = new System.Drawing.Size(104, 27);
            this.btnViewReport.TabIndex = 28;
            this.btnViewReport.Text = "Image Report";
            this.btnViewReport.Click += new System.EventHandler(this.btnViewReport_Click);
            // 
            // btnSyncFilm
            // 
            this.btnSyncFilm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSyncFilm.Location = new System.Drawing.Point(313, 697);
            this.btnSyncFilm.Name = "btnSyncFilm";
            this.btnSyncFilm.Size = new System.Drawing.Size(90, 27);
            this.btnSyncFilm.TabIndex = 29;
            this.btnSyncFilm.Text = "Sync Film";
            this.btnSyncFilm.Click += new System.EventHandler(this.btnImageReport_Click);
            // 
            // contentPanel
            // 
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.BackColor = System.Drawing.Color.Transparent;
            this.contentPanel.Location = new System.Drawing.Point(622, 200);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(399, 491);
            this.contentPanel.TabIndex = 37;
            // 
            // chkSplitter
            // 
            this.chkSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSplitter.AutoSize = true;
            this.chkSplitter.BackColor = System.Drawing.Color.Transparent;
            this.chkSplitter.Location = new System.Drawing.Point(904, 135);
            this.chkSplitter.Name = "chkSplitter";
            this.chkSplitter.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkSplitter.Size = new System.Drawing.Size(81, 17);
            this.chkSplitter.TabIndex = 36;
            this.chkSplitter.Text = "Use Splitter";
            this.chkSplitter.UseVisualStyleBackColor = false;
            this.chkSplitter.CheckedChanged += new System.EventHandler(this.chkSplitter_CheckedChanged);
            // 
            // chkAutoSizeToFrame
            // 
            this.chkAutoSizeToFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoSizeToFrame.AutoSize = true;
            this.chkAutoSizeToFrame.BackColor = System.Drawing.Color.Transparent;
            this.chkAutoSizeToFrame.Location = new System.Drawing.Point(904, 158);
            this.chkAutoSizeToFrame.Name = "chkAutoSizeToFrame";
            this.chkAutoSizeToFrame.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkAutoSizeToFrame.Size = new System.Drawing.Size(126, 17);
            this.chkAutoSizeToFrame.TabIndex = 35;
            this.chkAutoSizeToFrame.Text = "Size Image To Frame";
            this.chkAutoSizeToFrame.UseVisualStyleBackColor = false;
            // 
            // btnFieldSheet
            // 
            this.btnFieldSheet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFieldSheet.Location = new System.Drawing.Point(219, 697);
            this.btnFieldSheet.Name = "btnFieldSheet";
            this.btnFieldSheet.Size = new System.Drawing.Size(90, 27);
            this.btnFieldSheet.TabIndex = 38;
            this.btnFieldSheet.Text = "Field Sheet";
            this.btnFieldSheet.UseVisualStyleBackColor = false;
            this.btnFieldSheet.Click += new System.EventHandler(this.btnFieldSheet_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnLoginLogout;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1025, 730);
            this.Controls.Add(this.btnFieldSheet);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.chkSplitter);
            this.Controls.Add(this.chkAutoSizeToFrame);
            this.Controls.Add(this.btnSyncFilm);
            this.Controls.Add(this.btnViewReport);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnZoom);
            this.Controls.Add(this.chkFilter);
            this.Controls.Add(this.dateTimePickerFilterAfter);
            this.Controls.Add(this.btnViewStats);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnLog);
            this.Controls.Add(this.lsvFiles);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lsvSessions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.treePanel);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "iLOG INFRINGEMENT VIEWER";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDirectory;
        private System.Windows.Forms.Panel treePanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView lsvSessions;
        private System.Windows.Forms.ListView lsvFiles;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.ColumnHeader columnHeaderGroupingType;
        private System.Windows.Forms.ColumnHeader columnHeaderFileName;
        private System.Windows.Forms.ColumnHeader columnHeaderOffenceDate;
        private System.Windows.Forms.ColumnHeader columnHeaderZone;
        private System.Windows.Forms.ColumnHeader columnHeaderSpeed;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMachineName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Button btnLoginLogout;
        private System.Windows.Forms.ColumnHeader columnHeaderError;
        private System.Windows.Forms.ColumnHeader columnHeaderErrorDesc;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ColumnHeader columnHeaderCamera;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ColumnHeader columnHeaderHasErrors;
        private System.Windows.Forms.ColumnHeader columnHeaderCreateDate;
        private System.Windows.Forms.Button btnViewStats;
        private System.Windows.Forms.DateTimePicker dateTimePickerFilterAfter;
        private System.Windows.Forms.CheckBox chkFilter;
        private System.Windows.Forms.Button btnZoom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnViewReport;
        private System.Windows.Forms.Button btnSyncFilm;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.CheckBox chkSplitter;
        private System.Windows.Forms.CheckBox chkAutoSizeToFrame;
        private System.Windows.Forms.Button btnFieldSheet;
    }
}

