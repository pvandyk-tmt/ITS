using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Kapsch.ITS.App.Common;
using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Authenticate;
using Kapsch.Gateway.Models.Shared;
using Kapsch.ITS.App.Common.Models;

namespace Kapsch.ITS.App
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog
    {
        public LoginDialog()
        {
            InitializeComponent();

            //textBoxUsername.Text = "NVAAS";
            //passwordBoxPassword.Password = "Zandre@09";
            textBoxUsername.Focus();
        }

        private void buttonSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (Login(Username, Password))
                DialogResult = true;
        }

        private bool Login(string userName, string password)
        {
            
            try
            {
                var userSecurityService = ServiceFactory<IUserSecurityService>.GetService();
                ApplicationSession.AuthenticatedUser = userSecurityService.SignIn(userName, password);

                return true;
            }
            catch (GatewayException ge)
            {
                textBoxErrorMessage.Text = ge.Message;

                return false;
            }
            catch (Exception ex)
            {
                textBoxErrorMessage.Text = ex.Message;

                return false;
            }
        }

        private void textBoxUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Username = textBoxUsername.Text;

            ValidateUI();
        }

        private void passwordBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = passwordBoxPassword.Password;

            ValidateUI();
        }

        private void ValidateUI()
        {
            buttonSignIn.IsEnabled = (!string.IsNullOrEmpty(textBoxUsername.Text) && !string.IsNullOrEmpty(passwordBoxPassword.Password));
        }

        
        private string Username { get; set; }
        private string Password { get; set; }
    }
}
