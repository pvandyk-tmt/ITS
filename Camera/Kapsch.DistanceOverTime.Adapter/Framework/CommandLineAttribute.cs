#region

using System;

#endregion

namespace Kapsch.DistanceOverTime.Adapter.Framework
{
    public class CommandLineAttribute : Attribute
    {
        public string AlternateOption;
        public string Default;
        public string Example;
        public string Help;
        public string Option;
        public bool Required;
    }
}