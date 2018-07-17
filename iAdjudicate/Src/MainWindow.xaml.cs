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
using TMT.Core.Camera.Interfaces;
using TMT.Core.Components;
using Kapsch.ITS.Gateway.Models.Adjudicate;
using Kapsch.ITS.Gateway.Clients;
using Kapsch.ITS.App;
using Kapsch.Gateway.Models.Shared;
using MahApps.Metro.Controls;

namespace TMT.iAdjudicate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //private cDataAccess mDataAccess = null;
        private CaseModel mCaseInfo;
        private string mUserName = string.Empty;
        private int mTicketCount = 0;
        private IList<RejectReasonModel> mReasons = null;
        private IList<OffenceCodeModel> mAdditionals = new List<OffenceCodeModel>();
        private bool mNewCases = true;  // New cases or Fishpond cases
        private AdjudicateService adjudicateService = null;
        private bool usingRemotePath = false;
        private bool usingRemoteNPPath = false;

        public MainWindow()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.Opacity = 0.5;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblVersion.Content = string.Empty;
            lblVersion.ToolTip = string.Empty;

            this.Cursor = Cursors.Wait;
            displayClear();

            try
            {
                adjudicateService = new AdjudicateService(ApplicationSession.AuthenticatedUser.SessionToken);

            }
            catch (GatewayException gex)
            {
                MessageBox.Show(this, "Session token not found or has expired. \n\n" + gex.Message, "iAdjudicate", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Session token not found or has expired. \n\n" + ex.Message, "iAdjudicate", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            mCaseInfo = caseInitialise();

            try
            {
                mUserName = ApplicationSession.AuthenticatedUser.UserData.UserName;
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
               
                var askFishpond = new cFishpondPrompt();
                    askFishpond.Owner = this;
                    if (askFishpond.ShowDialog() == false)
                    {
                        this.Close();
                        return;
                    }

                    mNewCases = askFishpond.pNewCases;

                  if (mNewCases) // New cases
                  {
                        try
                        {
                            var firstCaseModel = adjudicateService.GetFirstCase(Environment.MachineName);

                            if (!File.Exists(firstCaseModel.Case.Image1))
                            {
                                usingRemotePath = true;
                            firstCaseModel.Case.Image1 = firstCaseModel.Case.RemoteImage1;
                            firstCaseModel.Case.Image2 = firstCaseModel.Case.RemoteImage2;

                            }
                            else
                            {
                                usingRemotePath = false;
                            }

                            if (!File.Exists(firstCaseModel.Case.ImageNP))
                            {
                                usingRemoteNPPath = true;
                            firstCaseModel.Case.ImageNP = firstCaseModel.Case.RemoteImageNP;
                            }
                            else
                            {
                                usingRemoteNPPath = false;
                            }

                            mReasons = firstCaseModel.RejectReasons;
                            mTicketCount = firstCaseModel.TicketCount;
                            mCaseInfo = firstCaseModel.Case;
                       
                            if (displayUpdate())
                                buttonsEnable(true);
                        }
                        catch (GatewayException gex)
                        {
                            MessageBox.Show(this, "Could not get any tickets. \n" + gex.Message, "Adjudicate tickets", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, "Could not get any tickets. \n" + ex.Message, "Adjudicate tickets", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                  }
                    else // Fishpond cases
                    {
                        this.Title += " - Fishpond Cases";

                        bool doExit;
                        bool okay;

                        okay = fishpondChooseCase(out mCaseInfo, out doExit);

                        if (doExit)
                        {
                            //mDataAccess.disconnect();
                            this.Close();
                            return;
                        }
                        else if (okay)
                        {
                            if (displayUpdate())
                                buttonsEnable(true);
                        }
                    }
                //}

            this.Opacity = 1;
            this.Cursor = null;
            buttonAccept.Focus();
        }

        private int mLastSortOrder = 1;                 // Remember the sorting order
        private string mLastSortColumn = string.Empty;  // Remember the column sorted on
        private int mLastSelectNo = 0;                  // Remember the last selected index

        public CaseModel caseInitialise()
        {
            CaseModel caseInfo = new CaseModel();

            caseInfo.TicketNo = string.Empty;
            caseInfo.OffenceSet = -1;
            caseInfo.Notification = string.Empty;
            caseInfo.Image1 = string.Empty;
            caseInfo.Image2 = string.Empty;
            caseInfo.RemoteImage1 = string.Empty;
            caseInfo.RemoteImage2 = string.Empty;
            //caseInfo.Image3 = string.Empty;
            //caseInfo.Image4 = string.Empty;
            caseInfo.ImageNP = string.Empty;
            caseInfo.RemoteImageNP = string.Empty;
            caseInfo.VehicleRegNo = string.Empty;
            caseInfo.VehicleRegNoConfirmed = false;
            caseInfo.VehicleMake = string.Empty;
            caseInfo.VehicleModel = string.Empty;
            caseInfo.VehicleColour = string.Empty;
            caseInfo.VehicleType = string.Empty;
            caseInfo.VehicleLicenseExpire = DateTime.MinValue;
            caseInfo.OffenceDate = DateTime.MinValue;
            caseInfo.OffenceSpeed = -1;
            caseInfo.OffenceZone = -1;
            caseInfo.OffenceDirectionLane = string.Empty;
            caseInfo.OffenceCode = -1;
            caseInfo.OffenceNotes = string.Empty;
            caseInfo.OffenceAdditionalsXml = string.Empty;
            //caseInfo.OnlyOneImage = false;

            return caseInfo;
        }

        /// <summary>
        /// Get a ticket from fishpond.
        /// </summary>
        /// <param name="caseInfo">Returns case</param>
        /// <param name="doExit">User chose to exit</param>
        /// <returns>True if all okay</returns>
        private bool fishpondChooseCase(out CaseModel caseInfo, out bool doExit)
        {
            caseInfo = caseInitialise();
            doExit = false;

            IList<FishpondCaseModel> cases = null;

            try
            {
                var fishpondCases = adjudicateService.GetFishpondCases(Environment.MachineName);

                cases = fishpondCases.Cases;
                mReasons = fishpondCases.RejectReasons;

            }
            catch (GatewayException gex)
            {
                MessageBox.Show(this, "Could not get any tickets. \n\n" + gex.Message, "Adjudicate tickets", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not get any tickets. \n\n" + ex.Message, "Adjudicate tickets", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            //if (!mDataAccess.GetFishpondCases(ref mReasons, ref cases))

            var fishpondDialog = new cFishpondList();

            fishpondDialog.Owner = this;
            fishpondDialog.pFishpondCases = cases;
            fishpondDialog.pSelectIndex = mLastSelectNo;
            fishpondDialog.pSortColumn = mLastSortColumn;
            fishpondDialog.pSortOrder = mLastSortOrder;

            fishpondDialog.ShowDialog();

            doExit = fishpondDialog.pDoExit;

            if (!doExit)
            {
                lblTicketNumber.Content =
                caseInfo.TicketNo = fishpondDialog.pTicketNo;
                mLastSortColumn = fishpondDialog.pSortColumn;
                mLastSelectNo = fishpondDialog.pSelectIndex;
                mLastSortOrder = fishpondDialog.pSortOrder;

                try
                {
                    var fishpondCase = adjudicateService.GetFishpondCase(caseInfo.TicketNo, Environment.MachineName);
                    caseInfo = fishpondCase;
                }
                catch (GatewayException gex)
                {
                    MessageBox.Show(this, "Could not get ticket '" + fishpondDialog.pTicketNo + "'. \n\n" + gex.Message, "Adjudicate tickets", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not get ticket '" + fishpondDialog.pTicketNo + "'. \n\n" + ex.Message, "Adjudicate tickets", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;

                }
                //TODO: Sien vorige todo oor fishpondGetCase
                //if (!mDataAccess.fishpondGetCase(fishpondDialog.pTicketNo, ref mCaseInfo))
                //{
                //    
                //}
            }

            return true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(mCaseInfo.TicketNo))
            {

                if (mNewCases) // Do not do this for Fishpond cases
                {
                    try
                    {
                        adjudicateService.UnlockCase(mCaseInfo.TicketNo);
                        
                       // if (!mDataAccess.caseUnlock(mCaseInfo.TicketNo)) 
                    }
                    catch (GatewayException gex)
                    {
                        MessageBox.Show(this, "Could not unlock last ticket. \n" + gex.Message, "Adjudicate unlock", MessageBoxButton.OK, MessageBoxImage.Error);
                        buttonsEnable(true);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Could not unlock last ticket. \n" + ex.Message, "Adjudicate unlock", MessageBoxButton.OK, MessageBoxImage.Error);
                        buttonsEnable(true);

                    }
                }
                else
                {
                    try
                    {
                        adjudicateService.UnlockFishpondCase(mCaseInfo.TicketNo);
                        //if (!mDataAccess.fishpondUnlock(mCaseInfo.TicketNo))
                        
                    }
                    catch (GatewayException gex)
                    {
                        MessageBox.Show(this, "Could not unlock fishpond ticket. \n" + gex.Message, "Adjudicate unlock", MessageBoxButton.OK, MessageBoxImage.Error);
                        buttonsEnable(true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Could not unlock fishpond ticket. \n" + ex.Message, "Adjudicate unlock", MessageBoxButton.OK, MessageBoxImage.Error);
                        buttonsEnable(true);
                    }
                   
                  
                }

                    //mDataAccess.disconnect();
            }
        }

        private void buttonViewEditNotes_Click(object sender, RoutedEventArgs e)
        {
            buttonViewEditNotes.IsEnabled = false;

            var notes = new cNotes();
            notes.Owner = this;
            notes.pTicketNumber = mCaseInfo.TicketNo;
            notes.pNotes = mCaseInfo.OffenceNotes;
            notes.ShowDialog();

            buttonViewEditNotes.ToolTip = mCaseInfo.OffenceNotes = notes.pNotes;

            buttonViewEditNotes.IsEnabled = true;
        }

        private void buttonAddAdditional_Click(object sender, RoutedEventArgs e)
        {
            buttonAddAdditional.IsEnabled = false;

                IList<OffenceCodeModel> codes;
            try
            {
                var additionals = adjudicateService.GetAdditionals(mCaseInfo.OffenceSet);
                codes = additionals;

                var adds = new cAdditionals();
                adds.Owner = this;
                adds.pAdditionalCodes = codes;
                adds.pOffenceItems = mAdditionals;

                if (adds.ShowDialog() == true)
                {
                    mAdditionals = adds.pOffenceItems;
                    labelAdditionals.Content = "Additionals " + mAdditionals.Count;
                }
            }
            catch (GatewayException gex)
            {
                MessageBox.Show(this, "Could not get Offence Codes. \n" + gex.Message, "Offence Codes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not get Offence Codes. \n" + ex.Message, "Offence Codes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
                //if (!mDataAccess.caseAdditionals(mCaseInfo.OffenceSet, out codes))
                  
                   
                

                //mDataAccess.disconnect();

            buttonAddAdditional.IsEnabled = true;
        }

        private void buttonsEnable(bool doEnable)
        {
            buttonAccept.IsEnabled = buttonAddAdditional.IsEnabled = buttonViewEditNotes.IsEnabled = doEnable;
            buttonReject.IsEnabled = true;
        }

        private void displayClear()
        {
            buttonsEnable(false);

            lblTicketNumber.Content = mUserName + " adjudicating...";
            lblFileLocation.Content = 
            lblNotification.Content = string.Empty;

            textBoxRegNum.Text = 
            textBoxVMake.Text = 
            textBoxVModel.Text = 
            textBoxVColor.Text = 
            textBoxVType.Text = string.Empty;

            textBoxODate.Text = 
            textBoxOSpeed.Text = 
            textBoxOZone.Text = 
            textBoxODir.Text = 
            textBoxOffence.Text =
            textBoxLExpire.Text = string.Empty;

            buttonViewEditNotes.ToolTip = string.Empty;

            cPictureDisplayLeft.releaseImage();
            cPictureDisplayRight.releaseImage();
            cPictureDisplayNP.releaseImage();
        }

        private bool displayUpdate()
        {
            bool multiPage;

            lblTicketNumber.Content = mUserName + " adjudicating ticket " + mCaseInfo.TicketNo;

            if (usingRemotePath || !File.Exists(mCaseInfo.Image1))
            {
                mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
            }
            else
            {
                usingRemotePath = false;
            }

            if (!string.IsNullOrEmpty(mCaseInfo.Image1))
                lblFileLocation.Content = System.IO.Path.GetDirectoryName(mCaseInfo.Image1);
            else
            {
                lblFileLocation.Content = "Image not found";
            }

            lblNotification.Content = mCaseInfo.Notification;
            lblTickets.Content = mTicketCount.ToString();

            textBoxRegNum.Text = mCaseInfo.VehicleRegNo;
            textBoxRegNum.Background = mCaseInfo.VehicleRegNoConfirmed ? Brushes.Green : Brushes.OrangeRed;
            textBoxVMake.Text = mCaseInfo.VehicleMake;
            textBoxVModel.Text = mCaseInfo.VehicleModel;
            textBoxVColor.Text = mCaseInfo.VehicleColour;
            textBoxVType.Text = mCaseInfo.VehicleType;

            textBoxODate.Text = mCaseInfo.OffenceDate.ToString(@"dd/MM/yyyy HH:mm");
            textBoxOSpeed.Text = mCaseInfo.OffenceSpeed.ToString();
            textBoxOZone.Text = mCaseInfo.OffenceZone.ToString();
            textBoxODir.Text = mCaseInfo.OffenceDirectionLane;
            textBoxOffence.Text = mCaseInfo.OffenceCode.ToString();
            textBoxLExpire.Text = mCaseInfo.VehicleLicenseExpire == DateTime.MinValue ? string.Empty : mCaseInfo.VehicleLicenseExpire.ToString(@"dd/MM/yyyy");
            textBoxLExpire.Foreground = (mCaseInfo.VehicleLicenseExpire.AddDays(21) < DateTime.Now) ? Brushes.OrangeRed : textBoxOffence.Foreground;

            buttonViewEditNotes.ToolTip = mCaseInfo.OffenceNotes;

            if (usingRemotePath || !File.Exists(mCaseInfo.Image1))
            {
                mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
                mCaseInfo.Image2 = mCaseInfo.RemoteImage2;
            }
            else
            {
                usingRemotePath = false;
            }

            if (!cPictureDisplayLeft.loadPicture(mCaseInfo.Image1, false, out multiPage))
            {
                MessageBox.Show(this, "Could not display Main image. \n" + cPictureDisplayLeft.pError, "Image display", MessageBoxButton.OK, MessageBoxImage.Error);
                //return false; 
            }

            if (!cPictureDisplayRight.loadPicture(mCaseInfo.Image2, false, out multiPage))
            {
                MessageBox.Show(this, "Could not display secondary image. \n" + cPictureDisplayLeft.pError, "Image display", MessageBoxButton.OK, MessageBoxImage.Error);
                //return false;
            }

            if (usingRemoteNPPath || !File.Exists(mCaseInfo.ImageNP))
            {
                mCaseInfo.ImageNP = mCaseInfo.RemoteImageNP;
            }
            else
            {
                usingRemoteNPPath = false;
            }

            if (string.IsNullOrEmpty(mCaseInfo.ImageNP))
                cPictureDisplayNP.releaseImage();
            else
            {
                if (!cPictureDisplayNP.loadPicture(mCaseInfo.ImageNP, true, out multiPage))
                {
                    MessageBox.Show(this, "Could not display Number plate image. \n" + cPictureDisplayLeft.pError + " \n" + mCaseInfo.ImageNP, "Image display", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

           // buttonImages.IsEnabled = !string.IsNullOrEmpty(mCaseInfo.Image3);

            return true;
        }

        private void buttonReject_Click(object sender, RoutedEventArgs e)
        {
            buttonsEnable(false);

            cReasons reas = new cReasons();
            reas.pReasons = mReasons;
            reas.pTicketNumber = mCaseInfo.TicketNo;
            reas.Owner = this;
            reas.pAdditionals = mAdditionals.Count;

            if (reas.ShowDialog() == true)
            {
                    bool okay = true;

                    if (mNewCases) // New cases
                    {
                        try
                        {
                            var rejectedCase = adjudicateService.RejectCase(mCaseInfo.TicketNo, reas.pReasonID, Environment.MachineName);
                            mCaseInfo = rejectedCase.Case;
                            mTicketCount = rejectedCase.TicketCount;
                        }
                    catch (GatewayException gex)
                    {
                        if (gex.Message.Contains("20034:"))
                        {
                            MessageBox.Show(this, "Finished with Adjudication. \n" + gex.Message, "Adjudicate", MessageBoxButton.OK, MessageBoxImage.Error);
                            buttonsEnable(true);
                            okay = false;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(this, "Could not Reject offence. \n" + gex.Message, "Reject error", MessageBoxButton.OK, MessageBoxImage.Error);
                            buttonsEnable(true);
                            okay = false;
                        }
                    }
                    catch (Exception ex)
                        {
                            MessageBox.Show(this, "Could not Reject offence. \n" + ex.Message, "Reject error", MessageBoxButton.OK, MessageBoxImage.Error);
                            buttonsEnable(true);
                            okay = false;
                        }
                    }

                    else  // Fishpond cases
                    {
                        try
                        {
                           adjudicateService.RejectFishpondCase(mCaseInfo.TicketNo, reas.pReasonID, mCaseInfo.OffenceNotes, Environment.MachineName);

                            bool doExit;

                            okay = fishpondChooseCase(out mCaseInfo, out doExit);

                            if (doExit)
                            {
                                //mDataAccess.disconnect();
                                this.Close();
                                return;
                            }
                        }
                    catch (GatewayException gex)
                    {
                        MessageBox.Show(this, "Could not Reject offence. \n" + gex.Message, "Reject error", MessageBoxButton.OK, MessageBoxImage.Error);
                        buttonsEnable(true);
                        okay = false;
                    }
                    catch (Exception ex)
                        {
                            MessageBox.Show(this, "Could not Reject offence. \n" + ex.Message, "Reject error", MessageBoxButton.OK, MessageBoxImage.Error);
                            buttonsEnable(true);
                            okay = false;
                        }  
                    }

                    //mDataAccess.disconnect();

                    if (okay)
                    {
                        if (displayUpdate())
                        {
                            buttonsEnable(true);
                            labelAdditionals.Content = "Additionals 0";
                            mAdditionals.Clear();
                        }
                    }
            }
            else
                buttonsEnable(true);
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            buttonsEnable(false);

            string prompt = "Are you sure you want to Accept '" + mCaseInfo.TicketNo + "'? \n";
            if (mAdditionals.Count > 0)
                prompt += "Added " + mAdditionals.Count + " additional charges."; //, amount " + mAdditionalsAmount.ToString("0.00");
            else
                prompt += "No additional charges added.";

            if (MessageBox.Show(this, prompt, "Adjudicate", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
       
                    if (mAdditionals.Count > 0)
                    {
                        mCaseInfo.OffenceAdditionalsXml = "<Details> ";
                        for (int i = 0; i < mAdditionals.Count; i++)
                        {
                            mCaseInfo.OffenceAdditionalsXml += "<Detail><Code>" + mAdditionals[i].Code + "</Code></Detail> ";
                        }
                        mCaseInfo.OffenceAdditionalsXml += "</Details>";
                    }
                    else
                    { 
                        mCaseInfo.OffenceAdditionalsXml = string.Empty;
                    }

                bool okay = true;

                    if (mNewCases) // New cases
                    {

                    try
                    {
                        var adjudicatedCase = adjudicateService.AcceptCase(mCaseInfo, Environment.MachineName);

                        mCaseInfo = adjudicatedCase.Case;
                        mTicketCount = adjudicatedCase.TicketCount;
                    }
                    catch (GatewayException gex)
                    {
                        if (gex.Message.Contains("20034:"))
                        {
                            MessageBox.Show(this, "Finished with Adjudication. \n" + gex.Message, "Adjudicate", MessageBoxButton.OK, MessageBoxImage.Error);
                            buttonsEnable(true);
                            okay = false;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(this, "Could not save Adjudication. \n" + gex.Message, "Adjudicate error", MessageBoxButton.OK, MessageBoxImage.Error);
                            buttonsEnable(true);
                            okay = false;
                        }
                    }
                    catch (Exception ex)
                        {
                        
                            MessageBox.Show(this, "Could not save Adjudication. \n" + ex.Message, "Adjudicate error", MessageBoxButton.OK, MessageBoxImage.Error);
                            buttonsEnable(true);
                            okay = false;
                        }
                    }
            
                    else  // Fishpond
                    {
                        bool doExit;
                        try
                        {
                        //if (!mDataAccess.fishpondCaseAccept(mCaseInfo))
                        adjudicateService.AcceptFishpondCase(mCaseInfo, Environment.MachineName);

                        okay = fishpondChooseCase(out mCaseInfo, out doExit);

                            if (doExit)
                            {
                                //mDataAccess.disconnect();
                                this.Close();
                                return;
                            }
                        }
                    catch (GatewayException gex)
                    {
                        MessageBox.Show(this, "Could not save Fishpond Adjudication. \n" + gex.Message, "Adjucate error", MessageBoxButton.OK, MessageBoxImage.Error);
                        okay = false;
                    }
                    catch (Exception ex)
                        {
                            MessageBox.Show(this, "Could not save Fishpond Adjudication. \n" + ex.Message, "Adjudicate error", MessageBoxButton.OK, MessageBoxImage.Error);
                            okay = false;
                        }
                        
       
                    }

                    if (okay)
                    {
                        if (displayUpdate())
                        {
                            buttonsEnable(true);
                            labelAdditionals.Content = "Additionals 0";
                            mAdditionals.Clear();
                        }
                    }

                    //mDataAccess.disconnect();
            }
            else
                buttonsEnable(true);
        }

        private void buttonImages_Click(object sender, RoutedEventArgs e)
        {
            buttonImages.IsEnabled = false;
            cShowImages si = new cShowImages();

            if (usingRemotePath || !File.Exists(mCaseInfo.Image1))
            {
                mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
                mCaseInfo.Image2 = mCaseInfo.RemoteImage2;
            }
            else
            {
                usingRemotePath = false;
            }

            si.Owner = this;
            si.pImages[0] = mCaseInfo.Image1;
            si.pImages[1] = mCaseInfo.Image2;
            //si.pImages[1] = (mCaseInfo.mOnlyOneImage ? mCaseInfo.Image1 : mCaseInfo.Image2);
            //si.pImages[2] = mCaseInfo.mImage3;
            //si.pImages[3] = mCaseInfo.mImage4;

            si.ShowDialog();

            buttonImages.IsEnabled = true;
        }
                
        private void buttonShowVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (usingRemotePath || !File.Exists(mCaseInfo.Image1))
                {
                    mCaseInfo.Image1 = mCaseInfo.RemoteImage1;
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
