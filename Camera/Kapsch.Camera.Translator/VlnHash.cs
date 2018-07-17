using System;
using System.ComponentModel;

namespace Kapsch.DistanceOverTime.Adapter
{
    public static class VlnHash
    {
        public static string[] Set = {"OD0Q", "I1", "S5", "VY", "38B", "2Z", "EF", "GC", "HMNW"};

        public static string Hash(string vln)
        {
            var newString = new char[vln.Length];

            for (var i = 0; i < vln.Length; i++)
            {
                var found = false;

                foreach (var setItem in Set)
                {
                    foreach (var replaceItem in setItem)
                    {
                        if (vln[i] != replaceItem) continue;

                        newString[i] = setItem[0];
                        found = true;
                    }
                }

                if (!found)
                {
                    newString[i] = vln[i];
                }
            }

            return new String(newString);
        }
    }
}