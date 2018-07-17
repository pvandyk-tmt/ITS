using System.Drawing;
using iLog.Camera.Base;

namespace TMT.iLog.Controls
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
