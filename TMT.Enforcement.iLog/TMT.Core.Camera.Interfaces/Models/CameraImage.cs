using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Core.Camera.Interfaces.Models
{
    public class CameraImage
    {
        public string Description { get; set; }
        public string FileName { get; set; }
        public bool IsPlateImage { get; set; }
        public byte[] ImageBuffer { get; set; }
        public string ImageMimeType { get; set; }
        public string ImageHash { get; set; }
    }
}
