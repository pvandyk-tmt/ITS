using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace TMT.Core.Camera.Utils
{
    public class cImage
    {
        public static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static byte[] ResizeImage(byte[] imgToResize, Size maxSize)
        {           
            using (Image img = cImage.ByteArrToImage(imgToResize))
            {                
                int sourceWidth = img.Width;
                int sourceHeight = img.Height;

                if (sourceWidth < maxSize.Width || sourceHeight < maxSize.Height)
                {
                    return imgToResize;
                }

                float nPercentW = ((float)maxSize.Width / (float)sourceWidth);
                float nPercentH = ((float)maxSize.Height / (float)sourceHeight);
                float nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                Bitmap b = new Bitmap(destWidth, destHeight);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.DrawImage(img, 0, 0, destWidth, destHeight);
                }

                return ImageToByteArray(b);
            }
        }

        public static Image ByteArrToImage(byte[] myByteArray)
        {
            using (var ms = new MemoryStream(myByteArray, 0, myByteArray.Length))
            {
                ms.Write(myByteArray, 0, myByteArray.Length);
                return Image.FromStream(ms, true);
            }
        }

        public static byte[] ImageToByteArray(string fileName)
        {
            try
            {
                // provide read access to the file
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    // Create a byte array of file stream length
                    var imageData = new byte[fs.Length];

                    //Read block of bytes from stream into the byte array
                    fs.Read(imageData, 0, Convert.ToInt32(fs.Length));

                    //Close the File Stream
                    fs.Close();
                    fs.Dispose();

                    return imageData;
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                return null;
            }
        }
    }
}