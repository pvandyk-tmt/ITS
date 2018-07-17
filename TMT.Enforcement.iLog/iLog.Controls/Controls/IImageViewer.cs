using System.Drawing;
using TMT.Core.Camera.Base;

namespace TMT.Enforcement.iLog.Controls.Controls
{
    public interface IImageViewer
    {
        void Clear();
        objImageViewer VisibleImage();
        bool AddPictureFile(cPictureFile pic, bool fitToFrame);
        Image ResizeImage(Image imgToResize, Size size);
        void Zoom();
    }
}
