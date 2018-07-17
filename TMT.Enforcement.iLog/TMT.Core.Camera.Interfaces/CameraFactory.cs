using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Core.Camera.Interfaces
{
    public static class CameraFactory
    {
        private static Assembly assembly;

        public static ICamera Find()
        {
            var configuration = new Configuration();
            string dllPath = configuration.GetDLLPath();

            if (assembly == null)
            {
                var assemblyFilePath = Path.Combine(dllPath, "TMT.Core.Camera.dll");
                assembly = Assembly.LoadFrom(assemblyFilePath);
            }
            
            var type = assembly.GetTypes().FirstOrDefault(f => typeof(ICamera).IsAssignableFrom(f) && !f.IsInterface);
            if (type == null)
                return null;

            return Activator.CreateInstance(type) as ICamera;                        
        }
    }
}