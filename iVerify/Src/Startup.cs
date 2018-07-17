    using Kapsch.ITS.App;
    using Kapsch.ITS.App.Common;
    using System.Reflection;

namespace TMT.iVerify
{
        public class Startup : IShellApplication
        {
            public bool HasAccess(Kapsch.ITS.App.Common.Models.AuthenticatedUser authenticatedUser)
            {
                return true;
            }

            public void Show(Kapsch.ITS.App.Common.Models.AuthenticatedUser authenticatedUser)
            {
                ApplicationSession.AuthenticatedUser = authenticatedUser;
                var mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }

            public string MenuLabel
            {
                get { return "iVerify " + Version; }
            }

            public System.Drawing.Bitmap MenuImage
            {
                get { return null; }
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