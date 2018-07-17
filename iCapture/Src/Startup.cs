using Kapsch.ITS.App;
using Kapsch.ITS.App.Common;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows;

namespace TMT.iCapture
{
    public class Startup : IShellApplication
    {
        public static Kapsch.ITS.App.Common.Models.AuthenticatedUser AuthenticatedUser;

        public bool HasAccess(Kapsch.ITS.App.Common.Models.AuthenticatedUser authenticatedUser)
        {
            AuthenticatedUser = authenticatedUser;
            return authenticatedUser.IsInRole("IMS Work Station: iApps - iCapture");
        }

        public void Show(Kapsch.ITS.App.Common.Models.AuthenticatedUser authenticatedUser)
        {
            AuthenticatedUser = authenticatedUser;
            try
            {
                var mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                //go back to main screen
            }
        }

        public string MenuLabel
        {
            get { return "iCapture " + Version; }
        }

        public System.Drawing.Bitmap MenuImage
        {
            get
            {
                Bitmap bitmap = new Bitmap(Application.GetResourceStream(new Uri("pack://application:,,,/iCapture;component/Images/app.png")).Stream);

                return bitmap;
            }
        }

        private string Version
        {
            get
            {
                return Assembly.GetCallingAssembly().GetName().Version.ToString();
            }
        }
    }
}
