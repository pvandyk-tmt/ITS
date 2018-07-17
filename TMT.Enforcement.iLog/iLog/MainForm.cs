#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TMT.Core.Camera.Base;
using TMT.Enforcement.iLog.Controls.Controls;
using TMT.Enforcement.iLog.Persistence;
using TMT.Core.Camera.RedRoom;
using TMT.Core.Camera.Interfaces;
using System.Security;
using TMT.Enforcement.ErrorWriting;
using System.Diagnostics;


#endregion

namespace TMT.Enforcement.iLog
{
    public partial class MainForm : Form
    {
        private const int HorizontalOffset = 50;
        private const int VerticalOffset = 50;
        private string mImagePath;
        private cPhysicalStudio mPhysicalStudio;
        private FileSystemTreeView mTree;
        private int mUserId;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mTree = new FileSystemTreeView();
            mTree.FolderSelected += tree_FolderSelected;
            treePanel.Controls.Add(mTree);
            mTree.Dock = DockStyle.Fill;
            mTree.HideSelection = false;

            mUserId = -1;
            mImagePath = "";

            lblVersion.Text = "Version " + Assembly.GetAssembly(typeof(MainForm)).GetName().Version.ToString();

#if(DEBUG)
            txtMachineName.Text = "HONB-RICHARDT";
#else
            txtMachineName.Text = Environment.MachineName;
#endif
            var configurationCore = new TMT.Core.Camera.Interfaces.Configuration();

            var redRoomConfiguration = new cConfiguration();

            List<cCamera> cameras = redRoomConfiguration.GetCameras(Path.Combine(DefaultPath(), "Config"), "CameraConfig.xml");
            mPhysicalStudio = new cPhysicalStudio();
            mPhysicalStudio.evFilmAdded += mPhysicalStudioFilmAdded;
            foreach (cCamera abstractCamera in cameras)
            {
                mPhysicalStudio.Register(abstractCamera);
            }

            dateTimePickerFilterAfter.Value = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));

            resetForm();

            if (App.AuthenticatedUser != null)
            {
                btnLoginLogout.PerformClick();
            }
        }

        private string DefaultPath()
        {
            string exe = Process.GetCurrentProcess().MainModule.FileName;
            string path = Path.GetDirectoryName(exe);

            return path;
            //return @"C:\TMT\Camera";
            //if (Environment.Is64BitOperatingSystem)
            //{
            //    return @"C:\Program Files (x86)\TMT\iLog";
            //}
            //else
            //{
            //    return @"C:\Program Files\TMT\iLog";
            //}
        }

        private void mPhysicalStudioFilmAdded(object sender, cFilm film)
        {
            if (film.getFirstValidPictureFile() != null)
            {
                lblInfo.Text = "Adding film session" + film.getFirstValidPictureFile().pSession + " for camera " + film.pCameraDriver.pName + " " + film.pCameraDriver.pVersion + ", " + film.getEncryptedPictureFiles().Count + " files...";
                lblInfo.Visible = true;
                Application.DoEvents();
            }
        }

        private void SetImageViewer()
        {
            foreach (Control control in contentPanel.Controls)
            {
                if (control is IImageViewer)
                {
                    var viewer = (IImageViewer)control;
                    viewer.Clear();
                }
            }

            contentPanel.Controls.Clear();

            if (chkSplitter.Checked)
            {
                var splitter = new ImageSplit {Dock = DockStyle.Fill};
                contentPanel.Controls.Add(splitter);
            }
            else
            {
                var tab = new ImageTab();
                tab.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(tab);
            }
        }

        private void btnLoginLogout_Click(object sender, EventArgs e)
        {
            if (btnLoginLogout.Text == "Logout")
            {
                mUserId = -1;
                mImagePath = "";
                btnLoginLogout.Text = "LogIn";
                lblLogin.Text = "You are not Logged In";
            }
            else
            {
                bool success;
                if (App.AuthenticatedUser != null)
                {
                    mUserId = (int)App.AuthenticatedUser.UserData.ID;
                }

                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["CoreContext"];
                
                var data = new cDataAccess(setting.ConnectionString);

                if (data.pError.Length > 0)
                {
                    MessageBox.Show("ERROR with Login Procedure! - " + data.pError, "Log Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (mUserId <= 0)
                {
                    if (App.AuthenticatedUser == null)
                    {
                        success = data.validateUser(txtUserName.Text, txtPassword.Text, out mUserId);

                        if (!success || mUserId < 0)
                        {
                            MessageBox.Show("Invalid User name or Password. Remeber, its case sensitive!" + " - " + data.pError, "Invalid Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                if (mImagePath == string.Empty)
                {
                    success = data.getImagePath(txtMachineName.Text, out mImagePath);

                    if (!success)
                    {
                        MessageBox.Show("Invalid Image path. Please check the computer name!", "Invalid Image Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        mUserId = -1;
                        return;
                    }

                    //MessageBox.Show("You are now logged in Please proceed to log!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnLoginLogout.Text = "Logout";
                    lblLogin.Text = "Logged In, User Id: " + mUserId;
                }
            }

            if (lsvSessions.Items.Count > 0)
            {
                loadFilms();
            }
        }

        private void btnDirectory_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtDirectory.Text = dlg.SelectedPath;
                mTree.Load(txtDirectory.Text);
                if (mTree.Nodes.Count > 0)
                    mTree.SelectedNode = mTree.Nodes[0];
            }
        }

        private void txtDirectory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (Directory.Exists(txtDirectory.Text) == false)
                {
                    MessageBox.Show("Directory Does Not Exist", "Invalid Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                mTree.Load(txtDirectory.Text);
                if (mTree.Nodes.Count > 0)
                    mTree.SelectedNode = mTree.Nodes[0];
            }
        }

        private void tree_FolderSelected(string path)
        {
            btnSelect.Enabled = true;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            loadFilms();
        }

        private void lsvSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPicsForFilm();
        }

        private void lsvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (lsvFiles.SelectedItems.Count > 0)
            {
                try
                {
                    ListViewItem item = lsvFiles.SelectedItems[0];
                    var pic = (cEncryptedPictureFile) item.Tag;
                    var film = (cFilm) item.Group.Tag;
                    var pictureFile = film.getPictureFile(pic.pEncryptedFileName);

                    if (pictureFile == null)
                    {
                        MessageBox.Show("Cannot show picture - An unexpected error occurred during picture development. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (pictureFile.pHasError)
                    {
                        StringBuilder sb = new StringBuilder();

                        foreach (string s in pictureFile.pErrorCollection)
                        {
                            sb.Append(s);
                            sb.Append(Environment.NewLine);
                        }

                        if (sb.Length == 0)
                        {
                            sb.Append("The Offence Date is missing");
                        }

                        MessageBox.Show("Cannot show picture - An unexpected error occurred during picture development. Please try again.\n\n " + sb, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    updateFilesListItem(item, pictureFile, false, true);
                    
                }
                catch
                {
                    MessageBox.Show("Cannot show picture - An unexpected error occurred during picture development. Please try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void btnViewStats_Click(object sender, EventArgs e)
        {
            if (lsvSessions.SelectedItems != null && lsvSessions.Items.Count > 0 && lsvSessions.SelectedItems.Count == 1)
            {
                foreach (ListViewItem selectedItem in lsvSessions.SelectedItems)
                {
                    var film = (cFilm) selectedItem.Tag;

                    Cursor = Cursors.WaitCursor;
                    mPhysicalStudio.DevelopFilm(film);
                    Cursor = Cursors.Default;

                    var sf = new StatsForm();
                    sf.BindFormData(film);
                    try
                    {
                        sf.ShowDialog(this);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    mPhysicalStudio.ClearFilm(film);
                }
            }
            else
            {
                MessageBox.Show("You have to select one item from the list in order to view the stats!", "Select one item!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnViewReport_Click(object sender, EventArgs e)
        {
            if (lsvSessions.SelectedItems != null && lsvSessions.Items.Count > 0)
            {
                try
                {
                    foreach (ListViewItem selectedItem in lsvSessions.SelectedItems)
                    {
                        var film = (cFilm)selectedItem.Tag;

                        Cursor = Cursors.WaitCursor;
                        mPhysicalStudio.DevelopFilm(film);
                        var reportFilm = (cFilm)film.Clone();
                        mPhysicalStudio.ClearFilm(film);
                        Cursor = Cursors.Default;

                        var sf = new StatsReport();
                        sf.userId = mUserId;
                        sf.BindFormData(reportFilm);

                        Thread.Sleep(500);

                        sf.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something went wrong while generating the report", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                MessageBox.Show("You have to select one item from the list in order to view the report!", "Select one item!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void chkFilter_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerFilterAfter.Enabled = chkFilter.Checked;
        }

        private void loadFilms()
        {
            if (txtDirectory.Text.Length <= 0)
            {
                MessageBox.Show("You must enter a start directory first.", "No Directory Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDirectory.Focus();
                return;
            }

            var dirSelected = new DirectoryInfo(txtDirectory.Text);
            if (!dirSelected.Exists)
            {
                MessageBox.Show("The directory you selected does not exist.", "Directory does not Exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDirectory.Focus();
                return;
            }

            resetForm();

            lblInfo.Text = "Loading Films...please be patient";
            lblInfo.Visible = true;
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["CoreContext"];
            var data = new cDataAccess(setting.ConnectionString);
            Cursor.Current = Cursors.WaitCursor;

            lsvSessions.BeginUpdate();

            try
            {
                if (mTree.SelectedNode != null)
                {
                    if (mTree.SelectedNode.GetType() != typeof(DirectoryNode))
                    {
                        MessageBox.Show("You cannot select a file from the treeview. Select a directory and try again.", "No Dir Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var node = (DirectoryNode)mTree.SelectedNode;
                    if (!node.UserSecurity.canRead())
                    {
                        MessageBox.Show("Your User Account Control does not allow you to read files in the selected directory. Remove UAC or contact your system Administrator.", "No Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!node.UserSecurity.canWrite())
                    {
                        MessageBox.Show("Your User Account Control does not allow you to write to files in the selected directory. Remove UAC or contact your system Administrator.", "No Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }



                    //create the entry here for calling the new camera methods in a dll
                    //getdevelopablefilms will need to accept new paramter - path
                    mPhysicalStudio.Path = node.FullPath;

                    List<cFilm> films = chkFilter.Checked ? mPhysicalStudio.GetDevelopableFilms(dateTimePickerFilterAfter.Value) : mPhysicalStudio.GetDevelopableFilms(null);
                    if (films.Count == 0)
                    {
                        var sb = new StringBuilder();
                        sb.Append("The system could not decrypt files in the selected path.");
                        sb.Append(Environment.NewLine);
                        sb.Append("Possible Reasons:");
                        sb.Append(Environment.NewLine);
                        sb.Append("1. No Files could be found, check the filter or if the path contains files.");
                        sb.Append(Environment.NewLine);
                        sb.Append("2. The files exists but are corrupted and cannot be decrypted.");
                        sb.Append(Environment.NewLine);
                        sb.Append("3. Invalid deskey or deskey not found.");
                        MessageBox.Show(sb.ToString(), "No pictures found.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    foreach (cFilm film in films)
                    {
                        ListViewItem item = new ListViewItem(film.pGroup);

                        item.SubItems.Add(film.pCreationTime != null ? film.pCreationTime.Value.ToString("yyyy-MM-dd") : "*Cannot Read Date");

                        item.SubItems.Add(film.pCameraDriver.pName + " " + film.pCameraDriver.pVersion);

                        item.SubItems.Add(film.pHasErrors ? "Yes" : "No");

                        if (mUserId > 0)
                        {
                            int result;
                            data.checkLoggedSessions(film, out result);
                            item.BackColor = result == 1 ? Color.Purple : Color.Green;
                        }
                        else
                        {
                            item.BackColor = Color.Orange;
                        }

                        item.Tag = film;
                        lsvSessions.Items.Add(item);
                    }
                }
            }
            finally
            {
                lsvSessions.EndUpdate();
                lblInfo.Visible = false;
                Cursor.Current = Cursors.Default;
            }
        }

        private void resetForm()
        {
            lsvFiles.Items.Clear();
            lsvSessions.Items.Clear();

            SetImageViewer();
        }

        private void loadPicsForFilm()
        {
            lblInfo.Text = "Loading Pics for Film...please be patient";
            lblInfo.Visible = true;
            Cursor.Current = Cursors.WaitCursor;

            lsvFiles.BeginUpdate();

            try
            {
                if (lsvSessions.SelectedItems != null && lsvSessions.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem i in lsvFiles.Items)
                    {
                        i.Tag = null;
                    }

                    foreach (ListViewGroup g in lsvFiles.Groups)
                    {
                        g.Tag = null;
                    }

                    lsvFiles.Items.Clear();
                    lsvFiles.Groups.Clear();

                    int groupCounter = 0;
                    var groups = new ListViewGroup[lsvSessions.SelectedItems.Count];

                    foreach (ListViewItem lsvItem in lsvSessions.SelectedItems)
                    {
                        var film = (cFilm) lsvItem.Tag;
                        IEnumerable<int> filesLogged = getFilesLogged(film);

                        var group = new ListViewGroup(film.pGroup) { Tag = film };
                        groups[groupCounter] = group;

                        foreach (cEncryptedPictureFile enc in film.getEncryptedPictureFiles())
                        {
                            var item = new ListViewItem(enc.pEncryptedFileName, group);

                            item.SubItems.Add("");
                            item.SubItems.Add("");
                            item.SubItems.Add("");
                            item.SubItems.Add("");
                            item.SubItems.Add("");

                            if (mUserId <= 0)
                            {
                                item.BackColor = Color.Orange;
                            }
                            else
                            {
                                item.BackColor = existsInDatabase(enc.encryptedFileNumberDb, filesLogged) ? Color.Purple : Color.Green;
                            }

                            item.Tag = enc;

                            cPictureFile file;
                            if (film.existsPictureFile(enc.pEncryptedFileName, out file))
                            {
                                updateFilesListItem(item, file, false, false);
                            }

                            lsvFiles.Items.Add(item);
                        }

                        groupCounter++;
                    }

                    lsvFiles.Groups.AddRange(groups);
                }
            }
            finally
            {
                lblInfo.Visible = false;
                Cursor.Current = Cursors.Default;
                lsvFiles.EndUpdate();
            }
        }

        #region Sync with Database

        private void btnImageReport_Click(object sender, EventArgs e)
        {
            if (mUserId <= 0)
            {
                MessageBox.Show("You must login with a valid username and password before you can validate logged images!", "Cannot Log", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            saveFileDialog1.Filter = "Text File|*.txt";
            saveFileDialog1.Title = "Save Report as a Text File";
            saveFileDialog1.ShowDialog();

            // Get file name.
            string name = saveFileDialog1.FileName;
            // Write to the file name selected.
            // ... You can write the text from a TextBox instead of a string literal.

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["CoreContext"];
            var data = new cDataAccess(setting.ConnectionString);
            data.evSyncedImage += data_SyncedImage;
            var aggregateOfMissingImages = new List<sLoggedImage>();

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (lsvSessions.SelectedItems != null && lsvSessions.SelectedItems.Count > 0 && lsvSessions.Items.Count > 0)
                {
                    if (lsvSessions.SelectedItems.Count > 1)
                    {
                        if (MessageBox.Show("You have selected " + lsvSessions.SelectedItems.Count + " sessions to sync. Are you sure you want to continue?", "Multi session Sync mode", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    foreach (ListViewItem selectedItem in lsvSessions.SelectedItems)
                    {
                        var film = (cFilm) selectedItem.Tag;

                        List<sLoggedImage> missingImages;
                        if (data.syncImages(txtMachineName.Text, film, out missingImages))
                        {
                            aggregateOfMissingImages.AddRange(missingImages);
                        }

                        if (data.pError.Length > 0)
                        {
                            if (film.getFirstValidPictureFile() != null)
                            {
                                MessageBox.Show("ERROR with sync film " + film.getFirstValidPictureFile().pFormattedSession + " - " + data.pError, "Sync Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show("ERROR with sync film <SESSION READ ERROR> - " + data.pError, "Sync Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        mPhysicalStudio.ClearFilm(film);
                    }

                    lblInfo.Visible = true;
                    lblInfo.Text = "Saving Report...";

                    foreach (sLoggedImage img in aggregateOfMissingImages)
                    {
                        var sb = new StringBuilder();

                        sb.Append(img.mLogDate);
                        sb.Append(",");
                        sb.Append(img.mSession);
                        sb.Append(",");
                        sb.Append(img.mLocationCode);
                        sb.Append(",");
                        sb.Append(img.mFileNumber);
                        sb.Append(",");
                        sb.Append(img.mFileName);
                        sb.Append(",");
                        sb.Append(img.mHasError);
                        sb.Append(",");
                        sb.Append(img.mMessage);
                        sb.Append(Environment.NewLine);

                        File.AppendAllText(name, sb.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("You must select a session with encrypted files before you can sync!", "Cannot Sync", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Your text based report contains " + aggregateOfMissingImages.Count + " missing images validated against the files in the database from " + dateTimePickerFilterAfter.Value.ToShortDateString() + ". You can now open and review the file.", "Sync Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblInfo.Text = "The system synced " + aggregateOfMissingImages.Count + " new images!";
            }
        }

        private void data_SyncedImage(object sender, sLoggedImage img, bool isMissing)
        {
            lblInfo.Visible = true;

            if (isMissing)
            {
                lblInfo.Text = "Image Missing..." + img.mFileName + " " + img.mFileNumber;
            }
            else
            {
                lblInfo.Text = "Image Found..." + img.mFileName + " " + img.mFileNumber;
            }

            Application.DoEvents();
        }

        #endregion

        #region Logging To Database

        private void btnLog_Click(object sender, EventArgs e)
        {
            if (mUserId <= 0)
            {
                MessageBox.Show("You must login with a valid username and password before you can log!", "Cannot Log", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int totalLoggedCases = 0;
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["CoreContext"];
            var data = new cDataAccess(setting.ConnectionString);
            data.evNewCaseLogged += data_NewCaseLogged;
            data.evNewCaseLoggedError += data_NewCaseLoggedError;
            data.evNewCasePreviouslyLogged += data_NewCasePreviouslyLogged;

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (lsvSessions.SelectedItems.Count > 0 && lsvSessions.Items.Count > 0)
                {
                    if (lsvSessions.SelectedItems.Count > 1)
                    {
                        if (MessageBox.Show("You have selected " + lsvSessions.SelectedItems.Count + " sessions to log. Are you sure you want to continue?", "Multi session Log mode", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    foreach (ListViewItem selectedItem in lsvSessions.SelectedItems)
                    {
                        var film = (cFilm) selectedItem.Tag;

                        int numberLoggedCases;
                        int previouslyLogged;
                        cFilm reportFilm;
                        if (data.logNewCases(film, mUserId, mImagePath, out numberLoggedCases, out reportFilm, out previouslyLogged))
                        {
                            totalLoggedCases += numberLoggedCases;

                            var sf = new StatsReport();
                            sf.userId = mUserId;
                            sf.BindFormData(reportFilm);
                            sf.Show(this);

                            //Process vosi list
                            data.logVosiFile(film);

                            if (!string.IsNullOrEmpty(film.pStatsFileName))
                            {
                                FileInfo fileInfo = new FileInfo(film.pStatsFileName);
                                var encStatsFileList = data.ReadStatsFile(fileInfo);
                                data.SubmitStatsFile(encStatsFileList);
                            }

                            Application.DoEvents();

                            int sessionLogged;
                            data.checkLoggedSessions(film, out sessionLogged);
                            selectedItem.BackColor = sessionLogged == 1 ? Color.Purple : Color.Green;
                        }

                        if (data.pError.Length > 0)
                        {
                            if (film.getFirstValidPictureFile() != null)
                            {
                                MessageBox.Show("ERROR with logging film " + film.getFirstValidPictureFile().pFormattedSession + " - " + data.pError, "Log Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show("ERROR with logging film <SESSION READ ERROR> - " + data.pError, "Log Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        mPhysicalStudio.ClearFilm(film);
                    }
                }
                else
                {
                    MessageBox.Show("You must select a session with encrypted files before you can log!", "Cannot Log", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("The system logged " + totalLoggedCases.ToString() + " new cases!", "Logged Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblInfo.Text = "The system logged " + totalLoggedCases + " new cases!";                
            }
        }    

        private void data_NewCasePreviouslyLogged(object sender, cFilm film, cEncryptedPictureFile encryptedPictureFile)
        {
            lblInfo.Visible = true;
            lblInfo.Text = "Previously Logged at " + encryptedPictureFile.pEncryptedFileName;

            foreach (ListViewItem item in lsvFiles.Items)
            {
                if (item.Group != null)
                {
                    var groupFilm = (cFilm) item.Group.Tag;

                    cPictureFile pic = film.getFirstValidPictureFile();
                    cPictureFile picGroup = groupFilm.getFirstValidPictureFile();

                    if (pic != null && picGroup != null)
                    {
                        if (pic.pFormattedSession == picGroup.pFormattedSession)
                        {
                            if (item.Text == encryptedPictureFile.pEncryptedFileName)
                            {
                                updateFilesListItemPreviouslyLogged(item, "Previously Logged");
                                break;
                            }
                        }  
                    }
                }
            }
        }

        private void data_NewCaseLoggedError(object sender, cPictureFile pictureFile, string errorDescription)
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["CoreContext"];
            var data = new cDataAccess(setting.ConnectionString);

            lblInfo.Visible = true;
            lblInfo.Text = "Errors at " + pictureFile.pEncryptedPicture.pEncryptedFileName;

            foreach (ListViewItem item in lsvFiles.Items)
            {
                cFilm film = pictureFile.pBelongsToFilm;

                if (item.Group != null)
                {
                    var groupFilm = (cFilm) item.Group.Tag;

                    cPictureFile pic = film.getFirstValidPictureFile();
                    cPictureFile picGroup = groupFilm.getFirstValidPictureFile();

                    if (pic != null && picGroup != null)
                    {
                        if (pic.pFormattedSession == picGroup.pFormattedSession)
                        {
                            if (item.Text == pictureFile.pEncryptedPicture.pEncryptedFileName)
                            {
                                updateFilesListItemError(item, pictureFile, errorDescription);                                
                                break;
                            }
                        }  
                    }
                }
            }
        }

        private void data_NewCaseLogged(object sender, cPictureFile pictureFile, int fileNumber, int numberLogged)
        {
            lblInfo.Visible = true;
            lblInfo.Text = "Logging " + pictureFile.pEncryptedPicture.pEncryptedFileName + ", " + numberLogged + " images";

            cFilm film = pictureFile.pBelongsToFilm;
            film.pCameraDriver.UpdateFileNumber(pictureFile, fileNumber);

            foreach (ListViewItem item in lsvFiles.Items)
            {
                if (item.Group != null)
                {
                    var groupFilm = (cFilm)item.Group.Tag;

                    cPictureFile pic = film.getFirstValidPictureFile();
                    cPictureFile picGroup = groupFilm.getFirstValidPictureFile();

                    if (pic != null && picGroup != null)
                    {
                        if (pic.pFormattedSession == picGroup.pFormattedSession)
                        {
                            if (item.Text == pictureFile.pEncryptedPicture.pEncryptedFileName)
                            {
                                updateFilesListItem(item, pictureFile, true, true);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void updateFilesListItem(ListViewItem item, cPictureFile picture, bool isLogging, bool showPicture)
        {
            lsvFiles.BeginUpdate();

            item.SubItems[1].Text = picture.pOffenceDate != null ? picture.pOffenceDate.Value.ToString("yyyy-MM-dd HH:mm") : "No Date";

            item.SubItems[2].Text = (picture.pZone.ToString());
            item.SubItems[3].Text = (picture.pSpeed.ToString());

            if (picture.pHasError)
            {
                item.SubItems[4].Text = ("Yes");

                var sb = new StringBuilder();
                foreach (string s in picture.pErrorCollection)
                {
                    sb.Append(s);
                }

                item.SubItems[4].Text = (sb.ToString());
            }
            else
            {
                item.SubItems[4].Text = ("No");
                item.SubItems[4].Text = ("");
            }

            if (picture.pIsTest)
            {
                item.BackColor = Color.Blue;
            }
            else if (picture.pHasDecryptedData == false)
            {
                item.BackColor = Color.Red;
            }
            else if (isLogging)
            {
                item.BackColor = Color.Purple;
            }

            if (showPicture)
            {
                if (!addPictureFileToTab(picture))
                {
                    item.SubItems[4].Text = ("Yes");
                    item.SubItems[5].Text = ("Cannot Decrypt Picture File");
                    item.BackColor = Color.Red;
                }
            }

            item.EnsureVisible();

            lsvFiles.EndUpdate();

            Application.DoEvents();
        }

        private void updateFilesListItemError(ListViewItem item, cPictureFile picture, string errorDescription)
        {
            lsvFiles.BeginUpdate();

            item.SubItems[1].Text = picture.pOffenceDate != null ? picture.pOffenceDate.Value.ToString("yyyy-MM-dd HH:mm") : "No Date";

            item.SubItems[2].Text = (picture.pZone.ToString());
            item.SubItems[3].Text = (picture.pSpeed.ToString());
            item.SubItems[4].Text = ("Yes");
            item.SubItems[5].Text = errorDescription;
            item.BackColor = Color.Orange;

            item.EnsureVisible();

            lsvFiles.EndUpdate();

            Application.DoEvents();
        }

        private void updateFilesListItemPreviouslyLogged(ListViewItem item, string message)
        {
            lsvFiles.BeginUpdate();

            item.SubItems[1].Text = "*****";
            item.SubItems[2].Text = ("*****");
            item.SubItems[3].Text = ("*****");
            item.SubItems[4].Text = ("No");
            item.SubItems[5].Text = message;
            item.BackColor = Color.Purple;

            item.EnsureVisible();

            lsvFiles.EndUpdate();

            Application.DoEvents();
        }

        private static bool existsInDatabase(long encFileNumber, IEnumerable<int> filesLogged)
        {
            return filesLogged.Any(s => encFileNumber == s);
        }

        private IEnumerable<int> getFilesLogged(cFilm film)
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["CoreContext"];
            var data = new cDataAccess(setting.ConnectionString);

            var filesLogged = new List<int>();
            if (mUserId > 0)
            {
                int result;
                data.checkLoggedSessions(film, out result);
                data.getCurrentLoggedCases(film, mUserId, mImagePath, out filesLogged);
                if (data.pError.Length > 0)
                {
                    MessageBox.Show("WARNING! Could not checked Logged Cases - " + data.pError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return filesLogged;
        }

        #endregion

        #region Picture Tab

        private bool addPictureFileToTab(cPictureFile pictureFile)
        {
            foreach (Control control in contentPanel.Controls)
            {
                if (control is IImageViewer)
                {
                    var viewer = (IImageViewer)control;

                    if (!viewer.AddPictureFile(pictureFile, chkAutoSizeToFrame.Checked))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void btnZoom_Click(object sender, EventArgs e)
        {
            foreach (Control control in contentPanel.Controls)
            {
                if (control is IImageViewer)
                {
                    var viewer = (IImageViewer)control;
                    viewer.Zoom();
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (tabControlImages.TabPages.Count > 0)
            //{
            //    var viewer = (objImageViewer) tabControlImages.TabPages[tabControlImages.SelectedIndex].Controls[0];

            //    Point ap = viewer.AutoScrollPosition;
            //    if (msg.WParam.ToInt32() == (int) Keys.Left) ap = new Point(-ap.X - HorizontalOffset, -ap.Y);
            //    else if (msg.WParam.ToInt32() == (int) Keys.Right) ap = new Point(-ap.X + HorizontalOffset, -ap.Y);
            //    else if (msg.WParam.ToInt32() == (int) Keys.Down) ap = new Point(-ap.X, -ap.Y + VerticalOffset);
            //    else if (msg.WParam.ToInt32() == (int) Keys.Up) ap = new Point(-ap.X, -ap.Y - VerticalOffset);
            //    else return base.ProcessCmdKey(ref msg, keyData);
            //    viewer.AutoScrollPosition = ap;
            //    return true;
            //}

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        private void chkSplitter_CheckedChanged(object sender, EventArgs e)
        {
            SetImageViewer();
        }

        private void btnFieldSheet_Click(object sender, EventArgs e)
        {
            string pdfPath;
            if (lsvSessions.SelectedItems != null && lsvSessions.Items.Count > 0)
            {
                try
                {
                    foreach (ListViewItem selectedItem in lsvSessions.SelectedItems)
                    {
                        var film = (cFilm)selectedItem.Tag;

                        Cursor = Cursors.WaitCursor;
                        pdfPath = film.pPath;
                        Cursor = Cursors.Default;

                        string[] files = Directory.GetFiles(pdfPath, "*.pdf");

                        if (files != null && files.Length != 0)
                            System.Diagnostics.Process.Start(files[0]);

                        Thread.Sleep(500);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something went wrong while generating the field sheet", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else

            {
                MessageBox.Show("You have to select one item from the list in order to view the field sheet!", "Select one item!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        } 
    }
}