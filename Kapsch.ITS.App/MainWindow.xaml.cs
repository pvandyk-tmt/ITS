using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Kapsch.ITS.App.Common;
using Kapsch.ITS.App.Common.Models;
using Kapsch.ITS.App.Controls;
using Kapsch.ITS.App.Utils;
using Kapsch.ITS.Gateway.Models;

namespace Kapsch.ITS.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Color[] Colors = 
            new[] 
            {
                Color.FromRgb(144, 136, 129),
                Color.FromRgb(222, 8, 0),
                Color.FromRgb(231, 88, 0),
                Color.FromRgb(25, 75, 23), 
                Color.FromRgb(255, 203, 0), 
                Color.FromRgb(6, 5, 6), 
                Color.FromRgb(0x1E, 0x90, 0xFF), 
                Color.FromRgb(0x1E, 0x90, 0xFF) 
            };
        public IList<IShellApplication> _applications;

        public MainWindow()
        {
            InitializeComponent();

            var ipAddress = GetIPAddress();
            labelHeading.Content = string.Format("Machine: {0} ({1})", Environment.MachineName, ipAddress == null ? "No IP Address" : ipAddress.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginDialog loginDialog = new LoginDialog();
            if (loginDialog.ShowDialog() != true)
            {
                App.Current.Shutdown();
                return;
            }

            labelFirstName.Content = AuthenticatedUser.UserData.FirstName;
            labelSurname.Content = AuthenticatedUser.UserData.LastName;

            var timer = new DispatcherTimer { Interval = AuthenticatedUser.SessionData.ExpiryTimestamp.Subtract(DateTime.UtcNow) };
            timer.Tick += delegate
            {
                timer.Stop();

                Close();
            };
            timer.Start();

            CreateTiles(LoadApplications());
        }

        private IList<IShellApplication> LoadApplications()
        {
            var applications = new List<IShellApplication>();

            foreach (var application in App.Applications)
            {
                if (application.HasAccess(AuthenticatedUser))
                {
                    applications.Add(application);
                }               
            }

            return applications;
        }        

        private void CreateTiles(IList<IShellApplication> applications)
        {
            tilePanel.Children.Clear();
            AppBlock appBlock = null;
            var index = 0;

            foreach (var application in applications)
            {
                var tile = new Tile();
                tile.Title = application.MenuLabel;
                tile.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                tile.Background = new SolidColorBrush(Colors[index % 5]);
                tile.Tag = application;
                tile.Click += tile_Click;
                if (application.MenuImage != null)
                {
                    tile.Content = new Image { Source = Imaging.CreateBitmapSourceFromBitmap(application.MenuImage), Height = 80, Stretch = Stretch.Uniform };
                }

                if (appBlock == null)
                {
                    appBlock = new AppBlock();
                }

                appBlock.AppearanceType = (AppearanceType)(tilePanel.Children.Count % 5);
                appBlock.Add(tile);
                if (!appBlock.CanAdd())
                {
                    tilePanel.Children.Add(appBlock);
                    appBlock = null;
                }

                index++;
            }

            if (appBlock != null)
                tilePanel.Children.Add(appBlock);
        }

        private new void Close()
        {
            try
            {
                var userService = ServiceFactory<IUserSecurityService>.GetService();
                userService.SignOut(AuthenticatedUser.SessionToken);
            }
            catch
            {
                // Empty on purpose
            }

            App.Current.Shutdown();
        }

        private void tile_Click(object sender, RoutedEventArgs e)
        {
            var tile = (Tile)sender;
            var application = (IShellApplication)tile.Tag;

            application.Show(AuthenticatedUser);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private AuthenticatedUser AuthenticatedUser
        {
            get { return ApplicationSession.AuthenticatedUser; }
        }

        public static IPAddress GetIPAddress()
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses("");

            foreach (IPAddress hostAddress in hostAddresses)
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(hostAddress) &&  
                    !hostAddress.ToString().StartsWith("169.254."))  
                    return hostAddress;
            }
            return null;
        }
    }
}
