using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TMT.Core.Camera.Utils;

namespace TMT.Core.Camera.Base
{
    public abstract class cCamera
    {
        public cCamera()
        {
        }

        public cCamera(string version, string name, string dateFormat, Size maxSize)
        {
            pName = name;
            pVersion = version;
            pCameraDateFormat = dateFormat;
            pImageMaxSize = maxSize;
        }

        protected virtual string pStatsFileName
        {
            get { return ""; }
        }

        protected virtual string pKeyFilename1
        {
            get { return ""; }
        }

        protected virtual string pKeyFilename2
        {
            get { return ""; }
        }

        public abstract string pEncFileName { get; }

        protected virtual string pVosiFileName
        {
            get { return ""; }
        }

        /// <summary>
        ///     Type Name for the camera - eg. Digicam or SafeTCam etc.
        /// </summary>
        public string pName { get; set; }

        /// <summary>
        ///     Friendly Name for the camera
        /// </summary>
        public string pVersion { get; set; }

        /// <summary>
        ///     Camera Date Format
        /// </summary>
        public string pCameraDateFormat { get; set; }

        /// <summary>
        ///     Maximum size the Camera Image can be
        /// </summary>
        public Size pImageMaxSize { get; set; }

        /// <summary>
        ///     Parent or home dir for the camera's films
        /// </summary>
        protected string pParentDirectory { get; set; }

        public virtual bool UpdateFileNumber(cPictureFile pictureFile, long fileNumber)
        {
            return true;
        }

        public void developFilm(cFilm film)
        {
            foreach (cEncryptedPictureFile enc in film.getEncryptedPictureFiles())
            {
                if (File.Exists(Path.Combine(enc.pEncryptedFilePath, enc.pEncryptedFileName)))
                {
                    developPictureFile(film, enc.pEncryptedFileName);
                }
            }
        }

        public virtual bool getFileNumber(string fileName, out long fileNumber, out string message)
        {
            return getFileNumber(fileName, "", out fileNumber, out message);
        }

        public virtual bool getFileNumber(string fileName, string filePath, out long fileNumber, out string message)
        {
            message = "";
            fileNumber = 0;

            string fileExcludingExtension = Path.GetFileNameWithoutExtension(fileName);

            if (fileExcludingExtension != null)
            {
                //Redlight, stop line and barrier line
                fileExcludingExtension = fileExcludingExtension.TrimEnd(new char[]{'r', 'l', 's'});

                int imgIndex = fileExcludingExtension.IndexOf("Img", StringComparison.InvariantCulture);
                if (imgIndex >= 0)
                {
                    if (!long.TryParse(fileExcludingExtension.Substring(imgIndex + 3, fileExcludingExtension.Length - (imgIndex + 3)), out fileNumber))
                    {
                        message = "Invalid picture file format - could not read picture number from encrypted file";
                        return false;
                    }

                    return true;
                }
            }

            message = "Invalid picture file format - could not read picture number from encrypted file";
            return false;
        }

        public abstract bool developPictureFile(cFilm film, string encFileName);

        public abstract bool developPicture(cPictureFile pictureFile, out List<cPicture> pictures, out string message);

        public virtual void readKeyFile(cFilm film)
        {
        }

        public virtual void ReadFilesFromDisk(cFilm film)
        {
            film.getEncryptedPictureFiles().Clear();

            string[] files = Directory.GetFiles(film.pPath, pEncFileName);

            foreach (string file in files)
            {
                var fi = new FileInfo(file);

                long fileNumber;
                string message;
                if (getFileNumber(fi.Name, fi.DirectoryName, out fileNumber, out message))
                { 
                    var enc = new cEncryptedPictureFile {pEncryptedFileName = fi.Name, pEncryptedFilePath = film.pPath, pEncryptedFileNumber = fileNumber};
                    film.addToEncryptedPictureFiles(enc);
                }
            }
        }

        public virtual void readStatsFile(cFilm film)
        {
        }

        public virtual void readVosiFile(cFilm film)
        {
        }

        public bool ResizeAndValidate(cPicture pic, out string message)
        {
            message = "";
           
            try
            {
                pic.pJpeg = cImage.ResizeImage(pic.pJpeg, pImageMaxSize);
            }
            catch (Exception ex)
            {
                message = pic.pJpegFileName + " Error, " + ex.Message;
                return false;
            }

            return true;
        }

        public virtual int getDBInfringementType(string cameraInfringementType)
        {
            return (int)InfrigementType.Unknown;
        }

        public virtual string developVideo(cFilm film, string filePath, string fileName)
        {
            return string.Empty;
        }
    }
}