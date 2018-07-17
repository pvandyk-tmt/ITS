using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMT.Core.Camera.Base
{
    public class cEncryptedPicture : cEncryptedPictureFile, IDisposable
    {
        public byte[] pEncryptedFile { get; set; }

        public virtual string pEncryptedFileHash
        {
            get
            {
                if (pEncryptedFile == null)
                    return string.Empty;

                byte[] bytesToHash = new byte[20];
                int y = pEncryptedFile.Length / 20;
                int z = 0;
                for (int x = 0; x < pEncryptedFile.Length && z < 20; x += y)
                {
                    bytesToHash[z] = pEncryptedFile[x];
                    z++;
                }

                return cSecurity.computeHash(bytesToHash);
            }
        }

        public void Dispose()
        {
            pEncryptedFile = null;
        }
    }
}
