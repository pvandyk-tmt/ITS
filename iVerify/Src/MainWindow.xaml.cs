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
using MahApps.Metro.Controls;
using Kapsch.ITS.Gateway.Models.Verify;
using Kapsch.ITS.App;
using Kapsch.ITS.Gateway.Clients;

namespace TMT.iVerify
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
        private IList<CaptureTypeModel> mTypes = null;
        private Thickness mThick1;
        private Thickness mThick1a;
        private Thickness mThick2;
        private Thickness mThick2a;
        private double mIconsBorderHeight = 192;    // Icons border height, get read at loaded event and used to collapse & re-expand control
        private bool mIgnoreEvents = true;
        private bool mVechileRegChanged = false;
        private bool mPersonChanged = false;
        private bool mAddresChanged = false;
        private bool mTypeChanged = false;
        private bool mNotesChanged = false;
        private bool mImagesChanged = false;
        private int mPrintImageNo = 0;
        private cFishpondPrompt.eLoadTypes mCaseType = cFishpondPrompt.eLoadTypes.None;
        private VerifyService verifyService = null;


        public MainWindow()
        {
            InitializeComponent();

            mThick1 = new Thickness(1);
            mThick1a = new Thickness(1, 1, 1, 0);
            mThick2 = new Thickness(2);
            mThick2a = new Thickness(2, 2, 2, 0);

            mReasons = new List<RejectReasonModel>();
            mTypes = new List<CaptureTypeModel>();

            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            recMask.Visibility = Visibility.Hidden;
            this.Opacity = 0.5;

            IsNATISDataVisible(false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                verifyService = new VerifyService(ApplicationSession.AuthenticatedUser.SessionToken);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Session token not found or has expired. \n\n" + ex.Message, "iVerify", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            ClearValue(SizeToContentProperty);
            LayoutRoot.ClearValue(WidthProperty);
            LayoutRoot.ClearValue(HeightProperty);

            ScaleTransform scale = new ScaleTransform(1, 1);
            brdIcons.RenderTransform = scale;
            brdIcons.RenderTransformOrigin = new Point(0.7, 0.05);

            mTypeChanged = true;  // Always send the type in as if changed.  Pieter changed SP in DB for reps.

            mIconsBorderHeight = brdIcons.ActualHeight;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion.ToString();

            lblVersion.Content = version;

            recMask.Height = Properties.Settings.Default.NumberplateHeight;
            recMask.Width = Properties.Settings.Default.NumberplateWidth;
            bdrNP.Height = recMask.Height + 2; // Padding 1 all around
            bdrNP.Width = recMask.Width + 2; // Padding 1 all around

            this.Cursor = Cursors.Wait;

            //mDataAccess = new cDataAccess(Properties.Settings.Default.ConnectionString);
            //mDataAccess.caseInitialise(out mCaseInfo);

            mCaseInfo = CaseInitialise();
            displayUpdate();

            img2.evOnMouseDown += img2_evOnMouseDown;
            img2.evOnAfterTransformation += img2_evOnAfterTransformation;

            try
            {
                mUserName = ApplicationSession.AuthenticatedUser.UserData.UserName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Username not found. \n\n" + ex.Message, "iVerify", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            var askFishpond = new cFishpondPrompt();
            askFishpond.Owner = this;
            if (askFishpond.ShowDialog() == false)
               {
                  this.Close();
                  return;
               }

            mCaseType = askFishpond.pCaseType;

            if (mCaseType == cFishpondPrompt.eLoadTypes.NewCases) // New cases
            {
                this.Title += " - New Cases";

                try
                {
                    var firstCaseModel = verifyService.GetFirstCase();
                    mReasons = firstCaseModel.RejectReasons;
                    mTypes = firstCaseModel.CaptureTypes;
                    mTicketCount = firstCaseModel.Count;
                    mCaseInfo = firstCaseModel.Case;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not get any tickets. \n" + ex.Message, "Verify tickets", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

                if (string.IsNullOrEmpty(mCaseInfo.TicketNo)) { }
                //Sit later terug:                    this.IsEnabled = false;
                else
                {
                    displayUpdate();
                    showImages();
                }
            }
            else if (mCaseType == cFishpondPrompt.eLoadTypes.Fishpond) // Fishpond cases
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
                    displayUpdate();
                    showImages();
                }
            }
            else if (mCaseType == cFishpondPrompt.eLoadTypes.Summons) // Summons
            {
                this.Title += " - Summons Cases";

                //bool doExit;
                //bool okay;

                try
                {
                    var lockedCase = verifyService.LockSummons(mCaseInfo);
                    mCaseInfo = lockedCase;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not lock Summons feeder. \n" + ex.Message, "DB connect error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                displayUpdate();
               
                //if (doExit)
                //{
                //    mDataAccess.disconnect();
                //    this.Close();
                //    return;
                //}
                //else if (okay)
                //{
                //    displayUpdate();
                //    showImages();
                //}

            }

            this.Opacity = 1;
            this.Cursor = null;

            buttonsCheckEnable();
            comboBoxType.Focus();

            mIgnoreEvents = false;
        }

        private CaseModel CaseInitialise()
        {
            CaseModel caseInfo = new CaseModel();

            caseInfo.TicketNo = string.Empty;
            caseInfo.Image1 = string.Empty;
            caseInfo.Image2 = string.Empty;
            caseInfo.Image3 = string.Empty;
            caseInfo.Image4 = string.Empty;
            caseInfo.ImageNP = string.Empty;
            caseInfo.Image1ID =
            caseInfo.Image2ID =
            caseInfo.Image3ID =
            caseInfo.Image4ID = -1;
            caseInfo.PrintImageNo = 0;
            caseInfo.OnlyOneImage = false;
            caseInfo.VehicleRegNo = string.Empty;
            caseInfo.VehicleRegNoConfirmed = false;
            caseInfo.VehicleMake = string.Empty;
            caseInfo.VehicleModel = string.Empty;
            caseInfo.VehicleColour = string.Empty;
            caseInfo.VehicleType = string.Empty;
            caseInfo.VehicleCaptureType = string.Empty;
            caseInfo.OffenceDate = DateTime.MinValue;
            caseInfo.OffenceOldNotes = string.Empty;
            caseInfo.OffenceNewNotes = string.Empty;

            caseInfo.PersonName =
            caseInfo.PersonSurname =
            caseInfo.PersonMiddleNames =
            caseInfo.PersonTelephone =
            caseInfo.PersonID = string.Empty;
            caseInfo.PersonKey =
            caseInfo.PersonPhysicalAddressKey =
            caseInfo.PersonPostalAddressKey = -1;

            caseInfo.UseGismoAddress = false;

            caseInfo.NatisPhysical = 
            caseInfo.NatisPostal =
            caseInfo.SystemPhysical = 
            caseInfo.SystemPostal = addressInitialise();

            return caseInfo;
            
        }

        private AddressInfoModel addressInitialise()
        {
            AddressInfoModel addr = new AddressInfoModel();

            addr.ResidualScore =
            addr.Key = -1;
            addr.Street =
            addr.Suburb =
            addr.Town =
            addr.POBox =
            addr.Residual =
            addr.Code = string.Empty;
            addr.Date = DateTime.MinValue;

            return addr;
        }

        private int mLastSortOrder = 1;                 // Remember the sorting order
        private string mLastSortColumn = string.Empty;  // Remember the column sorted on
        private int mLastSelectNo = 0;                  // Remember the last selected index

        /// <summary>
        /// Get a ticket from fishpond.
        /// </summary>
        /// <param name="caseInfo">Returns case</param>
        /// <param name="doExit">User chose to exit</param>
        /// <returns>True if all okay</returns>
        private bool fishpondChooseCase(out CaseModel caseInfo, out bool doExit)
        {
            caseInfo = CaseInitialise();
            doExit = false;

            IList<FishpondInfoModel> cases = null;

            try
            {
                var fishpondCases = verifyService.GetFirstFishpondCase();

                cases = fishpondCases.Cases;
                mReasons = fishpondCases.RejectReasons;

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not get any tickets. \n\n" + ex.Message, "Verify tickets", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

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
                    var fishpondCase = verifyService.GetFishpondCase(fishpondDialog.pTicketNo);

                    mCaseInfo = fishpondCase.Case;
                    mTypes = fishpondCase.CaptureTypes;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not get ticket '" + fishpondDialog.pTicketNo + "'. \n\n" + ex.Message, "Verify tickets", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            img2.evOnMouseDown -= img2_evOnMouseDown;
            img2.evOnAfterTransformation -= img2_evOnAfterTransformation;

            if (!string.IsNullOrEmpty(mCaseInfo.TicketNo) || mCaseType == cFishpondPrompt.eLoadTypes.Summons)
            {

                if (mCaseType == cFishpondPrompt.eLoadTypes.NewCases)
                {
                    try
                    {
                        verifyService.UnlockCase(mCaseInfo.TicketNo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Could not unlock last ticket. \n" + ex.Message, "Verify unlock", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else if (mCaseType == cFishpondPrompt.eLoadTypes.Fishpond)
                {
                    try
                    {
                        verifyService.UnlockFishpondCase(mCaseInfo.TicketNo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Could not unlock fishpond ticket. \n" + ex.Message, "Verify unlock", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (mCaseType == cFishpondPrompt.eLoadTypes.Summons)
                {
                    try
                    {
                        verifyService.UnlockSummons();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Could not unlock summons. \n" + ex.Message, "Verify unlock", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void displayAddresses()
        {
            
            //if (mDataAccess.pAddressIndicator == false)
            //{
            //    lblVerificationDone.Content = "Address needs to be Verified";
            //    lblVerificationDone.Background = Brushes.Red;
            //}
            //else
            //{
            //    lblVerificationDone.Content = "Address has been recently Verified. No update from eNaTIS received.";
            //    lblVerificationDone.Background = Brushes.Green;
            //}

            textBoxSystemPhyStreet.Text = mCaseInfo.SystemPhysical.Street;
            textBoxSystemPhySuburb.Text = mCaseInfo.SystemPhysical.Suburb;
            textBoxSystemPhyTown.Text = mCaseInfo.SystemPhysical.Town;
            textBoxSystemPhyPOBox.Text = mCaseInfo.SystemPhysical.POBox;
            textBoxSystemPhyCode.Text = mCaseInfo.SystemPhysical.Code;
            textBoxResidualPhysical.Text = mCaseInfo.SystemPhysical.Residual;
            bool checkAddress = false;
            if (mCaseInfo.SystemPhysical.ResidualScore == 1)
            {
                textBoxResidualPhysical.Background = Brushes.Orange;
                checkAddress = true;
            }
            else if (mCaseInfo.SystemPhysical.ResidualScore == 2)
            {
                textBoxResidualPhysical.Background = Brushes.Red;
                checkAddress = true;
            }
            else // Score == 0 or anything else
            {
                textBoxResidualPhysical.Background = Brushes.Green;
            }

            textBoxSystemPosStreet.Text = mCaseInfo.SystemPostal.Street;
            textBoxSystemPosSuburb.Text = mCaseInfo.SystemPostal.Suburb;
            textBoxSystemPosTown.Text = mCaseInfo.SystemPostal.Town;
            textBoxSystemPosPOBox.Text = mCaseInfo.SystemPostal.POBox;
            textBoxSystemPosCode.Text = mCaseInfo.SystemPostal.Code;
            textBoxResidualPostal.Text = mCaseInfo.SystemPostal.Residual;
            if (mCaseInfo.SystemPostal.ResidualScore == 1)
            {
                textBoxResidualPostal.Background = Brushes.Orange;
                checkAddress = true;
            }
            else if (mCaseInfo.SystemPostal.ResidualScore == 2)
            {
                textBoxResidualPostal.Background = Brushes.Red;
                checkAddress = true;
            }
            else // Score == 0 or anything else
            {
                textBoxResidualPostal.Background = Brushes.Green;
            }
            addressEnableEdit(checkAddress);
            buttonAccept.IsEnabled = !checkAddress;
            buttonCheckAddress.IsEnabled = checkAddress;
        }

        private void populateNATISAddresses()
        {
            txtNatisPhysicalAddress.Text = mCaseInfo.NatisPhysical.Street + "\r\n"
                + mCaseInfo.NatisPhysical.Suburb + "\r\n"
                + mCaseInfo.NatisPhysical.Town + "\r\n"
                + mCaseInfo.NatisPhysical.POBox + "\r\n"
                + mCaseInfo.NatisPhysical.Code;

            txtNatisPostalAddress.Text = mCaseInfo.NatisPostal.Street + "\r\n"
                + mCaseInfo.NatisPostal.Suburb + "\r\n"
                + mCaseInfo.NatisPostal.Town + "\r\n"
                + mCaseInfo.NatisPostal.POBox + "\r\n"
                + mCaseInfo.NatisPostal.Code;

            lblPhysicalDate.Content = "NATIS Physical Date: " + "\r\n" + mCaseInfo.NatisPhysical.Date.ToString() + "\r\n" + "System Physical Date: " + "\r\n" + mCaseInfo.SystemPhysical.Date.ToString();
            CompareDates(mCaseInfo.NatisPhysical.Date, mCaseInfo.SystemPhysical.Date, lblPhysicalDate);
            lblPostalDate.Content = "NATIS Postal Date: " + "\r\n" + mCaseInfo.NatisPostal.Date.ToString() + "\r\n" + "System Postal Date: " + "\r\n" + mCaseInfo.SystemPostal.Date.ToString();
            CompareDates(mCaseInfo.NatisPostal.Date, mCaseInfo.SystemPostal.Date, lblPostalDate);
        }

        private void displayUpdate()
        {
            if (string.IsNullOrEmpty(mCaseInfo.TicketNo))
                lblTicketNumber.Content = mUserName;
            else
                lblTicketNumber.Content = mUserName + " verifying " + mCaseInfo.TicketNo;
            if (!string.IsNullOrEmpty(mCaseInfo.Image1))
                lblFileLocation.Content = System.IO.Path.GetDirectoryName(mCaseInfo.Image1);
            lblTickets.Content = mTicketCount.ToString();

            buttonEditPerson.IsEnabled = false;

            personEnableEdit(false);
            addressEnableEdit(false);

            textBoxRegNum.Text = mCaseInfo.VehicleRegNo;
            textBoxRegNum.Background = mCaseInfo.VehicleRegNoConfirmed ? Brushes.Green : Brushes.OrangeRed;

            textBoxVMake.Text = mCaseInfo.VehicleMake;
            textBoxVModel.Text = mCaseInfo.VehicleModel;
            textBoxVColor.Text = mCaseInfo.VehicleColour;
            textBoxVType.Text = mCaseInfo.VehicleType;

            bool foundType = false;
            comboBoxType.BorderBrush = Brushes.Black;
            //comboBoxType.BorderThickness =
            checkBoxType.IsChecked = false;
            checkBoxType.IsEnabled = false;
            comboBoxType.Items.Clear();
            if (mTypes != null && mTypes.Count > 0)
                for (int i = 0; i < mTypes.Count; i++)
                {
                    comboBoxType.Items.Add(mTypes[i]);
                    if (mCaseInfo.VehicleCaptureType == mTypes[i].Description)
                    {
                        comboBoxType.SelectedIndex = i;
                        foundType = true;
                    }
                }
            if ((!foundType) && (comboBoxType.Items.Count > 0))
            {
                comboBoxType.BorderBrush = Brushes.Red;
                //comboBoxType.BorderThickness = 4;
                checkBoxType.IsChecked = false;
                checkBoxType.IsEnabled = true;
            }

            textBoxPSurname.Text = mCaseInfo.PersonSurname;
            textBoxPName.Text = mCaseInfo.PersonName;
            textBoxPMiddleNames.Text = mCaseInfo.PersonMiddleNames;
            textBoxPTelephone.Text = mCaseInfo.PersonTelephone;
            textBoxPID.Text = mCaseInfo.PersonID;

            displayAddresses();

            populateNATISAddresses();

            //textBoxGismoPhyStreet.Text = mCaseInfo.mGismoPhysical.mStreet;
            //textBoxGismoPhySuburb.Text = mCaseInfo.mGismoPhysical.mSuburb;
            //textBoxGismoPhyTown.Text = mCaseInfo.mGismoPhysical.mTown;
            //textBoxGismoPhyPOBox.Text = mCaseInfo.mGismoPhysical.mPOBox;
            //textBoxGismoPhyCode.Text = mCaseInfo.mGismoPhysical.mCode;

            //textBoxGismoPosStreet.Text = mCaseInfo.mGismoPostal.mStreet;
            //textBoxGismoPosSuburb.Text = mCaseInfo.mGismoPostal.mSuburb;
            //textBoxGismoPosTown.Text = mCaseInfo.mGismoPostal.mTown;
            //textBoxGismoPosPOBox.Text = mCaseInfo.mGismoPostal.mPOBox;
            //textBoxGismoPosCode.Text = mCaseInfo.mGismoPostal.mCode;

            //radioButtonUseNatis.IsChecked = true;
            //radioButtonUseGismo.IsChecked = false;
            //radioButtonUseGismo.IsEnabled = !string.IsNullOrEmpty(textBoxGismoPosCode.Text) || !string.IsNullOrEmpty(textBoxGismoPhyCode.Text) ||
            //                                !string.IsNullOrEmpty(textBoxGismoPosStreet.Text) || !string.IsNullOrEmpty(textBoxGismoPhyStreet.Text) ||
            //                                !string.IsNullOrEmpty(textBoxGismoPosPOBox.Text) || !string.IsNullOrEmpty(textBoxGismoPhyPOBox.Text);

            buttonNotes.ToolTip = mCaseInfo.OffenceOldNotes;

            mVechileRegChanged = mPersonChanged = mAddresChanged = mTypeChanged = mNotesChanged = mImagesChanged = false;
            mPrintImageNo = 0;

            img1.Source = img2.Source = imgBG.Source = imgNP.Source = null;
            Icon1.Source = Icon2.Source = Icon3.Source = Icon4.Source = null;
            img1.resetTransformation();
            img2.resetTransformation();

            numberplateDrop();
            buttonsCheckEnable();

            comboBoxType.Focus();
        }

        private void personEnableEdit(bool doEnable)
        {
            textBoxPSurname.IsReadOnly =
            textBoxPName.IsReadOnly =
            textBoxPMiddleNames.IsReadOnly =
            textBoxPTelephone.IsReadOnly =
            textBoxPID.IsReadOnly = !doEnable;

            textBoxPSurname.IsTabStop =
            textBoxPName.IsTabStop =
            textBoxPMiddleNames.IsTabStop =
            textBoxPTelephone.IsTabStop =
            textBoxPID.IsTabStop = doEnable;

            textBoxPSurname.Background =
            textBoxPName.Background =
            textBoxPMiddleNames.Background =
            textBoxPTelephone.Background =
            textBoxPID.Background = (doEnable ? Brushes.White : Brushes.LightBlue);
        }


        private void addressEnableEdit(bool doEnable)
        {
            textBoxSystemPhyStreet.IsReadOnly =
            textBoxSystemPhySuburb.IsReadOnly =
            textBoxSystemPhyTown.IsReadOnly =
            textBoxSystemPhyPOBox.IsReadOnly =
            textBoxSystemPhyCode.IsReadOnly = !doEnable;

            textBoxSystemPhyStreet.IsTabStop =
            textBoxSystemPhySuburb.IsTabStop =
            textBoxSystemPhyTown.IsTabStop =
            textBoxSystemPhyPOBox.IsTabStop =
            textBoxSystemPhyCode.IsTabStop = doEnable;

            textBoxSystemPosStreet.IsReadOnly =
            textBoxSystemPosSuburb.IsReadOnly =
            textBoxSystemPosTown.IsReadOnly =
            textBoxSystemPosPOBox.IsReadOnly =
            textBoxSystemPosCode.IsReadOnly = !doEnable;

            textBoxSystemPosStreet.IsTabStop =
            textBoxSystemPosSuburb.IsTabStop =
            textBoxSystemPosTown.IsTabStop =
            textBoxSystemPosPOBox.IsTabStop =
            textBoxSystemPosCode.IsTabStop = doEnable;

            textBoxSystemPhyStreet.Background =
            textBoxSystemPhySuburb.Background =
            textBoxSystemPhyTown.Background =
            textBoxSystemPhyPOBox.Background =
            textBoxSystemPhyCode.Background =

            textBoxSystemPosStreet.Background =
            textBoxSystemPosSuburb.Background =
            textBoxSystemPosTown.Background =
            textBoxSystemPosPOBox.Background =
            textBoxSystemPosCode.Background = (doEnable ? Brushes.White : Brushes.LightBlue);

        }

        private void personUpdateValues()
        {
            mCaseInfo.PersonID = textBoxPID.Text.Trim();
            mCaseInfo.PersonName = textBoxPName.Text.Trim();
            mCaseInfo.PersonSurname = textBoxPSurname.Text.Trim();
            mCaseInfo.PersonMiddleNames = textBoxPMiddleNames.Text.Trim();
            mCaseInfo.PersonTelephone = textBoxPTelephone.Text.Trim();
        }

        private void addressUpdateValues()
        {
            mCaseInfo.SystemPhysical.Street = textBoxSystemPhyStreet.Text.Trim();
            mCaseInfo.SystemPhysical.Suburb = textBoxSystemPhySuburb.Text.Trim();
            mCaseInfo.SystemPhysical.Town = textBoxSystemPhyTown.Text.Trim();
            mCaseInfo.SystemPhysical.POBox = textBoxSystemPhyPOBox.Text.Trim();
            mCaseInfo.SystemPhysical.Code = textBoxSystemPhyCode.Text.Trim();
            mCaseInfo.SystemPhysical.Residual = textBoxResidualPostal.Text;

            mCaseInfo.SystemPostal.Street = textBoxSystemPosStreet.Text.Trim();
            mCaseInfo.SystemPostal.Suburb = textBoxSystemPosSuburb.Text.Trim();
            mCaseInfo.SystemPostal.Town = textBoxSystemPosTown.Text.Trim();
            mCaseInfo.SystemPostal.POBox = textBoxSystemPosPOBox.Text.Trim();
            mCaseInfo.SystemPostal.Code = textBoxSystemPosCode.Text.Trim();
            mCaseInfo.SystemPostal.Residual = textBoxResidualPostal.Text;
        }

        private void buttonReject_Click(object sender, RoutedEventArgs e)
        {
            IsNATISDataVisible(false);

            cReasons reas = new cReasons();
            reas.pReasons = mReasons;
            reas.pTicketNumber = mCaseInfo.TicketNo;
            reas.Owner = this;

            buttonAccept.IsEnabled = buttonReject.IsEnabled = false;

            numberplateFreeze();

            if (reas.ShowDialog() == true)
                doReject(reas.pReasonID);

            buttonsCheckEnable();
        }

        private void doReject(int reasonID)
        {
            IsNATISDataVisible(false);

            this.Cursor = Cursors.Wait;
            buttonAccept.IsEnabled = buttonReject.IsEnabled = false;

                CaptureTypeModel typ = (CaptureTypeModel)comboBoxType.SelectedItem;
                bool okay = true;

                if (mPersonChanged)
                    personUpdateValues();

                if (mAddresChanged)
                    addressUpdateValues();

            if (mCaseType == cFishpondPrompt.eLoadTypes.NewCases) // New cases
            {
                try
                {
                    var rejectedCase = verifyService.RejectCase(mCaseInfo, reasonID, mVechileRegChanged, mAddresChanged, mPersonChanged, (mTypeChanged ? typ.ID : -1), (mTypeChanged ? typ.Amount : 0), Environment.MachineName);

                    mCaseInfo = rejectedCase.Case;
                    mTypes = rejectedCase.CaptureTypes;
                    mTicketCount = rejectedCase.Count;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not Reject offence. \n" + ex.Message, "Reject error", MessageBoxButton.OK, MessageBoxImage.Error);
                        okay = false;
                }
            }

            else if (mCaseType == cFishpondPrompt.eLoadTypes.Fishpond)  // Fishpond cases
            {
                //if (!mDataAccess.fishpondCaseReject(mCaseInfo, reasonID, mVechileRegChanged, mAddresChanged, mPersonChanged, (mTypeChanged ? typ.ID : -1), (mTypeChanged ? typ.Amount : 0)))
                //{
                try
                {
                    verifyService.RejectFishpondCase(mCaseInfo, reasonID, mVechileRegChanged, mAddresChanged, mPersonChanged, (mTypeChanged ? typ.ID : -1), (mTypeChanged ? typ.Amount : 0), Environment.MachineName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not Reject offence. \n" + ex.Message, "Reject error", MessageBoxButton.OK, MessageBoxImage.Error);
                        okay = false;
                }

                bool doExit;

                okay = fishpondChooseCase(out mCaseInfo, out doExit);

                if (doExit)
                    {
                        this.Close();
                        return;
                    }
            }

            else if (mCaseType == cFishpondPrompt.eLoadTypes.Summons)  // Summons
                {
                    okay = true;
                }

            if (okay)
            {
                displayUpdate();

                if (string.IsNullOrEmpty(mCaseInfo.TicketNo) && mCaseType != cFishpondPrompt.eLoadTypes.Summons)
                    this.IsEnabled = false;
                    else
                    {
                        displayUpdate();
                        showImages();
                    }
            }

            buttonsCheckEnable();
            this.Cursor = null;

        }

        private bool isAddressFieldsEmpty()
        {
            if (textBoxSystemPhyCode.Text == string.Empty || textBoxSystemPosCode.Text == string.Empty)
            {
                return true;
            }

            if ((textBoxSystemPhyTown.Text == string.Empty) &&
                (textBoxSystemPhySuburb.Text == string.Empty) &&
                (textBoxSystemPhyStreet.Text == string.Empty && textBoxSystemPhyPOBox.Text == string.Empty))
            {
                return true;
            }

            if ((textBoxSystemPosStreet.Text == string.Empty && textBoxSystemPosPOBox.Text == string.Empty) &&
                (textBoxSystemPosSuburb.Text == string.Empty) &&
                (textBoxSystemPosTown.Text == string.Empty))
            {
                return true;
            }

            return false;
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            if (isAddressFieldsEmpty())
            {
                MessageBox.Show(this, "Please make sure address fields are filled in correctly", "Verify", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsNATISDataVisible(false);

            if (checkBoxType.IsEnabled && (checkBoxType.IsChecked == false))
            {
                MessageBox.Show(this, "Vehicle Type must be Verified", "Verify", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string prompt = "Are you sure you want to Accept '" + mCaseInfo.TicketNo + "'? \n";

            this.Cursor = Cursors.Wait;
            buttonAccept.IsEnabled = buttonReject.IsEnabled = false;

            numberplateFreeze();

            if (MessageBox.Show(this, prompt, "Verify", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                    bool okay = true;
                    bool doAccept = true;

                    if (mPersonChanged)
                        personUpdateValues();

                    if (mAddresChanged)
                        addressUpdateValues();

                    if (mImagesChanged)
                    {
                        /***** Not done anymore
                        string error;

                        if (!doRenames(out error))
                        {
                            doAccept = MessageBox.Show(this, "Could not change Print and Alternative images. \n" + error + "\n\nDo you want to continue to Accept this ticket?", "Verify error", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                            showImages();
                        }
                        *****/
                    }

                    if (doAccept)
                    {
                        int printImageID = -1;

                        switch (mPrintImageNo)
                        {
                            case 0: printImageID = mCaseInfo.Image1ID; break;
                            case 1: printImageID = mCaseInfo.Image2ID; break;
                            case 2: printImageID = mCaseInfo.Image3ID; break;
                            case 3: printImageID = mCaseInfo.Image4ID; break;
                        }

                    if (mCaseType == cFishpondPrompt.eLoadTypes.NewCases) // New cases
                    {
                        CaptureTypeModel typ = (CaptureTypeModel)comboBoxType.SelectedItem;

                        //if (!mDataAccess.caseAccept(ref mCaseInfo, mAddresChanged, mPersonChanged, (mTypeChanged ? typ.ID : -1), (mTypeChanged ? typ.Amount : 0), ref mTypes, out mTicketCount, printImageID))
                            try
                            {
                            var acceptedCase = verifyService.AcceptCase(mCaseInfo, mAddresChanged, mPersonChanged, (mTypeChanged ? typ.ID : -1), (mTypeChanged ? typ.Amount : 0), printImageID, Environment.MachineName);

                            mCaseInfo = acceptedCase.Case;
                            mTypes = acceptedCase.CaptureTypes;
                            mTicketCount = acceptedCase.Count;

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(this, "Could not save Verify. \n" + ex.Message, "Verify error", MessageBoxButton.OK, MessageBoxImage.Error);
                                okay = false;
                            }
                        }
                    else if (mCaseType == cFishpondPrompt.eLoadTypes.Fishpond)  // Fishpond
                    {

                        bool doExit;
                        CaptureTypeModel typ = (CaptureTypeModel)comboBoxType.SelectedItem;

                        //if (!mDataAccess.fishpondCaseAccept(mCaseInfo, mAddresChanged, mPersonChanged, (mTypeChanged ? typ.ID : -1), (mTypeChanged ? typ.Amount : 0), printImageID))
                        try
                        {
                            var acceptedFishpondCase = verifyService.AcceptCase(mCaseInfo, mAddresChanged, mPersonChanged, (mTypeChanged ? typ.ID : -1), (mTypeChanged ? typ.Amount : 0), printImageID, Environment.MachineName);

                        }
                        catch (Exception ex)
                        {
                                MessageBox.Show(this, "Could not save Verify. \n" + ex.Message, "Verify error", MessageBoxButton.OK, MessageBoxImage.Error);
                                okay = false;
                        }
                            
                                okay = fishpondChooseCase(out mCaseInfo, out doExit);

                                if (doExit)
                                {
                                    //mDataAccess.disconnect();
                                    this.Close();
                                    return;
                                }
                            
                        }
                    else if (mCaseType == cFishpondPrompt.eLoadTypes.Summons)  // Summons
                    {
                        //if (!mDataAccess.summonsAccept(ref mCaseInfo, mAddresChanged, mPersonChanged))
                        try
                        {
                            var acceptedSummonsCase = verifyService.AcceptSummons(mCaseInfo, mAddresChanged, mPersonChanged, Environment.MachineName);
                            mCaseInfo = acceptedSummonsCase;

                        }
                        catch (Exception ex)
                        {
                                MessageBox.Show(this, "Could not save Summons verify. \n" + ex.Message, "Verify error", MessageBoxButton.OK, MessageBoxImage.Error);
                                okay = false;
                        }
                        }

                        if (okay)
                        {
                            MessageBox.Show(this, "Changes saved into database.", "Summons Save", MessageBoxButton.OK, MessageBoxImage.Information);
                            displayUpdate();
                            if (string.IsNullOrEmpty(mCaseInfo.TicketNo) && mCaseType != cFishpondPrompt.eLoadTypes.Summons)
                                this.IsEnabled = false;
                            else
                            {
                                displayUpdate();
                                showImages();
                            }
                        }
                    }
            }

            buttonsCheckEnable();
            this.Cursor = null;
        }

        /***** Not done anymore
        private bool doRenames(out string error)
        {
            error = string.Empty;

            try
            {
                string path = System.IO.Path.GetDirectoryName(mCaseInfo.mImage1);
                string i1 = System.IO.Path.GetFileName(img1.Source.ToString());
                string i2 = System.IO.Path.GetFileName(img2.Source.ToString());

                if (i1.Equals(i2))
                {
                    error = "The Print image and Alternative image are the same.";
                    return false;
                }

                if (!path.EndsWith(@"\"))
                    path += @"\";

                i1 = path + i1;
                i2 = path + i2;

                if (i1.Equals(mCaseInfo.mImage2) && i2.Equals(mCaseInfo.mImage1)) // Swap the first 2 images with each other
                {
                    if (!i1.Equals(mCaseInfo.mImage1))
                        if (!filesSwap(i1, mCaseInfo.mImage1, out error))
                            return false;
                }
                else
                {
                    bool doneFirst = false;
                    bool doneSecond = false;

                    // First do the firs 2 images then the 3rd and 4th images else the swap may swap out the files
                    if (i1.Equals(mCaseInfo.mImage2))
                    {
                        if (!filesSwap(mCaseInfo.mImage1, mCaseInfo.mImage2, out error))
                            return false;
                        doneFirst = true;
                    }
                    else if (i2.Equals(mCaseInfo.mImage1))
                    {
                        if (!filesSwap(mCaseInfo.mImage2, mCaseInfo.mImage1, out error))
                            return false;
                        doneSecond = true;
                    }

                    if (!doneFirst)
                    {
                        if (!i1.Equals(mCaseInfo.mImage1))
                            if (!filesSwap(i1, mCaseInfo.mImage1, out error))
                                return false;
                    }

                    if (!doneSecond)
                    {
                        if (!i2.Equals(mCaseInfo.mImage2))
                            if (!filesSwap(i2, mCaseInfo.mImage2, out error))
                                return false;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
      
            return true;
        }

        private bool filesSwap(string fil1, string fil2, out string error)
        {
            error = string.Empty;

            try
            {
                File.Move(fil1, fil1 + ".tmp");
                File.Move(fil2, fil1);
                File.Move(fil1 + ".tmp", fil2);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            return true;
        }
        *****/

        private void buttonsCheckEnable()
        {
           // buttonAccept.IsEnabled = !buttonCheckAddress.IsEnabled;
            buttonReject.IsEnabled = 
            buttonNotes.IsEnabled = 
            buttonEditRegNo.IsEnabled =
            buttonEditAddress.IsEnabled =
            buttonEditPerson.IsEnabled = !string.IsNullOrEmpty(textBoxRegNum.Text);

            if (mCaseType == cFishpondPrompt.eLoadTypes.Summons)
            {
                buttonEditAddress.IsEnabled = true;
                buttonReject.Visibility = System.Windows.Visibility.Hidden;
            }

            //buttonEditAddress.IsEnabled = textBoxNatisPhyStreet.IsReadOnly;
            //buttonEditPerson.IsEnabled = textBoxPName.IsReadOnly;
        }

        private void buttonNotes_Click(object sender, RoutedEventArgs e)
        {
            IsNATISDataVisible(false);

            buttonNotes.IsEnabled = false;

            numberplateFreeze();

            var notes = new cNotes();
            notes.Owner = this;
            notes.pTicketNumber = mCaseInfo.TicketNo;
            notes.pPreviousNotes = mCaseInfo.OffenceOldNotes;
            notes.pNewNotes = mCaseInfo.OffenceNewNotes;
            if (notes.ShowDialog() == true)
            {
                mNotesChanged = true;
                mCaseInfo.OffenceNewNotes = notes.pNewNotes;
                buttonNotes.ToolTip = mCaseInfo.OffenceOldNotes + " " + mCaseInfo.OffenceNewNotes;
            }

            buttonNotes.IsEnabled = true;
        }

        private void buttonEditRegNo_Click(object sender, RoutedEventArgs e)
        {
            IsNATISDataVisible(false);

            buttonEditRegNo.IsEnabled = false;

            //numberplateFreeze();

            var regno = new cRegNumber();
            regno.Owner = this;

            if (regno.ShowDialog() == true)
            {
                mVechileRegChanged = true;
                mCaseInfo.VehicleRegNo = regno.pRegNumber;
                doReject(-1);
            }

            buttonEditRegNo.IsEnabled = true;
        }

        private void buttonEditPerson_Click(object sender, RoutedEventArgs e)
        {
            IsNATISDataVisible(false);

            buttonEditPerson.IsEnabled = false;

            numberplateFreeze();

            personEnableEdit(true);

            textBoxPSurname.Focus();
        }

        private void buttonEditAddress_Click(object sender, RoutedEventArgs e)
        {
            buttonEditAddress.IsEnabled = false;
            mCaseInfo.UseGismoAddress = false;
            //radioButtonUseGismo.IsChecked = false;
            //radioButtonUseNatis.IsChecked = true;

            numberplateFreeze();

            IsNATISDataVisible(true);

            addressEnableEdit(true);

            textBoxSystemPhyStreet.Focus();
        }

        private void comboBoxType_GotFocus(object sender, RoutedEventArgs e)
        {
            if (mIgnoreEvents)
                return;

            numberplateFreeze();
        }

        //private void radioButtonUseNatis_Click(object sender, RoutedEventArgs e)
        //{
        //    if (mIgnoreEvents)
        //        return;

        //    //radioButtonUseGismo.IsChecked = false;
        //    buttonEditAddress.IsEnabled = true;
        //    mCaseInfo.mUseGismoAddress = false;

        //    numberplateFreeze();

        //    buttonsCheckEnable();
        //}

        //private void radioButtonUseGismo_Click(object sender, RoutedEventArgs e)
        //{
        //    if (mIgnoreEvents)
        //        return;

        //    //radioButtonUseNatis.IsChecked = false;
        //    buttonEditAddress.IsEnabled = true;
        //    mCaseInfo.mUseGismoAddress = true;

        //    numberplateFreeze();

        //    addressEnableEdit(false);

        //    buttonsCheckEnable();
        //}

        private void comboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mIgnoreEvents)
                return;
            mTypeChanged = true;
        }

        private void textBoxPerson_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mIgnoreEvents)
                return;
            mPersonChanged = true;
        }

        private void textBoxSystemAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mIgnoreEvents)
                return;
            buttonCheckAddress.IsEnabled = true;
            mAddresChanged = true;
            //radioButtonUseNatis.IsChecked = true;
            //radioButtonUseGismo.IsChecked = false;
            mCaseInfo.UseGismoAddress = false;
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            numberplateFreeze();
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
            img2.clipImage(recMask.Margin.Left, recMask.Margin.Top, recMask.Width, recMask.Height);

            imgNP.Source = img2.cropImageOnClip();

            recMask.Visibility = Visibility.Visible;
            bdrNP.Visibility = Visibility.Visible;

            buttonsCheckEnable();
        }

        private void numberplateDrop()
        {
            recMask.Visibility = Visibility.Hidden;
            bdrNP.Visibility = Visibility.Hidden;
            img2.Clip = null;

            buttonsCheckEnable();
        }

        /// <summary>
/// TODO: Ontwikkel dalk beter manier om NPImages te hanteer**************************************************************************************************************
        /// </summary>
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

        private void numberplateFreeze()
        {
            if (recMask.Visibility == Visibility.Visible)
            {

                if (string.IsNullOrEmpty(mCaseInfo.ImageNP))
                    mCaseInfo.ImageNP = ConstructNPName(mCaseInfo.Image1);

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

        delegate void delShowImages();
        private void showImages()
        {
            imgBG.Source = null;

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

        private void buttonCheckAddress_Click(object sender, RoutedEventArgs e)
        {
            IsNATISDataVisible(false);

            this.Cursor = Cursors.Wait;
            buttonCheckAddress.IsEnabled = false;
            buttonAccept.IsEnabled = buttonReject.IsEnabled = false;

                addressUpdateValues();
                buttonAccept.IsEnabled = buttonReject.IsEnabled = false;

            try
            {
                var checkedAddressInfo = verifyService.CheckAddress(mCaseInfo, mCaseType == cFishpondPrompt.eLoadTypes.Summons);
                mCaseInfo = checkedAddressInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not check addresses. \n" + ex.Message, "Verify check error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
                displayAddresses();
                addressEnableEdit(true);
                mAddresChanged = true;

                //addressUpdateValues();
            

            buttonsCheckEnable();
            buttonCheckAddress.IsEnabled = true;
            buttonAccept.IsEnabled = buttonReject.IsEnabled = true;
            this.Cursor = null;
        }

        private void IsNATISDataVisible(bool lbIsVisible)
        {
            if (lbIsVisible)
            {
                gpbNatisAddresses.Visibility = Visibility.Visible;
                buttonShowNatisAddress.Content = "Hide NATIS Addr";
            }
            else
            {
                gpbNatisAddresses.Visibility = Visibility.Hidden;
                buttonShowNatisAddress.Content = "Show NATIS Addr";
            }
        }

        private void buttonShowNatisAddress_Click(object sender, RoutedEventArgs e)
        {
            if (buttonShowNatisAddress.Content == "Hide NATIS Addr")
            {
                IsNATISDataVisible(false);
            }
            else
            {
                IsNATISDataVisible(true);
            }
        }

        private void CompareDates(DateTime dNatisDate, DateTime dSystemDate, Label lbl)
        {
            if (dNatisDate >= dSystemDate)
            {
                lbl.Background = Brushes.Green;
            }
            else
            {
                lbl.Background = Brushes.Red;
            }
        }

        private void searchCodes(bool isPhysical)
        {
            cPostalCodes diag = new cPostalCodes();

            diag.initialise(verifyService, isPhysical);
            diag.Owner = this;

            if (diag.ShowDialog() == true)
            {
                PostalCodeModel cde = diag.pSelectedCode;

                if (isPhysical)
                {
                    textBoxSystemPhyTown.Text = cde.City;
                    textBoxSystemPhySuburb.Text = cde.Suburb;
                    textBoxSystemPhyCode.Text = cde.Code;
                }
                else
                {
                    textBoxSystemPosTown.Text = cde.City;
                    textBoxSystemPosSuburb.Text = cde.Suburb;
                    textBoxSystemPosCode.Text = cde.Code;
                }
            }
        }

        private void btnPhysical_Click(object sender, RoutedEventArgs e)
        {
            searchCodes(true);
        }

        private void btnPostal_Click(object sender, RoutedEventArgs e)
        {
            searchCodes(false);
        }

        private void buttonShowVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Failed to load window from{0} - {1}", "OtherWindow", ex.Message));
                throw new Exception(String.Format("Failed to load window from{0} - {1}", "OtherWindow", ex.Message), ex);

            }

        }
     }
}
