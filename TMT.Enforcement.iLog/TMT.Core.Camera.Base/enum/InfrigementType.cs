using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMT.Core.Camera.Base
{
    public enum InfrigementType
    {
        Unknown = 0,
        LaserCam = 1,
        Static = 2,
        FreeDigital = 3,
        RobotCam = 4,
        SafeTcam = 5,
        RedLight = 6,
        SpeedOverDistance = 7,
        Video = 8,
        YellowLine = 9,
        LineViolation = 10,
        Headway = 11,
        StopLine = 12
    }
}
