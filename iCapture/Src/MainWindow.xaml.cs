using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Windows.Media.Animation;
using TMT.Core.Camera.Interfaces;
using TMT.Core.Components;
using Kapsch.ITS.Gateway.Clients;
using Kapsch.ITS.Gateway.Models.Capture;
using Kapsch.Gateway.Models.Shared;
using Kapsch.ITS.App.Common.Models;
using Kapsch.ITS.App;
using MahApps.Metro.Controls;
using MahApps.Metro;

namespace TMT.iCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //private cDataAccess mDataAccess = null;
        private CaseModel mCaseInfo;
        private string mUserName = string.Empty;
        private int mTicketIndex = -1;
        private IList<RejectReasonModel> mReasons = null;
        private IList<CaptureTypeModel> mTypes = null;
        private IList<OfficerModel> mOfficers = null;
        private int mOfficerID = -1;
        private string mSheetNo = string.Empty;
        private bool mSheetNoChanged = false;
        private int mSelectedSession = -1;
        private bool mSheetNoUsed = false;
        private IList<SessionModel> mSessions = null;
        private IList<int> mFileNumbers = null;
        private int mOffenceSet = -1;
        private Thickness mThick1;
        private Thickness mThick1a;
        private Thickness mThick2;
        private Thickness mThick2a;
        private double mIconsBorderHeight = 192;    // Icons border height, get read at loaded event and used to collapse & re-expand control
        private bool mIgnoreEvents = true;
        private bool mTypeChanged = false;
        private bool mImagesChanged = false;
        private ImageSource mImageCross;
        private ImageSource mImageCheck;
        private int mPrintImageNo = 0;
        private CaptureService captureService = null;
        private bool usingRemotePath = false;

        //public void ChangeAppStyle()
        //{
        //    // set the Red accent and dark theme only to the current window
        //    ThemeManager.ChangeAppStyle(this,
        //                                ThemeManager.GetAccent("Orange"),
        //                                ThemeManager.GetAppTheme("BaseLight"));
        //}

        public MainWindow()
        {
            InitializeComponent();
            //this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.SizeToContent = SizeToContent.WidthAndHeight;

            mThick1 = new Thickness(1);
            mThick1a = new Thickness(1, 1, 1, 0);
            mThick2 = new Thickness(2);
            mThick2a = new Thickness(2, 2, 2, 0);

            mFileNumbers = new List<int>();
            mSessions = new List<SessionModel>();


            mOfficers = new List<OfficerModel>();
            mReasons = new List<RejectReasonModel>();
            mTypes = new List<CaptureTypeModel>();

            mImageCross = imgNPmatch.Source;
            mImageCheck = imgOfficer.Source;

            if (!Properties.Settings.Default.NumberplateForce)
            {
                spanelNPimage.Visibility = Visibility.Collapsed;
                grpConditions.Height -= spanelNPimage.Height;
                grpConditions.Margin = new Thickness(grpConditions.Margin.Left, grpConditions.Margin.Top + 5, grpConditions.Margin.Right, grpConditions.Margin.Bottom);
            }

            recMask.Visibility = Visibility.Hidden;
            this.Opacity = 0.5;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                captureService = new CaptureService(Startup.AuthenticatedUser.SessionToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Session token not found or has expired. \n\n" + ex.Message, "iCapture", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            ClearValue(SizeToContentProperty);
            LayoutRoot.ClearValue(WidthProperty);
            LayoutRoot.ClearValue(HeightProperty);
            //this.SizeToContent = SizeToContent.WidthAndHeight;

            ScaleTransform scale = new ScaleTransform(1, 1);
            brdIcons.RenderTransform = scale;
            brdIcons.RenderTransformOrigin = new Point(0.7, 0.05);

            mIconsBorderHeight = brdIcons.ActualHeight;

            //imgRightGreen.Margin = new Thickness(-imgRightGreen.ActualWidth, 0, 0, 0);
            imgRightGreen.Visibility = Visibility.Collapsed;

            passwordBoxRegNum.ToolTip = string.Empty;

            lblVersion.Content = string.Empty;
            lblVersion.ToolTip = string.Empty;

            recMask.Height = Properties.Settings.Default.NumberplateHeight;
            recMask.Width = Properties.Settings.Default.NumberplateWidth;
            bdrNP.Height = recMask.Height + 2; // Padding 1 all around
            bdrNP.Width = recMask.Width + 2; // Padding 1 all around

            mCaseInfo = CaseInitialise();

            img2.evOnMouseDown += img2_evOnMouseDown;
            img2.evOnAfterTransformation += img2_evOnAfterTransformation;

            try
            {
                mUserName = Startup.AuthenticatedUser.UserData.UserName;
            }
            catch (GatewayException gex)
            {
                MessageBox.Show(this, "Username not found. \n\n" + gex.Message, "iCapture", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Username not found. \n\n" + ex.Message, "iCapture", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }


            if (!chooseSession())
            {
                this.Close();
                return;
            }

            textBoxSheetNo.IsEnabled = (Properties.Settings.Default.FieldSheetActive);

// remove btnShowVideo.IsEnabled = false; once camera is installed and videos are available
            btnShowVideo.IsEnabled = false;

            mSheetNoChanged = false;
            mSheetNoUsed = false;
            mIgnoreEvents = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            img2.evOnMouseDown -= img2_evOnMouseDown;
            img2.evOnAfterTransformation -= img2_evOnAfterTransformation;

            if (mTicketIndex >= 0)
            {
                if ((mTicketIndex > 0) && (MessageBox.Show(this, "Do you want to Submit your captured images? \n\nWarning, if you answer 'No' the " + mTicketIndex.ToString() + " captured images will have to be re-done.", "Capture images", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
                    doSubmit(false);
                else
                    doUnlock();
            }
        }

        private CaseModel CaseInitialise()
        {
            CaseModel caseInfo = new CaseModel();

            caseInfo.Image1 = string.Empty;
            caseInfo.Image2 = string.Empty;
            caseInfo.Image3 = string.Empty;
            caseInfo.Image4 = string.Empty;
            caseInfo.ImageNP = string.Empty;
            caseInfo.RemoteImage1 = string.Empty;
            caseInfo.RemoteImage2 = string.Empty;
            caseInfo.RemoteImage3 = string.Empty;
            caseInfo.RemoteImage4 = string.Empty;
            caseInfo.RemoteImageNP = string.Empty;
            caseInfo.Image1ID =
            caseInfo.Image2ID =
            caseInfo.Image3ID =
            caseInfo.Image4ID =
            caseInfo.RemoteImage1ID =
            caseInfo.RemoteImage2ID =
            caseInfo.RemoteImage3ID =
            caseInfo.RemoteImage4ID = -1;
            caseInfo.OnlyOneImage = false;
            caseInfo.PrintImageNumber = 0;
            caseInfo.VehicleRegisterNumber = string.Empty;
            caseInfo.VehicleType = string.Empty;
            caseInfo.PreviousRejectID = -1;
            caseInfo.OffenceDate = string.Empty;
            caseInfo.OffencePlace = string.Empty;
            caseInfo.OffenceSpeed = -1;
            caseInfo.OffenceZone = -1;

            return caseInfo;
        }

        /// <summary>
        /// Choose sessions.
        /// NB The connection is already open at this stage!
        /// </summary>
        /// <param name="doConnect"></param>
        /// <returns></returns>
        private bool chooseSession()
        {
            this.Cursor = Cursors.Wait;
            this.Opacity = 0.5;

            displayUpdate();

            bool loadNewOnly = true;
            string[] headings = null;

            for (; ; )
            {
                mTicketIndex = -1;
                mOfficerID = -1;
                mSheetNo = string.Empty;
                mSheetNoChanged = false;
                mSelectedSession = -1;
                mOffenceSet = -1;

                try
                {
                    captureService = new CaptureService(Startup.AuthenticatedUser.SessionToken);

                }
                catch (GatewayException gex)
                {
                    MessageBox.Show(this, "Session token not found or has expired. \n\n" + gex.Message, "iCapture", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Session token not found or has expired. \n\n" + ex.Message, "iCapture", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }

                try
                {
                    var sessionsModel = captureService.GetSessions();

                    headings = sessionsModel.Headings.ToArray();
                    mSessions = sessionsModel.Sessions;

                    cSessions diag = new cSessions(headings, ref mSessions);
                    diag.Owner = this;

                    if (diag.ShowDialog() != true) // Close program
                        return false;

                    mSelectedSession = diag.pSelectedIndex;
                    loadNewOnly = diag.pLoadNew;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not get any sessions. \n\n" + ex.Message, "Camera Sessions", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }

                try
                {
                    var activeCaseModel = captureService.GetFirstCase(loadNewOnly, Environment.MachineName, mSessions[mSelectedSession]);

                    if (!File.Exists(activeCaseModel.Case.Image1))
                    {
                        usingRemotePath = true;

                        activeCaseModel.Case.Image1 = activeCaseModel.Case.RemoteImage1;
                        activeCaseModel.Case.Image2 = activeCaseModel.Case.RemoteImage2;
                        activeCaseModel.Case.Image3 = activeCaseModel.Case.RemoteImage3;
                        activeCaseModel.Case.Image4 = activeCaseModel.Case.RemoteImage4;
                        activeCaseModel.Case.Image1ID = activeCaseModel.Case.RemoteImage1ID;
                        activeCaseModel.Case.Image2ID = activeCaseModel.Case.RemoteImage2ID;
                        activeCaseModel.Case.Image3ID = activeCaseModel.Case.RemoteImage3ID;
                        activeCaseModel.Case.Image4ID = activeCaseModel.Case.RemoteImage4ID;
                        activeCaseModel.Case.PrintImageNumber = activeCaseModel.Case.RemotePrintImageNumber;
                    }
                    else
                        usingRemotePath = false;

                    if (!File.Exists(activeCaseModel.Case.ImageNP))
                    {
                        activeCaseModel.Case.ImageNP = activeCaseModel.Case.RemoteImageNP;
                    }

                    mReasons = activeCaseModel.RejectReasons;
                    mTypes = activeCaseModel.CaptureTypes;
                    mFileNumbers = activeCaseModel.FileNumbers;
                    mOfficers = activeCaseModel.Officers;
                    mCaseInfo = activeCaseModel.Case;
                    mOffenceSet = activeCaseModel.OffenceSet;
                    mTicketIndex = activeCaseModel.StartIndex;

                    
                    if ((textBoxOfficer.ToolTip == null) && (activeCaseModel.Officers.Count > 0) && (activeCaseModel.Officers.Count < 50))
                    {
                        textBoxOfficer.ToolTip = string.Format("{0} {1}", activeCaseModel.Officers[0].Name, activeCaseModel.Officers[0].Surname);

                        for (int i = 1; i < activeCaseModel.Officers.Count; i++)
                            textBoxOfficer.ToolTip += string.Format("\n{0} {1}", activeCaseModel.Officers[i].Name, activeCaseModel.Officers[i].Surname);
                    }

                    displayUpdate();
                    showImages();

                    break;
                }
                catch (GatewayException gex)
                {
                    MessageBox.Show(this, "Could not get any tickets for this session. \n\n" + gex.Message, "Capture tickets", MessageBoxButton.OK, MessageBoxImage.Stop);
                    doUnlock();
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not get any tickets. \n\n" + ex.Message, "Capture tickets", MessageBoxButton.OK, MessageBoxImage.Stop);
                    doUnlock();
                    return false;
                }
            }

            this.Opacity = 1;
            this.Cursor = null;
            //this.SizeToContent = SizeToContent.WidthAndHeight;

            lblOfficer.Content = "Unknown";
            textBoxOfficer.Text = string.Empty;
            textBoxSheetNo.Text = string.Empty;
            buttonsCheckEnable();
            textBoxOfficer.Focus();

            return true;
        }

        private void displayUpdate()
        {
            lblTicketNumber.Content = string.Format("{0} {1}", mUserName, "capturing images..");

#if(DEBUG)
            if (mTicketIndex >= 0)
            {
                if (mTicketIndex < mFileNumbers.Count)
                    lblTicketNumber.Content += " (" + mFileNumbers[mTicketIndex] + ")";
                else
                    lblTicketNumber.Content += " (Finish)";
            }
#endif
            if (usingRemotePath || !File.Exists(mCaseInfo.Image1))
            {
                mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
                //usingRemotePath = true;
            }
            else
            {
                usingRemotePath = false;
            }

            if (!string.IsNullOrEmpty(mCaseInfo.Image1))
            {
                lblFileLocation.Content = System.IO.Path.GetDirectoryName(mCaseInfo.Image1);
            }

            if (mTicketIndex >= mFileNumbers.Count)
                mTicketIndex = mFileNumbers.Count - 1;

            lblTickets.Content = (mTicketIndex + 1).ToString() + " of " + mFileNumbers.Count.ToString();

            //textBoxRegNum.Text = mCaseInfo.mVehicleRegNo = string.Empty;
            textBoxRegNum.Text = string.Empty;
            textBoxRegNum.IsEnabled = false;  // Only enable after passwordBox has a few characters typed

            textBoxFine.Text = textBoxCode.Text = textBoxDescription.Text = string.Empty;

            comboBoxType.Items.Clear();
            if (mTypes != null && mTypes.Count > 0)
            {
                int selectIndex = 0; //if more vehicle types are made available - set selectIndex back to -1 on this line

                for (int i = 0; i < mTypes.Count; i++)
                {
                    comboBoxType.Items.Add(mTypes[i]);
                    if (selectIndex == -1 && mTypes[i].Type.ToLower().IndexOf("light") >= 0)
                        selectIndex = i;
                }

                if (selectIndex > -1)
                {
                    comboBoxType.SelectedIndex = selectIndex;
                    textBoxDescription.Text = mTypes[selectIndex].Description;
                    if (mTypes[selectIndex].Amount == 99999)
                        textBoxFine.Text = "NAG";
                    else
                        textBoxFine.Text = mTypes[selectIndex].Amount.ToString("0");
                    textBoxCode.Text = mTypes[selectIndex].Code.ToString();
                }
            }

            comboBoxType.IsEnabled = comboBoxType.Items.Count > 0;

            textBoxDate.Text = mCaseInfo.OffenceDate;

            textBoxPlace.Text = mCaseInfo.OffencePlace;
            if (mCaseInfo.OffenceZone > 0)
                textBoxZone.Text = mCaseInfo.OffenceZone.ToString();
            else
                textBoxZone.Text = string.Empty;

            if (mCaseInfo.OffenceSpeed >= 0)
                textBoxSpeed.Text = mCaseInfo.OffenceSpeed.ToString();
            else
                textBoxSpeed.Text = string.Empty;

            if (mCaseInfo.OffenceSpeed < 5)
            {
                textBoxSpeed.Background = Brushes.Red;
                textBoxDescription.Text = "Test Photo.";
            }
            else
                textBoxSpeed.Background = textBoxZone.Background;

            lblPrevReject.Content = string.Empty;
            if (mCaseInfo.PreviousRejectID > 0)
                for (int i = 0; i < mReasons.Count; i++)
                    if (mReasons[i].ID == mCaseInfo.PreviousRejectID)
                    {
                        lblPrevReject.Content = "Previous rejected \n" + mReasons[i].Description;
                        break;
                    }

            mTypeChanged = mImagesChanged = false;
            mPrintImageNo = 0;

            imgOfficer.Source = imgNPimage.Source = imgNPmatch.Source = mImageCross;

            img1.Source = img2.Source = imgBG.Source = imgNP.Source = null;
            Icon1.Source = Icon2.Source = Icon3.Source = Icon4.Source = null;
            img1.resetTransformation();
            img2.resetTransformation();

            numberplateDrop();

            buttonsCheckEnable();

            if (string.IsNullOrEmpty(mCaseInfo.VehicleRegisterNumber))
            {
                passwordBoxRegNum.ToolTip =
                passwordBoxRegNum.Password = string.Empty;
                passwordBoxRegNum.Focus();
            }
            else
            {
                bool ignore = mIgnoreEvents;
                mIgnoreEvents = false;
                passwordBoxRegNum.ToolTip =
                passwordBoxRegNum.Password = mCaseInfo.VehicleRegisterNumber;
                mIgnoreEvents = ignore;
                textBoxRegNum.Focus();
            }
        }

        private void doReject()
        {
            //if (MessageBox.Show(this, "Are you sure you want to reject '" + mCaseInfo.mTicketNo + "'?", "Image Reject", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            //    return;

            cReasons reas = new cReasons();
            reas.pReasons = mReasons;
            reas.pPrevReasonID = mCaseInfo.PreviousRejectID;
            reas.Owner = this;

            if (reas.ShowDialog() != true)
            {
                return;
            }
            else
            {
                bool isLastTicket = false;
                int thisFileNo = -1;
                int nextFileNo = -1;

                if (mTicketIndex < mFileNumbers.Count)
                    thisFileNo = mFileNumbers[mTicketIndex];
                if (mTicketIndex + 1 < mFileNumbers.Count)
                    nextFileNo = mFileNumbers[mTicketIndex + 1];

                isLastTicket = (nextFileNo < 0);

                if (Properties.Settings.Default.FieldSheetActive)
                {
                    mSheetNo = textBoxSheetNo.Text.Trim().ToUpper();
                    if (mSheetNo.Length <= 0)
                    {
                        MessageBox.Show(this, "Field Sheet Number may not be empty.", "Capture error", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else
                    mSheetNo = string.Empty;

                if (mOfficerID <= 0)
                {
                    MessageBox.Show(this, "Officer may not be empty.", "Capture error", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(mCaseInfo.ImageNP))
                {
                    try
                    {
                        CreateNPVariables(out string mimeType, out string mimeDataPath, out string fileName, out int evidenceFileNumber);
                        captureService.SaveNPImage(mimeType, mimeDataPath, fileName, evidenceFileNumber);

                    }
                    catch (GatewayException gex)
                    {
                        MessageBox.Show(this, "Could not save number plate image. \n\n" + gex.Message, "Number Plate Capture Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Could not save number plate image. \n\n" + ex.Message, "Number Plate Capture Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                try
                {

                    var rejectCase = new RejectCaseModel();

                    rejectCase.Session = mSessions[mSelectedSession];
                    rejectCase.CaseInfo = mCaseInfo;
                    rejectCase.RejectReasonID = reas.pReasonID;
                    rejectCase.OffenceSetID = mOffenceSet;
                    rejectCase.FileNumber = thisFileNo;
                    rejectCase.NextFileNumber = nextFileNo;
                    rejectCase.OfficerID = mOfficerID;
                    rejectCase.SheetNumber = mSheetNo;
                    rejectCase.HasSheetNumberChanged = mSheetNoChanged;
                    rejectCase.ComputerName = Environment.MachineName;

                    var rejectedCase = captureService.RejectCase(rejectCase, Environment.MachineName);

                    mCaseInfo = rejectedCase.Case;
                    mTypes = rejectedCase.CaptureTypes;

                }
                catch (GatewayException gex)
                {
                    MessageBox.Show(this, "Could not Reject offence. \n\n" + gex, "Reject error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not Reject offence. \n\n" + ex, "Reject error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //mDataAccess.disconnect();

                if (isLastTicket)
                {
                    if (MessageBox.Show(this, "All images are captured, okay to Submit?", "Capture completed", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        doSubmit(true);
                }

            }

            mSheetNoChanged = false;
            mSheetNoUsed = true;
            mTicketIndex++;
            displayUpdate();
            showImages();
        }

        private void doAccept()
        {
            mCaseInfo.VehicleRegisterNumber = textBoxRegNum.Text = textBoxRegNum.Text.ToUpper().Trim();

            //if (MessageBox.Show(this, "Are you sure you want to accept the current Image?", "Image Accept", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            //    return;

            bool doAccept = true;
            bool isLastTicket = false;
            CaptureTypeModel typ = (CaptureTypeModel)comboBoxType.SelectedItem;

            if (doAccept)
            {
                int thisFileNo = -1;
                int nextFileNo = -1;
                int printImageID = -1;

                if (mTicketIndex < mFileNumbers.Count)
                    thisFileNo = mFileNumbers[mTicketIndex];
                if (mTicketIndex + 1 < mFileNumbers.Count)
                    nextFileNo = mFileNumbers[mTicketIndex + 1];

                isLastTicket = (nextFileNo < 0);

                switch (mPrintImageNo)
                {
                    case 0: printImageID = mCaseInfo.Image1ID; break;
                    case 1: printImageID = mCaseInfo.Image2ID; break;
                    case 2: printImageID = mCaseInfo.Image3ID; break;
                    case 3: printImageID = mCaseInfo.Image4ID; break;
                }

                if (Properties.Settings.Default.FieldSheetActive)
                {
                    mSheetNo = textBoxSheetNo.Text.Trim().ToUpper();
                    if (mSheetNo.Length <= 0)
                    {
                        MessageBox.Show(this, "Field Sheet Number may not be empty.", "Capture error", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else
                    mSheetNo = string.Empty;

                if (mOfficerID <= 0)
                {
                    MessageBox.Show(this, "Officer may not be empty.", "Capture error", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(mCaseInfo.ImageNP))
                {
                    try
                    {
                        CreateNPVariables(out string mimeType, out string mimeDataPath, out string fileName, out int evidenceFileNumber);
                        captureService.SaveNPImage(mimeType, mimeDataPath, fileName, evidenceFileNumber);

                    }
                    catch (GatewayException gex)
                    {
                        MessageBox.Show(this, "Could not save number plate image. \n\n" + gex.Message, "Number Plate Capture Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Could not save number plate image. \n\n" + ex.Message, "Number Plate Capture rror", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                try
                {
                    var acceptCase = new AcceptCaseModel();
                    acceptCase.Session = mSessions[mSelectedSession];
                    acceptCase.OfficerID = mOfficerID;
                    acceptCase.FileNumber = thisFileNo;
                    acceptCase.NextFileNumber = nextFileNo;
                    acceptCase.SheetNumber = mSheetNo;
                    acceptCase.OffenceSetID = mOffenceSet;
                    acceptCase.HasSheetNumberChanged = mSheetNoChanged;
                    acceptCase.CaseInfo = mCaseInfo;
                    acceptCase.CaptureType = typ;
                    acceptCase.PrintImageID = printImageID;
                    acceptCase.ComputerName = Environment.MachineName;

                   var acceptedCase = captureService.AcceptCase(acceptCase, Environment.MachineName);

                    mCaseInfo = acceptedCase.Case;
                    mTypes = acceptedCase.CaptureTypes;

                }
                catch (GatewayException gex)
                {
                    MessageBox.Show(this, "Could not save Capture. \n\n" + gex.Message, "Capture error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not save Capture. \n\n" + ex.Message, "Capture error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                mSheetNoChanged = false;
                mSheetNoUsed = true;
                mTicketIndex++;
                displayUpdate();
                showImages();

            }

            if (isLastTicket)
            {
                if (MessageBox.Show(this, "All images are captured, okay to Submit?", "Capture completed", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    doSubmit(true);
            }
        }

        private void CreateNPVariables (out string mimeType, out string mimeDataPath, out string fileName, out int evidenceFileNumber)
        {
            mimeDataPath = mCaseInfo.ImageNP;
            mimeDataPath = mimeDataPath.Substring(mimeDataPath.IndexOf("SafetyCamImages\\"));
            mimeDataPath = mimeDataPath.Remove(0, 16);
            mimeDataPath = mimeDataPath.Contains("T-Img") ? mimeDataPath.Substring(0, mimeDataPath.IndexOf("\\T-Img")) : mimeDataPath.Substring(0, mimeDataPath.IndexOf("\\Img"));

            fileName = mCaseInfo.ImageNP.Contains("T-Img") ? mCaseInfo.ImageNP.Substring(mCaseInfo.ImageNP.IndexOf("T-Img")) : mCaseInfo.ImageNP.Substring(mCaseInfo.ImageNP.IndexOf("Img"));

            if (fileName.Contains(".jpg") || fileName.Contains(".jpeg"))
                mimeType = "image/jpeg";
            else if (fileName.Contains(".png"))
                mimeType = "image/png";
            else if (fileName.Contains(".bmp"))
                mimeType = "image/bmp";
            else if (fileName.Contains(".gif"))
                mimeType = "image/gif";
            else if (fileName.Contains(".mp4a"))
                mimeType = "audio/mp4";
            else if (fileName.Contains(".mp4"))
                mimeType = "video/mp4";
            else if (fileName.Contains(".avi"))
                mimeType = "video/avi";
            else if (fileName.Contains(".tiff"))
                mimeType = "image/tiff";
            else if (fileName.Contains(".tif"))
                mimeType = "image/tif";
            else
                mimeType = "unknown";

            string evidenceFileNumberStringVal = fileName.Contains("T-Img") ? fileName.TrimStart(("T-Img").ToCharArray()) : fileName.TrimStart(("Img").ToCharArray());
            evidenceFileNumberStringVal = evidenceFileNumberStringVal.Substring(0, evidenceFileNumberStringVal.IndexOf(".enc_"));

            evidenceFileNumber = Convert.ToInt32(evidenceFileNumberStringVal);
        }

        private void doSubmit(bool doSessions)
        {
            this.Cursor = Cursors.Wait;
            buttonSubmit.IsEnabled = buttonNext.IsEnabled = buttonPrevious.IsEnabled = false;
            
            numberplateFreeze();

            mCaseInfo = CaseInitialise();

            displayUpdate();
            showImages();

            int captureTotal;
            int rejectTotal;

            try
            {
                var submittedCase = new SubmitCaseModel();

                submittedCase = captureService.SubmitCase(mSessions[mSelectedSession].CameraDate, mSessions[mSelectedSession].CameraSessionID, mSessions[mSelectedSession].LocationCode);

                captureTotal = submittedCase.CaptureTotal;
                rejectTotal = submittedCase.RejectTotal;

                MessageBox.Show(this, "Submit completed successfully. \nAccepted " + captureTotal + " and rejected " + rejectTotal + " images.", "Submit images", MessageBoxButton.OK, MessageBoxImage.Information);
                mTicketIndex = -1;
                if (doSessions)
                {
                    if (!chooseSession())
                    {
                        this.Close();
                        return;
                    }
                }
            }
            catch (GatewayException gex)
            {
                MessageBox.Show(this, "Could not submit captured images. \n\n" + gex.Message, "Submit error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (doSessions)
                {
                    if (!chooseSession())
                    {
                        this.Close();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not submit captured images. \n\n" + ex.Message, "Submit error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (doSessions)
                {
                    if (!chooseSession())
                    {
                        this.Close();
                        return;
                    }
                }
            }

            buttonsCheckEnable();
            this.Cursor = null;
        }
        

        private void doUnlock()
        {
            //if (doConnect && (!mDataAccess.connect()))
            //{
            //    MessageBox.Show(this, "Could not connect to database. \n\n" + mDataAccess.pError, "DB connect error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}


            //if (doConnect)
            //{
                try
                {
                    captureService.UnlockCase(mSessions[mSelectedSession].CameraDate, mSessions[mSelectedSession].CameraSessionID, mSessions[mSelectedSession].LocationCode);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not unlock. \n\n" + ex.Message, "Capture error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            //}


            //if (doConnect)
            //    mDataAccess.disconnect();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            buttonSubmit.IsEnabled = buttonNext.IsEnabled = buttonPrevious.IsEnabled = false;

            numberplateFreeze();

            if (testAccept())
                doAccept();
            else
                doReject();

            buttonsCheckEnable();
            this.Cursor = null;
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            buttonSubmit.IsEnabled = buttonNext.IsEnabled = buttonPrevious.IsEnabled = false;

            numberplateFreeze();
          
                mTicketIndex--;

                if (mTicketIndex < 0)
                    mTicketIndex = 0;
            try
                {

                var previousCase = new NewCaseModel();

                previousCase = captureService.PreviousCase(mOffenceSet, mFileNumbers[mTicketIndex], Environment.MachineName, mSessions[mSelectedSession]);

                mCaseInfo = previousCase.Case;
                mTypes = previousCase.CaptureTypes;

                displayUpdate();
                showImages();

                }
            catch (GatewayException gex)
            {
                MessageBox.Show(this, "Could not retrieve previous offence. \n\n" + gex.Message, "Previous error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not retrieve previous offence. \n\n" + ex.Message, "Previous error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            buttonsCheckEnable();
            this.Cursor = null;
        }

            private void buttonSubmit_Click(object sender, RoutedEventArgs e)
        {
            buttonSubmit.IsEnabled = false;
            this.Cursor = Cursors.Wait;
            doSubmit(true);
            this.Cursor = null;
            //buttonSubmit.IsEnabled = false;
        }


        private bool checkRegNos()
        {
            return (textBoxRegNum.Text.Trim().Length > 0) && textBoxRegNum.Text.Trim().ToUpper().Equals(passwordBoxRegNum.Password.Trim().ToUpper());
        }

        private bool checkOfficer()
        {
            return (textBoxOfficer.Text.Trim().Length > 0 && (string)lblOfficer.Content != "Unknown");
        }

        private bool checkFieldSheetNo()
        {
            if (Properties.Settings.Default.FieldSheetActive)
                return (textBoxSheetNo.Text.Trim().Length > 0);

            return true;
        }

        private bool testAccept()
        {
            return ((mTicketIndex >= 0) && (mCaseInfo.OffenceSpeed > 5) &&
                    (!Properties.Settings.Default.NumberplateForce || (Properties.Settings.Default.NumberplateForce && imgNP.Source != null)) &&
                    (comboBoxType.SelectedItem != null) &&
                    checkOfficer() && checkRegNos() && checkFieldSheetNo());
        }

        private void buttonsCheckEnable()
        {
            if (usingRemotePath || !File.Exists(mCaseInfo.Image1))
            {
                mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
                //usingRemotePath = true;
            }
            else
            {
                usingRemotePath = false;
            }

            buttonNext.IsEnabled = grpMain.IsEnabled = !string.IsNullOrEmpty(mCaseInfo.Image1); // Always enabled to do rejects

            if (mCaseInfo.OffenceSpeed < 5)  // Test image will get rejected
            {
                buttonNext.ToolTip = "Reject this test image and load the next image...";
                lblAcceptReject.Content = "Reject";
                lblAcceptReject.Background = Brushes.Red;
            }
            else
            {
                if (testAccept())
                {
                    buttonNext.ToolTip = "Accept this image and load the next image...";
                    lblAcceptReject.Content = "Accept";
                    lblAcceptReject.Background = Brushes.Green;
                    imgRightGreen.Visibility = Visibility.Visible;
                    imgRightRed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    buttonNext.ToolTip = "Reject this image and load the next image...";
                    lblAcceptReject.Content = "Reject";
                    lblAcceptReject.Background = Brushes.Red;
                    imgRightGreen.Visibility = Visibility.Collapsed;
                    imgRightRed.Visibility = Visibility.Visible;
                }
            }

            if (imgNP.Source == null)
                imgNPimage.Source = mImageCross;
            else
                playStoryboard(imgNPimage);

            if (checkRegNos())
                playStoryboard(imgNPmatch);
            else
                imgNPmatch.Source = mImageCross;

            if (checkOfficer())
                playStoryboard(imgOfficer);
            else
                imgOfficer.Source = mImageCross;

            buttonSubmit.IsEnabled = buttonPrevious.IsEnabled = (mTicketIndex > 0);
        }

        private void playStoryboard(Image img)
        {
            if (mIgnoreEvents)
                return;

            if (img.Source != mImageCheck)
            {
                img.Source = mImageCheck;
                Storyboard hu = this.FindResource("HeightUp") as Storyboard;
                Storyboard.SetTarget(hu, img);
                hu.Begin();
            }
        }

        private void comboBoxType_GotFocus(object sender, RoutedEventArgs e)
        {
            if (mIgnoreEvents)
                return;
            numberplateFreeze();
        }

        private void comboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mIgnoreEvents)
                return;
            mTypeChanged = true;

            if (comboBoxType.SelectedIndex >= 0)
            {
                textBoxDescription.Text = mTypes[comboBoxType.SelectedIndex].Description;
                if (mTypes[comboBoxType.SelectedIndex].Amount == 99999)
                    textBoxFine.Text = "NAG";
                else
                    textBoxFine.Text = mTypes[comboBoxType.SelectedIndex].Amount.ToString("0");
                textBoxCode.Text = mTypes[comboBoxType.SelectedIndex].Code.ToString();
            }
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (mIgnoreEvents)
                return;

            textBoxRegNum.MaxLength = passwordBoxRegNum.Password.Length;

            numberplateFreeze();
        }

        private void passwordBoxRegNum_LostFocus(object sender, RoutedEventArgs e)
        {
            if (mIgnoreEvents)
                return;

            passwordBoxRegNum.Password = passwordBoxRegNum.Password.ToUpper().Trim().Replace(" ", "");

            textBoxRegNum.MaxLength = passwordBoxRegNum.Password.Length;
        }

        private void passwordBoxRegNum_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (mIgnoreEvents)
                return;

            textBoxRegNum.Text = string.Empty;
            textBoxRegNum.IsEnabled = passwordBoxRegNum.Password.Trim().Length > 2;

            numberplateFreeze();
        }

        private void textBoxRegNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mIgnoreEvents)
                return;
            numberplateFreeze();

            if ((passwordBoxRegNum.Password.Length > 0) && (textBoxRegNum.Text.Trim().Length == passwordBoxRegNum.Password.Trim().Length))
            {
                if (!checkRegNos())
                {
                    MessageBox.Show(this, "Registration numbers differ. \nPlease retype.", "Capture error", MessageBoxButton.OK, MessageBoxImage.Hand);
                    textBoxRegNum.Text = passwordBoxRegNum.Password = string.Empty;
                    passwordBoxRegNum.Focus();
                }
            }
        }

        private void textBoxOfficer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mIgnoreEvents)
                return;
            numberplateFreeze();

            string tmp = textBoxOfficer.Text.ToUpper().Trim();
            lblOfficer.Content = "Unknown";
            mOfficerID = -1;

            if (!string.IsNullOrEmpty(tmp))
            {
                for (int i = 0; i < mOfficers.Count; i++)
                    if (mOfficers[i].ExternalID.ToUpper().Equals(tmp))
                    {
                        textBoxOfficer.IsTabStop = false;
                        textBoxOfficer.Text = textBoxOfficer.Text.ToUpper().Trim();
                        lblOfficer.Content = mOfficers[i].Name + " " + mOfficers[i].Surname;
                        mOfficerID = mOfficers[i].ID;
                        break;
                    }
            }

            buttonsCheckEnable();
        }

        private void textBoxSheetNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mIgnoreEvents)
                return;
            numberplateFreeze();

            if (mSheetNoUsed)
                mSheetNoChanged = true;

            buttonsCheckEnable();
        }

        #region ---- Image handling -----
        private void img2_evOnMouseDown(Point? parentPnt)
        {
            if ((recMask.Visibility == Visibility.Hidden) && (parentPnt != null))
            {
                recMask.Margin = new Thickness(parentPnt.Value.X - (recMask.Width / 2), parentPnt.Value.Y - (recMask.Height / 2), 0, 0);

                numberplateClip();
            }
            else
            {
                buttonsCheckEnable();
            }

            img2.Focus();
        }

        private void img2_evOnAfterTransformation(Point? parentPnt)
        {
            if (recMask.Visibility != Visibility.Hidden)
                numberplateClip();
        }

        private void img2_GotFocus(object sender, RoutedEventArgs e)
        {
            brdImg2.BorderBrush = bdrImg2Label.BorderBrush = Brushes.OrangeRed;
            brdImg2.BorderThickness = mThick2;
            bdrImg2Label.BorderThickness = mThick2a;

            if (bdrNP.Visibility == Visibility.Hidden) // If img2 gets focus and no number plate is shown, do clip
                numberplateClip();
        }

        private void img2_LostFocus(object sender, RoutedEventArgs e)
        {
            brdImg2.BorderBrush = bdrImg2Label.BorderBrush = Brushes.White;
            brdImg2.BorderThickness = mThick1;
            bdrImg2Label.BorderThickness = mThick1a;

            //if (bdrNP.Visibility == Visibility.Visible) // If img2 gets focus and no numberplate is shown, do clip
            //    numberplateFreeze();
        }

        private void numberplateClip()
        {
            //if (mCaseInfo.mOffenceSpeed < 5)
            //    MessageBox.Show(this, "May not capture an image with zero speed.", "Test Image", MessageBoxButton.OK, MessageBoxImage.Information);
            //else
            if (mCaseInfo.OffenceSpeed > 5)
            {
                img2.clipImage(recMask.Margin.Left, recMask.Margin.Top, recMask.Width, recMask.Height);

                imgNP.Source = img2.cropImageOnClip();

                recMask.Visibility = Visibility.Visible;
                bdrNP.Visibility = Visibility.Visible;
            }

            buttonsCheckEnable();
        }

        private void numberplateDrop()
        {
            recMask.Visibility = Visibility.Hidden;
            bdrNP.Visibility = Visibility.Hidden;
            img2.Clip = null;

            buttonsCheckEnable();
        }

        private void numberplateFreeze()
        {
            if (recMask.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(mCaseInfo.ImageNP))
                {
                    if (usingRemotePath || !File.Exists(mCaseInfo.Image1))
                    {
                        mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
                        //usingRemotePath = true;
                    }
                    else
                    {
                        usingRemotePath = false;
                    }

                        mCaseInfo.ImageNP = ConstructNPName(mCaseInfo.Image1);
                }

                imgNP.ToolTip = mCaseInfo.ImageNP;
                imgNP.Save(mCaseInfo.ImageNP);
                bdrNP.InvalidateVisual();  // Force a rerender because the NP is slightly moved after save

            }

            recMask.Visibility = Visibility.Hidden;
            img2.Clip = null;
            img2.resetTransformation();
            img1.resetTransformation();

            buttonsCheckEnable();
        }

        public string ConstructNPName(string fromName)
        {
            if (mCaseInfo.IsNumberPlateCentralCaptured && (!string.IsNullOrEmpty(mCaseInfo.NumberPlateCentralPath)))
            {
                fromName = fromName.Replace(System.IO.Path.GetDirectoryName(fromName), mCaseInfo.NumberPlateCentralPath);
            }

            if (fromName.IndexOf("_3.jpg") > 1)
                return fromName.Replace("_3.jpg", "_NP.jpg");
            if (fromName.IndexOf("_2.jpg") > 1)
                return fromName.Replace("_2.jpg", "_NP.jpg");
            if (fromName.IndexOf("_1.jpg") > 1)
                return fromName.Replace("_1.jpg", "_NP.jpg");

            return fromName.Replace("_0.jpg", "_NP.jpg");
        }

        delegate void delShowImages();
        private void showImages()
        {
            imgBG.Source = null;

            if (usingRemotePath || !File.Exists(mCaseInfo.Image1))
            {
                mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
                //usingRemotePath = true;
            }
            else
            {
                usingRemotePath = false;
            }

            if (loadBitmap(mCaseInfo.Image1, img1))
            {
                Icon1.Source = img1.Source;
                Icon1.Focusable = false;
            }
            else
                Icon1.Source = null;

            if (loadBitmap((mCaseInfo.OnlyOneImage ? mCaseInfo.Image1 : mCaseInfo.Image2), img2, imgBG))
            {
                Icon2.Source = img2.Source;
                Icon2.Focusable = false;
            }
            else
                Icon2.Source = null;

            ImageViewer.cImage tmp = new ImageViewer.cImage();

            if (loadBitmap(mCaseInfo.Image3, tmp))
            {
                Icon3.Source = tmp.Source;
                Icon3.Focusable = false;
            }
            else
                Icon3.Source = null;

            if (loadBitmap(mCaseInfo.Image4, tmp))
            {
                Icon4.Source = tmp.Source;
                Icon4.Focusable = false;
            }
            else
                Icon4.Source = null;

            if (string.IsNullOrEmpty(mCaseInfo.ImageNP))
            {
                numberplateDrop();
                imgNP.Source = null;
            }
            else if (loadBitmap(mCaseInfo.ImageNP, tmp))
            {
                bdrNP.Visibility = Visibility.Visible;
                imgNP.Source = tmp.Source;
            }
            else
            {
                numberplateDrop();
                imgNP.Source = null;
            }

            tmp.Dispose();

            img2.Clip = null;

            img1.Focusable = imgNP.Focusable = imgBG.Focusable = false;
            img2.Focusable = true;

            this.Cursor = null;
        }

        public bool loadBitmap(string filename, ImageViewer.cImage image)
        {
            return loadBitmap(filename, image, null);
        }

        public bool loadBitmap(string filename, ImageViewer.cImage image, ImageViewer.cImage bindImage)
        {
            if (string.IsNullOrEmpty(filename))
            {
                image.Source = null;
                if (bindImage != null)
                    bindImage.Source = null;
                return false;
            }

            bool multi; // Not used in this application (multi frame tiff)

            if (!image.loadImage(filename, out multi))
            {
                MessageBox.Show("Could not load image '" + filename + "'. \n" + image.pError, "Image load error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (bindImage != null)
            {
                bindImage.Source = image.Source;
                image.bindImage(bindImage);
            }

            return true;
        }
        #endregion

        #region ----- Icons dropdown -----

        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (grdIcons.Visibility == Visibility.Visible)
            {
                ScaleTransform scale = (ScaleTransform)brdIcons.RenderTransform;
                scale.ScaleX = scale.ScaleY = 1;

                grdIcons.Visibility = Visibility.Collapsed;
                brdIcons.Height = btnCollapse.Height + 10;
            }
            else
            {
                grdIcons.Visibility = Visibility.Visible;
                brdIcons.Height = mIconsBorderHeight;
            }
        }

        private void Icon_MouseMove(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;

            Point pos = e.GetPosition(img);

            if (pos.X < (img.Width / 2))
                img.Cursor = Cursors.ScrollW;
            else
                img.Cursor = Cursors.ScrollE;
        }

        private void Polygon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (usingRemotePath)
            {
                mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
                mCaseInfo.Image2 = mCaseInfo.RemoteImage2;
                mCaseInfo.Image3 = mCaseInfo.RemoteImage3;
                mCaseInfo.Image4 = mCaseInfo.RemoteImage4;
            }

            Polygon p = (Polygon)sender;
            bool only2 = string.IsNullOrEmpty(mCaseInfo.Image3) && string.IsNullOrEmpty(mCaseInfo.Image4);  // Only 2 images exist

            only2 = false; // Do not swap images anymore

            if (mCaseInfo.OnlyOneImage)
                return;

            switch (p.Name)
            {
                case "L1":  // Left image 1
                    mPrintImageNo = 0;
                    img1.Source = Icon1.Source;
                    if (only2)
                        loadBitmap(mCaseInfo.Image2, img2, imgBG);
                    break;
                case "L2":
                    mPrintImageNo = 1;
                    img1.Source = Icon2.Source;
                    if (only2)
                        loadBitmap(mCaseInfo.Image1, img2, imgBG);
                    break;
                case "L3":
                    mPrintImageNo = 2;
                    if (Icon3.Source != null)
                        img1.Source = Icon3.Source;
                    break;
                case "L4":
                    mPrintImageNo = 3;
                    if (Icon4.Source != null)
                        img1.Source = Icon4.Source;
                    break;
                case "R1": // Right image 1
                    loadBitmap(mCaseInfo.Image1, img2, imgBG);
                    if (only2)
                        img1.Source = Icon2.Source;
                    break;
                case "R2":
                    loadBitmap(mCaseInfo.Image2, img2, imgBG);
                    if (only2)
                        img1.Source = Icon1.Source;
                    break;
                case "R3":
                    if (!string.IsNullOrEmpty(mCaseInfo.Image3))
                    {
                        loadBitmap(mCaseInfo.Image3, img2, imgBG);
                    }
                    break;
                case "R4":
                    if (!string.IsNullOrEmpty(mCaseInfo.Image4))
                    {
                        loadBitmap(mCaseInfo.Image4, img2, imgBG);
                    }
                    break;
            }

            mImagesChanged = true;
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (usingRemotePath)
            {
                mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
                mCaseInfo.Image2 = mCaseInfo.RemoteImage2;
                mCaseInfo.Image3 = mCaseInfo.RemoteImage3;
                mCaseInfo.Image4 = mCaseInfo.RemoteImage4;
            }

            Image i = (Image)sender;
            Point p = e.GetPosition(i);
            bool isLeft = (p.X < i.ActualWidth / 2);
            bool only2 = string.IsNullOrEmpty(mCaseInfo.Image3) && string.IsNullOrEmpty(mCaseInfo.Image4);  // Only 2 images exist

            only2 = false; // Do not swap images anymore

            if (mCaseInfo.OnlyOneImage)
                return;

            switch (i.Name)
            {
                case "Icon1":
                    if (isLeft)
                    {
                        mPrintImageNo = 0;
                        img1.Source = Icon1.Source;
                        if (only2)
                            loadBitmap(mCaseInfo.Image2, img2, imgBG);
                    }
                    else
                    {
                        loadBitmap(mCaseInfo.Image1, img2, imgBG);
                        if (only2)
                            img1.Source = Icon2.Source;
                    }
                    break;
                case "Icon2":
                    if (isLeft)
                    {
                        mPrintImageNo = 1;
                        img1.Source = Icon2.Source;
                        if (only2)
                            loadBitmap(mCaseInfo.Image1, img2, imgBG);
                    }
                    else
                    {
                        loadBitmap(mCaseInfo.Image2, img2, imgBG);
                        if (only2)
                            img1.Source = Icon1.Source;
                    }
                    break;
                case "Icon3":
                    if (isLeft)
                    {
                        mPrintImageNo = 2;
                        if (Icon3.Source != null)
                            img1.Source = Icon3.Source;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(mCaseInfo.Image3))
                        {
                            loadBitmap(mCaseInfo.Image3, img2, imgBG);
                        }
                    }
                    break;
                case "Icon4":
                    if (isLeft)
                    {
                        mPrintImageNo = 3;
                        if (Icon4.Source != null)
                            img1.Source = Icon4.Source;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(mCaseInfo.Image4))
                        {
                            loadBitmap(mCaseInfo.Image4, img2, imgBG);
                        }
                    }
                    break;
            }

            mImagesChanged = true;
        }

        private bool mBusyD = false;
        private void Icon_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (mBusyD)
                return;
            mBusyD = true;

            ScaleTransform scale = (ScaleTransform)brdIcons.RenderTransform;

            if ((e.Delta < 0) && (scale.ScaleX < 1 || scale.ScaleY < 1))
            {
                mBusyD = false;
                return;
            }

            if ((e.Delta > 0) && (scale.ScaleX > 3 || scale.ScaleY > 3))
            {
                mBusyD = false;
                return;
            }

            scale.ScaleX += (e.Delta > 0 ? 0.1 : -0.1);
            scale.ScaleY += (e.Delta > 0 ? 0.1 : -0.1);

            mBusyD = false;
        }

        #endregion

        #region ----- Zoom & Panning -----

        private void btnBInc_Click(object sender, RoutedEventArgs e)
        {
            img2.adjustBrightnessUp();

            numberplateClip();
        }

        private void btnBDec_Click(object sender, RoutedEventArgs e)
        {
            img2.adjustBrightnessDown();

            numberplateClip();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            img2.reloadImage(false);
        }

        private void btnPLeft_Click(object sender = null, RoutedEventArgs e = null)
        {
            img2.panLeft();
        }

        private void btnPRight_Click(object sender = null, RoutedEventArgs e = null)
        {
            img2.panRight();
        }

        private void btnPUp_Click(object sender = null, RoutedEventArgs e = null)
        {
            img2.panUp();
        }

        private void btnPDown_Click(object sender = null, RoutedEventArgs e = null)
        {
            img2.panDown();
        }

        private void btnZFit_Click(object sender, RoutedEventArgs e)
        {
            img2.resetTransformation();
        }

        private void btnZIn_Click(object sender, RoutedEventArgs e)
        {
            img2_Zoom(true);
        }

        private void btnZOut_Click(object sender, RoutedEventArgs e)
        {
            img2_Zoom(false);
        }

        private void img2_Zoom(bool doIn)
        {
            if (doIn)
                img2.zoomIn(recMask.Margin.Left + (recMask.Width / 2), recMask.Margin.Top + (recMask.Height / 2));
            else
                img2.zoomOut(recMask.Margin.Left + (recMask.Width / 2), recMask.Margin.Top + (recMask.Height / 2));
        }

        private void img2Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if ((e.Key == Key.Left) || (e.Key == Key.Right) || (e.Key == Key.Up) || (e.Key == Key.Down))
            //{
            //    img2Grid_KeyDown(sender, e);
            //    ////////////img2Grid.Focus();
            //}
        }


        private void img2Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                img2.resetTransformation();

            if (e.Key == Key.Back)
                img2.reloadImage(false);

            if (e.Key == Key.Left || e.Key == Key.L)
                btnPLeft_Click();
            else if (e.Key == Key.Right || e.Key == Key.R)
                btnPRight_Click();
            else if (e.Key == Key.Up || e.Key == Key.U)
                btnPUp_Click();
            else if (e.Key == Key.Down || e.Key == Key.D)
                btnPDown_Click();

            if (e.Key == Key.PageUp)
                img2.switchImage(true);
            else if (e.Key == Key.PageDown)
                img2.switchImage(false);

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (e.Key == Key.OemPlus || e.Key == Key.Add)
                    img2_Zoom(true);
                else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
                    img2_Zoom(false);
                else if (e.Key == Key.B)
                    img2.adjustBrightnessUp();
                else if (e.Key == Key.C)
                    img2.adjustContrastUp();

                return;
            }

            if (e.Key == Key.B)
                img2.adjustBrightnessDown();
            else if (e.Key == Key.C)
                img2.adjustContrastDown();

            if (e.Key == Key.OemPlus || e.Key == Key.Add)
                img2_Zoom(true);
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
                img2_Zoom(false);
        }
        #endregion

        private void btnShowVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (usingRemotePath || !File.Exists(mCaseInfo.Image1))
                {
                    mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
                    //usingRemotePath = true;
                }
                else
                {
                    usingRemotePath = false;
                }

                string filePath = mCaseInfo.Image1;
                //string filePath = @"C:\ENC\LuAnn\Not Showing in Session\Img0178s.enc";

                var camera = CameraFactory.Find();
                if (camera == null)
                {
                    return;
                }

                Configuration configuration = new Configuration();

                string videoPath;
                videoPath = camera.ExtractInfringementVideo(filePath, configuration.GetDLLPath());
                if (videoPath != string.Empty)
                {
                    VideoViewer videoViewer = new VideoViewer();
                    videoViewer.LoadVideo(videoPath);
                    videoViewer.ShowDialog();
                    File.Delete(videoPath);
                }
                else
                {
                    MessageBox.Show("No Video File found.");
                }
            }
            catch (GatewayException gex)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Failed to load window from{0} - {1}", "OtherWindow", gex.Message));
                throw new Exception(String.Format("Failed to load window from{0} - {1}", "OtherWindow", gex.Message), gex);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Failed to load window from{0} - {1}", "OtherWindow", ex.Message));
                throw new Exception(String.Format("Failed to load window from{0} - {1}", "OtherWindow", ex.Message), ex);

            }

        }

    }
}
