using Kapsch.Camera.Listener.Base;
using Kapsch.Camera.Listener.Listeners.iCam;
using Kapsch.Camera.Listener.Listeners.Mock;
using Kapsch.Device.Listener.Listeners;
using Kapsch.DistanceOverTime.Adapter.Framework;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Enums;

namespace Kapsch.DistanceOverTime.Adapter.Factory
{
    public class ListenerFactory
    {
        public enum PointDefinition
        {
            PointA = 0,
            PointB = 1
        }

        public static BaseCameraListener GetListener(Defaults defaults, PointDefinition pointDefinition)
        {
            switch (defaults.Listener)
            {
                case ListenerTypeEnum.Socket:
                    switch (pointDefinition)
                    {
                        case PointDefinition.PointA:
                            SocketConfigurationModel pointA = new SocketConfigurationModel
                            {
                                ListenEveryMilliseconds = defaults.ListenEveryMilliseconds,
                                TimeoutInMilliseconds = 60000,
                                IpAddress = defaults.IpAndPortA.Split(':')[0],
                                IpPort = int.Parse(defaults.IpAndPortA.Split(':')[1])
                            };

                            return new iCamCameraListener(new SocketListener(pointA));

                        case PointDefinition.PointB:
                            SocketConfigurationModel pointB = new SocketConfigurationModel
                            {
                                ListenEveryMilliseconds = defaults.ListenEveryMilliseconds,
                                TimeoutInMilliseconds = 60000,
                                IpAddress = defaults.IpAndPortB.Split(':')[0],
                                IpPort = int.Parse(defaults.IpAndPortB.Split(':')[1])
                            };

                            return new iCamCameraListener(new SocketListener(pointB));
                    }
                    break;

                case ListenerTypeEnum.Mock:
                    switch (pointDefinition)
                    {
                        case PointDefinition.PointA:
                            MockConfigurationModel modelStart = new MockConfigurationModel
                            {
                                ListenEveryMilliseconds = defaults.ListenEveryMilliseconds,
                                TimeOffsetSecondsStart = 0,
                                TimeOffsetSecondsEnd = 5,
                                Seed = 1500,
                                DeviceId = 1,
                                DeviceName = "Mock Device A",
                                LocationCode = "MockA"
                            };
 
                            return new MockCameraListener(new MockListener(modelStart));

                        case PointDefinition.PointB:
                            MockConfigurationModel modelEnd = new MockConfigurationModel
                            {
                                ListenEveryMilliseconds = defaults.ListenEveryMilliseconds,
                                TimeOffsetSecondsStart = 250,
                                TimeOffsetSecondsEnd = 360,
                                Seed = 2500,
                                DeviceId = 2,
                                DeviceName = "Mock Device B",
                                LocationCode = "MockB"
                            };
                            return new MockCameraListener(new MockListener(modelEnd));
                    }
                    break;

                case ListenerTypeEnum.Disk:
                    switch (pointDefinition)
                    {
                        case PointDefinition.PointA:
                            DiskConfigurationModel pointA = new DiskConfigurationModel
                            {
                                ListenEveryMilliseconds = defaults.ListenEveryMilliseconds,
                                FilePath = Helper.PathPointA,
                                SearchPattern = Helper.PathFilter
                            };

                            return new iCamCameraListener(new DiskListener(pointA));

                        case PointDefinition.PointB:
                            DiskConfigurationModel pointB = new DiskConfigurationModel
                            {
                                ListenEveryMilliseconds = defaults.ListenEveryMilliseconds,
                                FilePath = Helper.PathPointB,
                                SearchPattern = Helper.PathFilter
                            };

                            return new iCamCameraListener(new DiskListener(pointB));
                    }
                    break;
            }

            return null;
        }
    }
}