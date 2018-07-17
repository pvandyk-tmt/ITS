using System;

namespace TMT.Core.Camera.Base
{
    public class cPicture : IDisposable, ICloneable
    {
        public cPicture()
        {
            pIsPlateImage = false;
        }

        #region JPEG

        public byte[] pJpeg { get; set; }

        public virtual string pJpegHash
        {
            get
            {
                if (pJpeg == null || pJpeg.Length < 50)
                    return string.Empty;

                byte[] bytesToHash = cSecurity.getBytesToHash(pJpeg);
                return cSecurity.computeHash(bytesToHash);
            }
        }

        public string pImageDescription { get; set; }

        public string pJpegFileName { get; set; }

        public bool pIsPlateImage { get; set; }

        #endregion

        public void Dispose()
        {
            pJpeg = null;
        }

        public object Clone()
        {
            cPicture pic = new cPicture
                {
                    pImageDescription = this.pImageDescription,
                    pJpeg = this.pJpeg,
                    pJpegFileName = pJpegFileName,
                    pIsPlateImage = pIsPlateImage
                };

            return pic;
        }
    }
}
