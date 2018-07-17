using Kapsch.ITS.App.Common;
using Kapsch.ITS.App.Common.Models;
using System;
using System.Windows;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;


namespace iPortal
{
    public class Startup : IShellApplication
    {
        private string ITSPortalEndpoint = ConfigurationManager.AppSettings["ITSPortalEndpoint"];

        public bool HasAccess(AuthenticatedUser authenticatedUser)
        {
            return true;
        }

        public void Show(AuthenticatedUser authenticatedUser)
        {
            Process.Start(ITSPortalEndpoint);
        }

        public string MenuLabel
        {
            get { return "IMS Portal " + Version; }
        }

        public System.Drawing.Bitmap MenuImage
        {
            get
            {
                //return null;
                Bitmap bitmap = new Bitmap(Application.GetResourceStream(new Uri("pack://application:,,,/iPortal;component/Images/app.png")).Stream);

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
