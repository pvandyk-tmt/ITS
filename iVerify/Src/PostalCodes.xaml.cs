using Kapsch.ITS.Gateway.Clients;
using Kapsch.ITS.Gateway.Models.Verify;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TMT.iVerify
{
    /// <summary>
    /// Interaction logic for PostalCodes.xaml
    /// </summary>
    public partial class cPostalCodes : MetroWindow
    {
        private VerifyService mVerifyService = null;
        private bool mIsPhysical = true;

        public PostalCodeModel pSelectedCode { get; set; }
        public IList<PostalCodeModel> pCodes { get; set; }

        public cPostalCodes()
        {
            InitializeComponent();

            pCodes = new List<PostalCodeModel>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCity.Focus();
        }

        public void initialise(VerifyService verifyService, bool isPhysical)
        {
            mVerifyService = verifyService;
            mIsPhysical = isPhysical;

            this.Title = "Postal Codes - " + (mIsPhysical ? "Physical Address" : "Postal Address");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (listCodes.SelectedIndex >= 0)
            {
                pSelectedCode = (PostalCodeModel)listCodes.SelectedItem;

                DialogResult = true;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void checkSearch()
        {
            btnSearch.IsEnabled = ((txtCity.Text.Trim().Length > 2) ||
                       (txtSuburb.Text.Trim().Length > 2) ||
                       (txtCode.Text.Trim().Length > 2));
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            listCodes.BeginInit();
            listCodes.ItemsSource = null;
            listCodes.Items.Clear();

            //if (mDataAccess.getPostalCodes(txtCity.Text.Trim().ToUpper(), txtSuburb.Text.Trim().ToUpper(), txtCode.Text.Trim().ToUpper(), mIsPhysical, pCodes))
            try
            {
                var postalCodes = mVerifyService.GetPostalCode(txtCity.Text.Trim().ToUpper(), txtSuburb.Text.Trim().ToUpper(), txtCode.Text.Trim().ToUpper(), mIsPhysical);

                pCodes = postalCodes;
                listCodes.ItemsSource = postalCodes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not get postal codes \n\n" + ex.Message, "Postal Codes", MessageBoxButton.OK, MessageBoxImage.Information);
                listCodes.EndInit();
            }
        }

        private void txtCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkSearch();
        }

        private void txtSuburb_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkSearch();
        }

        private void txtCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkSearch();
        }

        private void listCodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOK.IsEnabled = (listCodes.SelectedIndex >= 0);
        }

        private void listCodes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (btnOK.IsEnabled)
                btnOK_Click(null, null);
        }

    }
}
