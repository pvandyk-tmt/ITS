#region

using System;
using System.Reflection;
using System.Text;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Enums;

#endregion

namespace Kapsch.DistanceOverTime.Adapter.Framework
{
    public class Defaults
    {
        [CommandLine(Option = "-listenerType", Default = "M",
            Example = "M, S, F",
            Help = "S = Socket, M = Mock and F = File (Disk) Based",
            Required = true)]
        public string ListenerType { get; set; }

        [CommandLine(Option = "-ipA", Default = "127.0.0.1:80",
            Example = "127.0.0.1",
            Help = "IP Address and Port of Camera for Point A when Service Type = S",
            Required = true)]
        public string IpAndPortA { get; set; }

        [CommandLine(Option = "-ipB", Default = "127.0.0.1:80",
            Example = "127.0.0.1",
            Help = "IP Address and Port of Camera for Point B when Service Type = S",
            Required = true)]
        public string IpAndPortB { get; set; }

        [CommandLine(Option = "-listenMs", Default = "5000",
            Example = "5000",
            Help = "Listen interval in milliseconds for Cameras for Point A and B when Service Type = S",
            Required = true)]
        public int ListenEveryMilliseconds { get; set; }

        public ListenerTypeEnum Listener
        {
            get
            {
                switch (ListenerType)
                {
                    case "M":
                        return ListenerTypeEnum.Mock;

                    case "D":
                        return ListenerTypeEnum.Disk;

                    case "S":
                        return ListenerTypeEnum.Socket;
                }

                return ListenerTypeEnum.Mock;
            }
        }

        public void Dump(StringBuilder builder)
        {
            PropertyInfo[] propInfos = GetType().GetProperties();
            foreach (PropertyInfo propInfo in propInfos)
            {
                builder.Append(propInfo.Name);
                builder.Append("=");
                builder.AppendLine(Convert.ToString(propInfo.GetGetMethod().Invoke(this, null)));
            }
        }

        public bool HasErrors()
        {
            bool result = false;

            if (string.IsNullOrEmpty(ListenerType))
            {
                Console.WriteLine("Please define the Service Type to run:");
                Console.WriteLine("Type '-serviceType M' (Default) for connecting as a Mock");
                Console.WriteLine("Type '-serviceType S' for connecting to a camera via a Socket");
                Console.WriteLine("Type '-serviceType D' for connecting to files on a Disk");

                result = true;
            }

            if (ListenerType.ToUpper() == "S")
            {
                if (string.IsNullOrEmpty(IpAndPortA))
                {
                    Console.WriteLine("Selecting a Service Type of Socket (S) you must enter the IP Address and Port to Connect to Point A:");
                    Console.WriteLine("Type '-ipA 127.0.0.1:80' for connecting to a camera with IP Address 127.0.0.1 and Port 80");
                    result = true;
                }

                if (string.IsNullOrEmpty(IpAndPortB))
                {
                    Console.WriteLine("Selecting a Service Type of Socket (S) you must enter the IP Address and Port to Connect to Point B:");
                    Console.WriteLine("Type '-ipB 127.0.0.1:80' for connecting to a camera with IP Address 127.0.0.1 and Port 80");
                    result = true;
                }
            }

            if (ListenerType.ToUpper() == "D")
            {
                if (string.IsNullOrEmpty(Helper.PathPointA))
                {
                    Console.WriteLine("Selecting a Listener Type of Disk (D) you must enter the Path Connect to Files stored for Point A:");
                    Console.WriteLine("Update the Application.Configuration file to e.g. 'C:\\Temp' for connecting to a files stored in the Temp Folder");
                    result = true;
                }

                if (string.IsNullOrEmpty(Helper.PathPointB))
                {
                    Console.WriteLine("Selecting a Listener Type of Disk (D) you must enter the Path Connect to Files stored for Point B:");
                    Console.WriteLine("Update the Application.Configuration file to e.g. 'C:\\Temp' for connecting to a files stored in the Temp Folder");
                    result = true;
                }

                if (string.IsNullOrEmpty(Helper.PathFilter))
                {
                    Console.WriteLine("Selecting a Listener Type of Disk (D) you must enter the Path Filter to find Files stored for Point A and B:");
                    Console.WriteLine("Update the Application.Configuration file to e.g. *.enc' for finding all files with enc extension stored in the indicative folders for Points A and B respectively");
                    result = true;
                }
            }

            return result;
        }
    }
}