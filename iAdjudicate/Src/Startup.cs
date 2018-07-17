using Kapsch.ITS.App;
using Kapsch.ITS.App.Common;
using Kapsch.ITS.App.Common.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TMT.iAdjudicate
{
    public class Startup : IShellApplication
    {
        public static Kapsch.ITS.App.Common.Models.AuthenticatedUser AuthenticatedUser;

        public bool HasAccess(Kapsch.ITS.App.Common.Models.AuthenticatedUser authenticatedUser)
        {
            AuthenticatedUser = authenticatedUser;
            return authenticatedUser.IsInRole("IMS Work Station: iApps - iAdjudicate");
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
            //go back to main window
            }

            }

        public string MenuLabel
        {
            get { return "iAdjudicate " + Version; }
        }

        public System.Drawing.Bitmap MenuImage
        {
            get
            {
                Bitmap bitmap = new Bitmap(Application.GetResourceStream(new Uri("pack://application:,,,/iAdjudicate;component/Images/app.png")).Stream);
                
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