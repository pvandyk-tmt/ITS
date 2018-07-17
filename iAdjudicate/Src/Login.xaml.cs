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

namespace TMT.iAdjudicate
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class cLogin : MetroWindow
    {
        public string pUsername
        {
            set { txtUsername.Text = value; }
            get { return txtUsername.Text.Trim(); }
        }

        public string pPassword
        {
            get { return txtPassword.Password.Trim(); }
        }

        public cLogin()
        {
            InitializeComponent();

            btnCancel.IsCancel = true;
            btnOK.IsDefault = true;

            txtUsername.Focus();
            txtUsername.SelectAll();
        }

        private void txtBox_UsernameChanged(object sender, TextChangedEventArgs e)
        {
            checkOK();
        }

        private void txtBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            checkOK();
        }

        private void checkOK()
        {
            btnOK.IsEnabled = (txtUsername.Text.Trim().Length > 0 && txtPassword.Password.Trim().Length > 0);
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
