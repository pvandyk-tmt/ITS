#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace TMT.Core.Camera.Base
{
    public class cFilm : IDisposable, ICloneable
    {
        #region eGroupingType enum

        //p_grouping_type = 1 => session per location
        //p_grouping_type = 2 => session per day
        public enum eGroupingType
        {
            SessionPerLocation = 1,
            SessionPerDay = 2
        }

        #endregion

        public cFilm()
        {
            pGroupType = eGroupingType.SessionPerLocation;

            pStatsCollection = new List<IStatEntry>();
            pVosiCollection = new List<IVosiEntry>();
            pPictureFileCollection = new List<cPictureFile>();
            pEncryptedPictureFileCollection = new List<cEncryptedPictureFile>();
            pEncryptionKey = "";

            pLowSpeedCount = 0;
            pHighSpeedCount = 0;

            pLowestSpeed = 0;
            pHighestSpeed = 0;

            pInfringements = 0;
            pVehiclesChecked = 0;
            pMeasurementErrors = 0;
            pCaptureErrors = 0;
            pTestPhotos = 0;
            pJammerCount = 0;
            pAverageSpeed = 0;
        }

        #region Film Properties

        public string pPath { get; set; }
        public cCamera pCameraDriver { get; set; }
        public string pStatsFileName { get; set; }
        public DateTime? pCreationTime { get; set; }
        public List<IStatEntry> pStatsCollection { get; set; }
        public string pEncryptionKey { get; set; }
        public string pVosiFileName { get; set; }
        public List<IVosiEntry> pVosiCollection { get; set; }

        /// <summary>
        /// Returns the days the camera captured pics from the first CamDate
        /// </summary>
        public int pNumDays
        {
            get
            {
                int numdays = 0;

                if (pHasEncryptedPictureFiles)
                {
                    cPictureFile first = getFirstValidPictureFile();
                    cPictureFile last = getLastValidPictureFile();

                    if (first != null && last != null)
                    {
                        if (first.pOffenceDate != null && last.pOffenceDate != null)
                        {
                            DateTime startDate = first.pOffenceDate.Value;
                            DateTime endDate = last.pOffenceDate.Value;

                            TimeSpan diff = endDate.Date.Subtract(startDate.Date);
                            double days = diff.TotalDays;
                            if ((int) days > numdays)
                            {
                                numdays = (int) days;
                            }
                        }
                    }
                }

                return numdays;
            }
        }

        public bool pHasEncryptedPictureFiles
        {
            get { return pEncryptedPictureFileCollection.Count > 0; }
        }

        public bool pHasKeyFile
        {
            get { return pEncryptionKey.Length > 0; }
        }

        public bool pHasStatsFile
        {
            get { return pStatsCollection.Count > 0; }
        }

        public bool pHasVosiFile
        {
            get { return pVosiCollection.Count > 0; }
        }

        public bool pHasErrors
        {
            get
            {
                if (getFirstValidPictureFile() == null)
                {
                    return true;
                }

                return getFirstValidPictureFile().pHasError;
            }
        }

        public eGroupingType pGroupType { get; set; }

        public string pGroup
        {
            get
            {
                cPictureFile pic = getFirstValidPictureFile();
                if (pic == null)
                {
                    return "";
                }

                switch (pGroupType)
                {
                    case eGroupingType.SessionPerDay:
                        return pic.pOffenceDateStringMMDD + "-" + pic.pFormattedSession;

                    case eGroupingType.SessionPerLocation:
                        return pic.pLocationCode + "-" + pic.pFormattedSession;

                    default:
                        return pic.pFormattedSession;
                }
            }
        }

        #endregion

        #region Stats

        private int mTotalSpeed;

        public DateTime? pStartDate { get; set; }
        public DateTime? pEndDate { get; set; }
        public int pLowSpeedCount { get; set; }
        public int pHighSpeedCount { get; set; }
        public int pInfringements { get; set; }
        public int pVehiclesChecked { get; set; }
        public int pMeasurementErrors { get; set; }
        public int pCaptureErrors { get; set; }
        public int pTestPhotos { get; set; }
        public decimal pAverageSpeed { get; set; }
        public int pHighestSpeed { get; set; }
        public int pLowestSpeed { get; set; }
        public int pJammerCount { get; set; }

        public bool pHasStatsSummary
        {
            get
            {
                if (pHasStatsFile)
                {
                    if (pStartDate == null)
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }
        }

        public string pStartDateString
        {
            get
            {
                if (pStartDate.HasValue)
                {
                    return pStartDate.Value.ToString("dd/MM/yyyy HH:mm:ss ");
                }

                return "";
            }
        }

        public string pEndDateString
        {
            get
            {
                if (pEndDate.HasValue)
                {
                    return pEndDate.Value.ToString("dd/MM/yyyy HH:mm:ss ");
                }

                return "";
            }
        }

        public void applyStats()
        {
            if (pHasStatsSummary)
            {
                return;
            }

            if (!pHasKeyFile)
            {
                pCameraDriver.readKeyFile(this);
            }

            if (!pHasStatsFile)
            {
                pCameraDriver.readStatsFile(this);
            }

            if (!pHasVosiFile)
            {
                pCameraDriver.readVosiFile(this);
            }

            pLowSpeedCount = 0;
            pHighSpeedCount = 0;
            pLowestSpeed = 0;
            pHighestSpeed = 0;
            pInfringements = 0;
            pVehiclesChecked = 0;
            pMeasurementErrors = 0;
            pCaptureErrors = 0;
            pTestPhotos = 0;

            if (pHasStatsFile)
            {
                foreach (IStatEntry statEntry in pStatsCollection)
                {
                    string error;
                    statEntry.Extract(out error);

                    if (error.Length == 0)
                    {
                        if (statEntry.Captured && getPicturesFiles() != null)
                        {
                            foreach (cPictureFile pf in getPicturesFiles())
                            {
                                if ((!string.IsNullOrEmpty(pf.pEncryptedPicture.pEncryptedFileName)) && (pf.pEncryptedPicture.pEncryptedFileName.ToLower() == statEntry.EncryptedFilename.ToLower()))
                                {
                                    syncStatWithFile(statEntry, pf);
                                    break;
                                }
                            }
                        }

                        if (statEntry.Speed <= 300)
                        {
                            pLowSpeedCount += statEntry.LowSpeed;
                            pHighSpeedCount += statEntry.HighSpeedNonInfringement;
                            pInfringements += statEntry.Infringement;
                            pMeasurementErrors += statEntry.MeasurementErrors;
                            pCaptureErrors += statEntry.CaptureErrors;
                            pTestPhotos += statEntry.TestPhoto;

                            try
                            {
                                if (statEntry.Zone > 0)
                                {
                                    if (pStartDate == null)
                                    {
                                        pStartDate = Convert.ToDateTime(statEntry.Date.Replace("\0", "").Trim() + " " + statEntry.Time);
                                    }

                                    pEndDate = Convert.ToDateTime(statEntry.Date.Replace("\0", "").Trim() + " " + statEntry.Time);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.Write(e);
                            }

                            if (statEntry.MeasurementErrors == 0 && statEntry.CaptureErrors == 0 && statEntry.TestPhoto == 0)
                            {
                                pVehiclesChecked += statEntry.VehiclesChecked;
                                addSpeed(statEntry.Speed);
                            }
                        }
                        else
                        {
                            pJammerCount += 1;
                        }
                    }
                }
            }
        }  

        private void addSpeed(int speed)
        {
            if (speed > pHighestSpeed)
            {
                pHighestSpeed = speed;
            }

            if (speed < pLowestSpeed || (pLowestSpeed == 0 & speed > 0))
            {
                pLowestSpeed = speed;
            }

            mTotalSpeed += speed;

            calcAverageSpeed();
        }

        public void calcAverageSpeed()
        {
            pAverageSpeed = pVehiclesChecked != 0 ? Math.Round(Decimal.Divide(mTotalSpeed, pVehiclesChecked), 2) : 0;
        }

        private static void syncStatWithFile(IStatEntry entry, cPictureFile file)
        {
            string entryDistance = entry.Distance.Replace("m", "");

            try
            {
                file.pDistance = Convert.ToDouble(entryDistance);
            }
            catch
            {
                file.pDistance = 0.0;
            }

            file.pDirection = entry.Direction;
        }

        #endregion

        private List<cPictureFile> pPictureFileCollection { get; set; }
        public List<cEncryptedPictureFile> pEncryptedPictureFileCollection { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            clearFilm();
        }

        #endregion

        /// <summary>
        /// Returns the picture for the enc file name
        /// </summary>
        /// <param name="encryptedFileName"></param>
        /// <returns></returns>
        public cPictureFile getPictureFile(string encryptedFileName)
        {
            if (pHasEncryptedPictureFiles)
            {
                cPictureFile file;
                if (existsPictureFile(encryptedFileName, out file))
                {
                    return file;
                }

                cCamera camera = pCameraDriver;
                bool pictureDeveloped;
                pictureDeveloped = camera.developPictureFile(this, encryptedFileName);
                if (!pictureDeveloped)
                    return null;

                return getPictureFile(encryptedFileName);
            }

            return null;
        }

        /// <summary>
        /// Returns the first picture file in the film regardless
        /// </summary>
        /// <returns></returns>
        public cPictureFile getFirstPictureFile()
        {
            if (pHasEncryptedPictureFiles)
            {
                return getPictureFile(pEncryptedPictureFileCollection[0].pEncryptedFileName);
            }

            return null;
        }

        /// <summary>
        /// Returns the first [VALID] picture file in the film
        /// </summary>
        /// <returns></returns>
        public cPictureFile getFirstValidPictureFile()
        {
            if (pHasEncryptedPictureFiles)
            {
                int i = 0;
                cPictureFile file = getPictureFile(pEncryptedPictureFileCollection[i].pEncryptedFileName);

                while (file == null || file.pHasError)
                {
                    if (i > pEncryptedPictureFileCollection.Count - 1)
                    {
                        break;
                    }

                    file = getPictureFile(pEncryptedPictureFileCollection[i].pEncryptedFileName);

                    i++;
                }

                return file;
            }

            return null;
        }

        /// <summary>
        /// Returns the last [VALID] picture file in the film
        /// </summary>
        /// <returns></returns>
        public cPictureFile getLastValidPictureFile()
        {
            if (pHasEncryptedPictureFiles)
            {
                int i = pEncryptedPictureFileCollection.Count - 1;
                cPictureFile file = getPictureFile(pEncryptedPictureFileCollection[i].pEncryptedFileName);

                while (file == null || file.pHasError)
                {
                    if (i < 0)
                    {
                        break;
                    }

                    file = getPictureFile(pEncryptedPictureFileCollection[i].pEncryptedFileName);

                    i--;
                }

                return file;
            }

            return null;
        }

        /// <summary>
        /// Returns the last picture file in the film regardless
        /// </summary>
        /// <returns></returns>
        public cPictureFile getLastPictureFile()
        {
            if (pHasEncryptedPictureFiles)
            {
                return getPictureFile(pEncryptedPictureFileCollection[pEncryptedPictureFileCollection.Count - 1].pEncryptedFileName);
            }

            return null;
        }

        /// <summary>
        /// Safe way to add to the Pictures Collection once developed
        /// </summary>
        /// <param name="file"></param>
        public void addToPictureFiles(cPictureFile file)
        {
            bool found = pPictureFileCollection.Any(pictureFile => pictureFile.pEncryptedPicture.pEncryptedFileName == file.pEncryptedPicture.pEncryptedFileName);
            if (!found)
            {
                pPictureFileCollection.Add(file);
            }
        }

        public bool existsPictureFile(string encryptedFileName, out cPictureFile file)
        {
            foreach (var pictureFile in pPictureFileCollection.Where(pictureFile => pictureFile.pEncryptedPicture.pEncryptedFileName == encryptedFileName))
            {
                file = pictureFile;
                return true;
            }

            file = null;
            return false;
        }

        public List<cPictureFile> getPicturesFiles()
        {
            if (pPictureFileCollection.Count > 0)
                pPictureFileCollection.Sort(compare);

            return pPictureFileCollection;
        }

        private static int compare(cPictureFile b, cPictureFile a)
        {
            int diff = 0;

            if (b.pOffenceDate != null && a.pOffenceDate != null)
            {
                diff = DateTime.Compare(b.pOffenceDate.Value, a.pOffenceDate.Value);
                return diff;
            }

            diff = b.pEncryptedPicture.pEncryptedFileNumber.CompareTo(a.pEncryptedPicture.pEncryptedFileNumber);
            return diff;
        }

        public void addToEncryptedPictureFiles(cEncryptedPictureFile enc)
        {
            bool found = pEncryptedPictureFileCollection.Any(encryptedPictureFile => encryptedPictureFile.pEncryptedFileName == enc.pEncryptedFileName);
            if (!found)
            {
                pEncryptedPictureFileCollection.Add(enc);
            }
        }

        public List<cEncryptedPictureFile> getEncryptedPictureFiles()
        {
            if (pEncryptedPictureFileCollection.Count > 0)
                pEncryptedPictureFileCollection.Sort(compare);

            return pEncryptedPictureFileCollection;
        }

        private static int compare(cEncryptedPictureFile b, cEncryptedPictureFile a)
        {
            int diff = b.pEncryptedFileNumber.CompareTo(a.pEncryptedFileNumber);
            return diff;
        }

        public virtual bool isDeveloped(cEncryptedPictureFile enc)
        {
            return pPictureFileCollection.Where(pictureFile => pictureFile.pEncryptedPicture.pEncryptedFileName == enc.pEncryptedFileName).Any(pictureFile => pictureFile.pHasDecryptedData);
        }

        public virtual void clearFilm()
        {
            pPictureFileCollection.Clear();
        }

        public object Clone()
        {
            cFilm film = new cFilm 
            {
                pAverageSpeed = this.pAverageSpeed,
                mTotalSpeed = this.mTotalSpeed,
                pCameraDriver = this.pCameraDriver,
                pCaptureErrors = this.pCaptureErrors,
                pCreationTime = this.pCreationTime,
                pEncryptionKey = this.pEncryptionKey,
                pEndDate = this.pEndDate,
                pGroupType = this.pGroupType,
                pHighestSpeed = this.pHighestSpeed,
                pInfringements = this.pInfringements,
                pHighSpeedCount = this.pHighSpeedCount,
                pJammerCount = this.pJammerCount,
                pLowestSpeed = this.pLowestSpeed,
                pLowSpeedCount = this.pLowSpeedCount,
                pMeasurementErrors = this.pMeasurementErrors,
                pPath = this.pPath,
                pStartDate = this.pStartDate,
                pStatsFileName = this.pStatsFileName,
                pTestPhotos = this.pTestPhotos,
                pVehiclesChecked = this.pVehiclesChecked
            };

            film.pPictureFileCollection = new List<cPictureFile>();
            foreach (cPictureFile f in this.pPictureFileCollection)
            {
                film.pPictureFileCollection.Add(f);
            }

            film.pEncryptedPictureFileCollection = new List<cEncryptedPictureFile>();
            foreach (cEncryptedPictureFile f in this.pEncryptedPictureFileCollection)
            {
                film.pEncryptedPictureFileCollection.Add(f);
            }

            film.pStatsCollection = new List<IStatEntry>();
            foreach (IStatEntry f in this.pStatsCollection)
            {
                film.pStatsCollection.Add(f);
            }

            film.pVosiCollection = new List<IVosiEntry>();
            foreach (IVosiEntry f in this.pVosiCollection)
            {
                film.pVosiCollection.Add(f);
            }

            return film;
        }
    }
}