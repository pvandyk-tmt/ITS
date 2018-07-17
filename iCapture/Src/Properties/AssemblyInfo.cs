﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("iCapture")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Kapsch SA Holdings")]
[assembly: AssemblyProduct("iApps")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("BC694792-F669-4C8F-9D48-3696C24E819A")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
//[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyVersion("1.0.986")]
//[assembly: AssemblyFileVersion("1.0.0.0")]

public static class SVNInfo
{
    public static string mRevision = "986";
    public static string mDate = "2018/05/18 09:17:29";

    private static AssemblyFileVersionAttribute fileAttr = System.Attribute.GetCustomAttribute(typeof(SVNInfo).Assembly, typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;

    public static string mVersion = fileAttr.Version;
    public static string mTooltip = string.Format("Stamped {0} {1}", mDate, (fileAttr.Version.IndexOf("beta") > 0 ? "\nBeta version, not for release." : ""));
}
