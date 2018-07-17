#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

#endregion

namespace TMT.Core.Camera.Base
{
    public class cPictureFile
    {
        private DateTime? _offenceDate;
        public string pCameraType { get; set; }

        public cPictureFile()
        {
            pExtraInfo = string.Empty;
            pErrorCollection = new List<string>();
            pPlates = new List<cPlate>();

            pTest = "";

            pZone = 0;

            pZoneL = 0;
            pZoneH = 0;
            pZonePT = 0;

            OffenceCode = "";
            FileName = "";
            SourceFileName = "";
            OfficerCode = "";
        }

        public bool pHasDecryptedData { get; set; }
        public int pNumberOfFrames { get; set; }

        public int? pSpeed { get; set; }

        public int pZone { get; set; }


        public int pZoneL { get; set; }
        public int pZoneH { get; set; }
        public int pZonePT { get; set; }

        public int? pDecodeErrorNo { get; set; }
        public string pInfringementType { get; set; }
        public string pDecodeErrorMessage { get; set; }
        public string pExtraInfo { get; set; }
        public string pNonCriticalDisplayMessage { get; set; }
        public string pCriticalDisplayMessage { get; set; }
        public string pFileType { get; set; }

        public string pLatitude { get; set; }
        public string pLongitude { get; set; }

        public string pRedTime { get; set; }
        public string pAmberTime { get; set; }

        public cFilm pBelongsToFilm { get; set; }
        public List<string> pErrorCollection { get; set; }
        public cEncryptedPictureFile pEncryptedPicture { get; set; }

        public long pPictureOffset { get; set; }

        public bool pHasError
        {
            get
            {
                if (pOffenceDate == null)
                {
                    return true;
                }

                if (pErrorCollection.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool pIsJammer
        {
            get
            {
                if (pSpeed != null && pSpeed.Value > 300)
                {
                    return true;
                }

                return false;
            }
        }

        public virtual bool pIsTest
        {
            get
            {
                if (pTest.ToUpper().Trim() == "YES")
                {
                    return true;
                }

                return false;
            }
        }

        #region Image Info

        public string pTest { get; set; }
        public bool IsPartOfAverageSpeed { get; set; }

        public List<cPlate> pPlates { get; set; }

        public string pClassification { get; set; }

        public DateTime? DotIntialTime { get; set; }
        public DateTime? DotFinalTime { get; set; }

        public string pDvdSession { get; set; }
        public string pMachineId { get; set; }
        public string pLocationCode { get; set; }

        public string pSession { get; set; }

        public string pFormattedSession
        {
            get { return pDvdSession + "-" + pSession; }
        }

        public string pOperatorId { get; set; }

        public string pDate { get; set; }
        public string pTime { get; set; }

        public string pDirection { get; set; }
        public double? pDistance { get; set; }
        public string pLane { get; set; }

        public DateTime? pOffenceDate
        {
            get
            {
                if (_offenceDate != null)
                {
                    return _offenceDate;
                }

                if (string.IsNullOrEmpty(pDate) && string.IsNullOrEmpty(pTime) && string.IsNullOrEmpty(pBelongsToFilm.pCameraDriver.pCameraDateFormat) && DotFinalTime == null)
                {
                    return null;
                }

                if (DotFinalTime != null)
                {
                    return DotFinalTime;
                }

                try
                {
                    return DateTime.ParseExact(pDate.Replace('-', '/') + " " + pTime, pBelongsToFilm.pCameraDriver.pCameraDateFormat.Replace('*', '/'), CultureInfo.InvariantCulture);
                }
                catch
                {
                    return null;
                }
            }
            set { _offenceDate = value; }
        }

        public string pOffenceDateStringYYMMDD
        {
            get
            {
                if (pOffenceDate != null)
                {
                    return pOffenceDate.Value.ToString("yyMMdd");
                }

                return "";
            }
        }

        public string pOffenceDateStringYYYYMMDD
        {
            get
            {
                if (pOffenceDate != null)
                {
                    return pOffenceDate.Value.ToString("yyyyMMdd");
                }

                return "";
            }
        }

        public string pOffenceDateStringMMDD
        {
            get
            {
                if (pOffenceDate != null)
                {
                    return pOffenceDate.Value.ToString("MMdd");
                }

                return "";
            }
        }

        public string pOffenceDateStringDD_MM_YYYY
        {
            get
            {
                if (pOffenceDate != null)
                {
                    return pOffenceDate.Value.ToString("dd/MM/yyyy");
                }

                return "";
            }
        }

        public string OffenceCode { get; set; }
        public string FileName { get; set; }
        public string SourceFileName { get; set; }
        public string OfficerCode { get; set; }

        #endregion
    }
}