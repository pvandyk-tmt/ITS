using System;
using System.Collections.Generic;
using System.Reflection;
using TMT.Core.Camera;
using TMT.Core.Camera.Interfaces.Models;

namespace TMT.Core.Camera.Interfaces
{
    public interface ICamera
    {
        IList<CameraOffence> ExtractInfringmentImages(string configPath, string filePath);
        string ExtractInfringementVideo(string pathSource, string dllPath);
        void LoadImplementations(string dllPath);
    }
}
