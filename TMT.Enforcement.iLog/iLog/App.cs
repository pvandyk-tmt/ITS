using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Kapsch.ITS.App.Common;
using Kapsch.ITS.App.Common.Models;
using System.Reflection;

namespace TMT.Enforcement.iLog
{
    public class App : IShellApplication
    {
        public static AuthenticatedUser AuthenticatedUser;

        public bool HasAccess(Kapsch.ITS.App.Common.Models.AuthenticatedUser authenticatedUser)
        {
            AuthenticatedUser = authenticatedUser;
            return authenticatedUser.IsInRole("IMS Work Station: iApps - iLog");
            //return true;
        }

        public Bitmap MenuImage
        {
            get 
            {
                Bitmap bitmap = (Bitmap)Properties.Resources.iLog;
                return bitmap; 
            }
        }

        public string MenuLabel
        {
            get { return "iLog " + Version; }
        }

        public void Show(AuthenticatedUser authenticatedUser)
        {
            Application.Run(new MainForm());
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
