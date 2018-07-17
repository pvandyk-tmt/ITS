#region

using System;
using System.Collections;
using System.Reflection;
using System.Text;

#endregion

namespace Kapsch.DistanceOverTime.Adapter.Framework
{
    public class CommandLineParser
    {
        protected Hashtable Args = new Hashtable();

        public CommandLineParser(string[] args)
        {
            Args.Parse(args);
        }

        public void Parse(object fillObject)
        {
            Type type = fillObject.GetType();

            PropertyInfo[] propInfos = type.GetProperties();
            foreach (PropertyInfo info in propInfos)
            {
                ProcessProp(fillObject, info);
            }
        }
        
        private void ProcessProp(object fillObject, PropertyInfo info)
        {
            CommandLineAttribute[] argAttrs = (CommandLineAttribute[]) info.GetCustomAttributes(typeof (CommandLineAttribute), true);

            foreach (CommandLineAttribute attr in argAttrs)
            {
                if (Args.ContainsKey(attr.Option))
                {
                    info.GetSetMethod().Invoke(fillObject, info.PropertyType == typeof (Int32) ? new object[] {Convert.ToInt32(Args[attr.Option])} : new[] {Args[attr.Option]});
                }
                else if (attr.AlternateOption != null && Args.ContainsKey(attr.AlternateOption))
                {
                    info.GetSetMethod().Invoke(fillObject, info.PropertyType == typeof (Int32) ? new object[] {Convert.ToInt32(Args[attr.AlternateOption])} : new[] {Args[attr.AlternateOption]});
                }
                else
                {
                    // use the default to populate the value
                    ReflectionUtils.SetProperty(fillObject, info, attr.Default);
                }
            }
        }
        
        public void Help(object fillObject)
        {
            StringBuilder builder = new StringBuilder();
            Help(builder, fillObject);
            Console.WriteLine(builder.ToString());
        }
        
        public void Help(StringBuilder builder, object fillObject)
        {
            PropertyInfo[] propInfos = fillObject.GetType().GetProperties();
            foreach (PropertyInfo info in propInfos)
            {
                CommandLineAttribute[] argAttrs = (CommandLineAttribute[]) info.GetCustomAttributes(typeof (CommandLineAttribute), true);
                foreach (CommandLineAttribute argAttr in argAttrs)
                {
                    builder.Append(argAttr.Option.PadRight(5));
                    builder.Append(" " + argAttr.Help);
                    builder.Append(argAttr.Required ? " [required] " : " [not required] ");
                    
                    builder.Append(Environment.NewLine);

                    if (!StringExt.IsNullOrEmpty(argAttr.Default))
                    {
                        builder.Append("Default: " + argAttr.Default + ", ");
                    }

                    if (!StringExt.IsNullOrEmpty(argAttr.AlternateOption))
                    {
                        builder.Append("Alternate Option: " + argAttr.AlternateOption + ", ");
                    }

                    if (!StringExt.IsNullOrEmpty(argAttr.Example))
                    {
                        builder.Append("Example: " + argAttr.Example);
                    }

                    builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);
                }
            }
        }
    }

    internal static class ReflectionUtils
    {
        public static void SetProperty(object destinObject, PropertyInfo info, object value)
        {
            object destinValue = TypeConverter.Convert(value, info.PropertyType);
            info.GetSetMethod().Invoke(destinObject, new[] {destinValue});
        }
    }

    internal static class TypeConverter
    {
        public static object Convert(object value, Type toType)
        {
            if (toType == typeof (Int32)) return System.Convert.ToInt32(value);
            if (toType == typeof (string)) return System.Convert.ToString(value);
            if (toType == typeof (Boolean)) return System.Convert.ToBoolean(value);

            throw new NotImplementedException();
        }
    }
}