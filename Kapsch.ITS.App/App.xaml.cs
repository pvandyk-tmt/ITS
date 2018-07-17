using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Kapsch.ITS.App.Common;
using Kapsch.ITS.App.Common.Models;

namespace Kapsch.ITS.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IList<IShellApplication> Applications;
        private static string CultureName = ConfigurationManager.AppSettings["CultureUI"];
        private static string DateSeparator = ConfigurationManager.AppSettings["DateSeparator"];
        private static string LongDatePattern = ConfigurationManager.AppSettings["LongDatePattern"];
        private static string ShortDatePattern = ConfigurationManager.AppSettings["ShortDatePattern"];
        private static string LongTimePattern = ConfigurationManager.AppSettings["LongTimePattern"];
        private static string ShortTimePattern = ConfigurationManager.AppSettings["ShortTimePattern"];

        protected override void OnStartup(StartupEventArgs e)
        {           
            InitialiseCulture();
            InitialiseTheme();
          
            RegisterServicesAndApplications();

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            base.OnStartup(e);          
        }

        private void InitialiseTheme()
        {
            var appTheme = ThemeManager.GetAppTheme("BaseLight");
            var accent = ThemeManager.GetAccent("Steel");
            ThemeManager.ChangeAppStyle(Current, accent, appTheme);
        }

        private void InitialiseCulture()
        {
            var cultureInfo = new CultureInfo(CultureName);

            cultureInfo.DateTimeFormat.DateSeparator = DateSeparator;
            cultureInfo.DateTimeFormat.LongDatePattern = LongDatePattern;
            cultureInfo.DateTimeFormat.ShortDatePattern = ShortDatePattern;
            cultureInfo.DateTimeFormat.LongTimePattern = LongTimePattern;
            cultureInfo.DateTimeFormat.ShortTimePattern = ShortTimePattern;

            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
        }

        private void RegisterServicesAndApplications()
        {
            Applications = new List<IShellApplication>();
            var assemblies = LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetExportedTypes();

                foreach (var type in types)
                {
                    var appInterface = type.GetInterface(typeof(IShellApplication).ToString());
                    if (appInterface != null)
                    {
                        object obj = Activator.CreateInstance(type);
                        if (obj != null && obj is IShellApplication)
                        {
                            var shellApplication = (IShellApplication)obj;

                            Applications.Add(shellApplication);
                        }
                    }
                    else
                    {
                        var userSecurityServiceInterface = type.GetInterface(typeof(IUserSecurityService).ToString());
                        if (userSecurityServiceInterface != null)
                        {
                            ServiceFactory<IUserSecurityService>.CreateService = () => (IUserSecurityService)Activator.CreateInstance(type);
                            ServiceFactory<AuthenticatedUser>.CreateService = () => ApplicationSession.AuthenticatedUser;
                        }
                    }                   
                }
            }
        }

        private IEnumerable<Assembly> LoadAssemblies(string path)
        {
            var assemblies = new List<Assembly>();

            var allowedExtensions = new[] { "dll", "exe" };
            var files = Directory
                .GetFiles(path)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .ToList();

            foreach (FileInfo file in files.Select(f => new FileInfo(f)))
            {
                try
                {
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(file.FullName);
                    Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName);

                    assemblies.Add(assembly);
                }
                catch
                {
                    // Empty on purpose
                }
            }

            return assemblies;
        }
    }
}
