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
    /// Interaction logic for RegNumber.xaml
    /// </summary>
    public partial class cRegNumber : MetroWindow
    {
        public string pRegNumber
        {
            get { return txtRegis2.Text.Trim(); }
        }

        public cRegNumber()
        {
            InitializeComponent();

            btnCancel.IsCancel = true;
            btnOK.IsDefault = true;

            txtRegis2.IsEnabled = false;
            txtRegis1.Focus();
            txtRegis1.SelectAll();
        }

        private void txtBox_Regis1Changed(object sender, RoutedEventArgs e)
        {
            checkOK();
        }

        private void txtBox_Regis2Changed(object sender, TextChangedEventArgs e)
        {
            checkOK();
        }

        private void checkOK()
        {
            string r1 = txtRegis1.Password.Trim();
            string r2 = txtRegis2.Text.Trim();

            txtRegis2.IsEnabled = r1.Length > 2;

            btnOK.IsEnabled = (r2.Length > 0 && r1.Length > 0 && r1.Equals(r2));
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            //this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            //this.Close();
        }
    }
}
