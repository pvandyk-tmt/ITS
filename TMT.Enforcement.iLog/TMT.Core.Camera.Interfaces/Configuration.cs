#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace TMT.Core.Camera.Interfaces
{
    public class Configuration
    {
        public string DefaultPath()
        {
            string exe = Process.GetCurrentProcess().MainModule.FileName;
            string path = Path.GetDirectoryName(exe);

            return path;

            //if (Environment.Is64BitOperatingSystem)
            //{
            //    return @"C:\Program Files (x86)\TMT\iLog";
            //}
            //else
            //{
            //    return @"C:\Program Files\TMT\iLog";
            //}
        }

        public string GetDLLPath()
        {
            string dllPath = ConfigurationManager.AppSettings["DllPath"];
            if (string.IsNullOrWhiteSpace(dllPath))
                return DefaultPath();

            return dllPath;
        }
    }
}