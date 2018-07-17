#region

using System.Collections;

#endregion

namespace Kapsch.DistanceOverTime.Adapter.Framework
{
    public static class HashtableExt
    {
        public static void Parse(this Hashtable hash, string[] args)
        {
            if (args.Length%2 == 0)
            {
                for (int i = 0; i <= args.Length - 1; i++)
                {
                    if (i % 2 == 0)
                    {
                        hash.Add(args[i], args[i + 1]);
                    }
                } 
            }
        }
    }
}