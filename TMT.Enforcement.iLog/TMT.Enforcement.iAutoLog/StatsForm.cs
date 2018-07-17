using System;
using System.IO;
using System.Windows.Forms;
using TMT.Core.Camera.Base;

namespace TMT.Enforcement.iAutoLog
{
    public partial class StatsForm : Form
    {
        private cFilm _film;

        public StatsForm()
        {
            InitializeComponent();
        }

        public void BindFormData(cFilm film)
        {
            _film = film;

            if (_film.pHasEncryptedPictureFiles)
            {
                _film.applyStats();

                clearDisplay();

                if (!_film.pHasErrors)
                {
                    cPictureFile pic = film.getFirstValidPictureFile();

                    lblCamDate.Text = pic.pOffenceDateStringDD_MM_YYYY;
                    lblAveSpeed.Text = film.pAverageSpeed.ToString("0.0") + " km/h";
                    lblCaptureErrors.Text = film.pCaptureErrors.ToString();
                    if (film.pEndDate.HasValue)
                    {
                        lblEndDate.Text = film.pEndDate.Value.ToString("dd MMM yyyy HH:mm:ss");
                    }
                    else
                    {
                        lblEndDate.Text = "";
                    }

                    lblHighestSpeed.Text = film.pHighestSpeed + " km/h";
                    lblHighSpeed.Text = film.pHighSpeedCount.ToString();
                    lblInfringements.Text = film.pInfringements.ToString();
                    lblLocationCode.Text = pic.pLocationCode;
                    lblLowSpeed.Text = film.pLowSpeedCount.ToString();
                    lblMeasureErrors.Text = film.pMeasurementErrors.ToString();
                    lblTestPhotos.Text = film.pTestPhotos.ToString();
                    lblSessionName.Text = pic.pSession;
                    if (film.pStartDate.HasValue)
                    {
                        lblStartDate.Text = film.pStartDate.Value.ToString("dd MMM yyyy HH:mm:ss");
                    }
                    else
                    {
                        lblStartDate.Text = "";
                    }
                    lblVehiclesChecked.Text = film.pVehiclesChecked.ToString();
                    lblJammers.Text = film.pJammerCount.ToString();   
                }
            }
        }

        private void clearDisplay()
        {
            lblAveSpeed.Text = "";
            lblCaptureErrors.Text = "";
            lblEndDate.Text = "";
            lblHighestSpeed.Text = "";
            lblHighSpeed.Text = "";
            lblInfringements.Text = "";
            lblLocationCode.Text = "";
            lblLowSpeed.Text = "";
            lblMeasureErrors.Text = "";
            lblSessionName.Text = "";
            lblStartDate.Text = "";
            lblVehiclesChecked.Text = "";
            lblTestPhotos.Text = "";
            lblJammers.Text = "";
            lblCamDate.Text = "";
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            const string delimiter = ",";

            if (txtDirectory.Text.Length == 0)
            {
                MessageBox.Show("Cannot Export. Please select a directory to export to!", "Cannot Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }

            if (!_film.pHasErrors)
            {
                cPictureFile pic = _film.getFirstValidPictureFile();

                using (StreamWriter sw = File.CreateText(Path.Combine(txtDirectory.Text, pic.pSession + ".txt")))
                {
                    TimeSpan duration = new TimeSpan();

                    if (_film.pStartDate.HasValue && _film.pEndDate.HasValue)
                    {
                        duration = _film.pEndDate.Value - _film.pStartDate.Value;

                        sw.WriteLine(_film.pStartDate.Value.ToString("ddMMyyyy") + delimiter +
                                     pic.pSession + delimiter +
                                     pic.pLocationCode + delimiter +
                                     _film.pInfringements + delimiter +
                                     "0" + delimiter +
                                     "0" + delimiter +
                                     "1" + delimiter +
                                     DateTime.Today + delimiter +
                                     _film.pStartDate.Value + delimiter +
                                     _film.pEndDate.Value + delimiter +
                                     duration.TotalMinutes + delimiter +
                                     _film.pVehiclesChecked + delimiter +
                                     _film.pInfringements + delimiter +
                                     _film.pHighestSpeed + delimiter +
                                     "0" + delimiter +
                                     _film.pAverageSpeed
                            );
                        sw.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cannot Export. Stats file does not have start and end date!", "Cannot Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                MessageBox.Show("Done", "Exported", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Cannot Export Stats file - Film Error!", "Cannot Export", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtDirectory.Text = dlg.SelectedPath;
            }
        }
    }
}