namespace TMT.Core.Camera.Base
{
    public class cEncryptedPictureFile
    {
        public string pEncryptedFileName { get; set; }

        public string pEncryptedFilePath { get; set; }

        public long pEncryptedFileNumber { get; set; }

        /// <summary>
        /// Database can only take number that contains 5 or less characters
        /// </summary>
        public long encryptedFileNumberDb 
        {
            get 
            {
                string fs = pEncryptedFileNumber.ToString("00000");
                long fileNumber;
                if (!long.TryParse(fs.Substring(fs.Length - 5, 5), out fileNumber))
                {
                    return 0;
                }

                return fileNumber;

            }
        }

        // ajm 20081201 digicam provision
        //int fileNumber = 0;size
        //if (imgIndex == -1)
        //{
        //    string fileIndex = EncryptedFileName.Substring(EncryptedFileName.LastIndexOf(".jpg") - 1, 1);
        //    return Convert.ToInt32(Date + fileIndex);
        //}
        //else
        //    return Convert.ToInt32(EncryptedFileName.Substring(imgIndex + 3, 3));
    }
}