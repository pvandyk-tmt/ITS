using System.Configuration;

namespace Kapsch.DistanceOverTime.Adapter
{
    public static class Helper
    {
        public static string PhysicalInfringementPath
        {
            get
            {
                string keyStrVal = ConfigurationManager.AppSettings["PhysicalInfringementPath"];
                if (!string.IsNullOrEmpty(keyStrVal))
                {
                    return keyStrVal;
                }
                return "";
            }
        }

        public static string PathPointA
        {
            get
            {
                string keyStrVal = ConfigurationManager.AppSettings["PathPointA"];
                if (!string.IsNullOrEmpty(keyStrVal))
                {
                    return keyStrVal;
                }
                return "";
            }
        }

        public static string PathPointB
        {
            get
            {
                string keyStrVal = ConfigurationManager.AppSettings["PathPointB"];
                if (!string.IsNullOrEmpty(keyStrVal))
                {
                    return keyStrVal;
                }
                return "";
            }
        }

        public static string PathFilter
        {
            get
            {
                string keyStrVal = ConfigurationManager.AppSettings["PathFilter"];
                if (!string.IsNullOrEmpty(keyStrVal))
                {
                    return keyStrVal;
                }
                return "";
            }
        }

        public static int ListenerCounterTimeout
        {
            get
            {
                string keyStrVal = ConfigurationManager.AppSettings["ListenerCounterTimeout"];
                if (!string.IsNullOrEmpty(keyStrVal))
                {
                    return int.Parse(keyStrVal);
                }

                return 100;
            }
        }

        public static int HeartbeatSeconds
        {
            get
            {
                string keyStrVal = ConfigurationManager.AppSettings["HeartbeatSeconds"];
                if (!string.IsNullOrEmpty(keyStrVal))
                {
                    return int.Parse(keyStrVal);
                }

                return 120;
            }
        }
    }
}