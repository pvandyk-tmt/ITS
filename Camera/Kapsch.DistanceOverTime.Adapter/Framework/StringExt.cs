namespace Kapsch.DistanceOverTime.Adapter.Framework
{
    public static class StringExt
    {
        public static string Left(this string source, int index)
        {
            if (index > source.Length)
            {
                index = source.Length;
            }
            return source.Substring(0, index);
        }
        
        public static string Right(this string source, int index)
        {
            return source.Substring(index);
        }
        
        public static bool IsNullOrEmpty(string source)
        {
            if (source == null) return true;
            if (source.Trim().Length == 0) return true;

            return false;
        }
    }
}