using System;
using System.Collections.Generic;
using System.Globalization;
using Kapsch.Camera.Translator.Interfaces;
using Kapsch.RTE.Gateway.Models;
using Kapsch.RTE.Gateway.Models.Camera;
using Kapsch.RTE.Gateway.Models.Camera.Enum;

namespace Kapsch.Camera.Translator.Translators.iCam
{
    public class InfringementLineTextTranslator : ITranslator
    {
        public const int LineLength = 16;

        private DateTime? _infringeDateTime;
        private string[] _lineSplit;
        private object _textLine;
        private int? _threshold;
        private int? _zone;

        /// <summary>
        ///     This class reads a logfile produced by the safeTcam camera used in combindation with other cameras to produce DOT
        ///     infrigments
        ///     Note. Local folder is the folder where the camera files was copied to when copied from the camera
        ///     Camear folder is the folder where the files are located on the actual camera
        /// </summary>
        /// Datum,Tyd,LocationCode,Latitude,Longitude,Direction,Distance,Classification,Zones,Thresholds,SerialNumber,HardwareID,NumberPlate,PlateConfidence,VOSIReason,ENCPath
        public InfringementLineTextTranslator()
        {
            ZoneClassifications = new Dictionary<string, int>();
            ZoneThresholdClassifications = new Dictionary<string, int>();
        }

        public AtPointModel Translate()
        {
            if (TextLine == null)
            {
                throw new Exception("You did not provide a SourceTextLine to translate!");
            }

            AtPointModel model = new AtPointModel
            {
                AnprAccuracy = Confidence,
                Image = null,
                MachineId = MachineId,
                SectionPointCode = LocationCode,
                SerialNumber = SerialNumber,
                Speed = null,
                SourceTextLine = TextLine.ToString(),
                Vln = Vln,
                VosiReason = VosiReason,
                EventDateTime = CreatedOn,
                ImageName = EncFileName,
                ImagePhysicalFileAndPath = EncFileAndPathRaw,
                PlateImageName = "",
                PlateImagePhysicalFileAndPath = "",
                ShotDistance = Distance,
                SectionPointDescription = ""
            };

            if (Classification.ToUpper() == "L")
            {
                model.Classification = new ClassificationZoneModel
                {
                    Classification = VehicleClassificationEnum.Light
                };
            }

            if (Classification.ToUpper() == "H")
            {
                model.Classification = new ClassificationZoneModel
                {
                    Classification = VehicleClassificationEnum.Heavy
                };
            }

            if (Classification.ToUpper() == "PT")
            {
                model.Classification = new ClassificationZoneModel
                {
                    Classification = VehicleClassificationEnum.PublicTransport
                };
            }

            model.Classification.Zone = Zone ?? 0;
            model.Classification.Grace = Threshold ?? 0;

            if (Direction == "TOWARDS")
            {
                model.ShotDirection = DirectionEnum.Towards;
            }

            if (Direction == "AWAY")
            {
                model.ShotDirection = DirectionEnum.Away;
            }

            model.PointLocation = new LocationModel
            {
                GpsLatitude = Latitude,
                GpsLongitude = Longitude
            };

            return model;
        }

        public object TextLine
        {
            get { return _textLine; }
            set
            {
                _textLine = value;
                _lineSplit = _textLine.ToString().Split(',');
            }
        }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? InfringeDateTime
        {
            get
            {
                if (_infringeDateTime != null)
                    return _infringeDateTime;

                if (!IsValidLength())
                    return null;

                DateTime infringementDateTime;

                if (DateTime.TryParseExact(_lineSplit[0].Replace('-', '/'), "yyyy/MM/dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out infringementDateTime))
                {
                    TimeSpan ts;
                    TimeSpan.TryParse(_lineSplit[1], CultureInfo.InvariantCulture, out ts);
                    _infringeDateTime = new DateTime(infringementDateTime.Year, infringementDateTime.Month,
                        infringementDateTime.Day, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

                    return _infringeDateTime;
                }

                return null;
            }
        }

        public string LocationCode
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[2];
            }
        }

        public string Latitude
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[3];
            }
        }

        public string Longitude
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[4];
            }
        }

        /// <summary>
        ///     "TOWARDS" or "AWAY"
        /// </summary>
        public string Direction
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[5];
            }
        }

        public double Distance
        {
            get
            {
                if (!IsValidLength())
                    return 0;

                return double.Parse(_lineSplit[6]);
            }
        }

        /// <summary>
        ///     H or L or PT
        /// </summary>
        public string Classification
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[7];
            }
        }

        /// <summary>
        ///     Zones kan 'n spoed wees (as daar nie heavy vehicle classification is nie, bv 80) of "xxx(L) xxx(PT) xxx(H)" (bv
        ///     120(L) 110(PT) 80(H) )
        /// </summary>
        public string ZoneString
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[8];
            }
        }

        /// <summary>
        ///     Thresholds kan 'n spoed wees (as daar nie heavy vehicle classification is nie, bv 80) of "xxx(L) xxx(PT) xxx(H)"
        ///     (bv 120(L) 110(PT) 80(H) )
        /// </summary>
        public string ThresholdString
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[9];
            }
        }

        public string SerialNumber
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[10];
            }
        }

        public string MachineId
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[11];
            }
        }

        public string Vln
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[12];
            }
        }

        public double Confidence
        {
            get
            {
                if (!IsValidLength())
                    return 0;

                return double.Parse(_lineSplit[13]);
            }
        }

        public string VosiReason
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[14];
            }
        }

        /// <summary>
        ///     Full Name and Path of the .Enc file as per the logifle
        /// </summary>
        public string EncFileAndPathRaw
        {
            get
            {
                if (!IsValidLength())
                    return null;

                return _lineSplit[15];
            }
        }

        /// <summary>
        ///     The .Enc file name
        /// </summary>
        public string EncFileName
        {
            get
            {
                var index = EncFileAndPathRaw.LastIndexOf('/');

                if (index > 0)
                    return EncFileAndPathRaw.Substring(index, EncFileAndPathRaw.Length - index).TrimStart('/');

                return "";
            }
        }

        public int? Zone
        {
            get { return _zone ?? (_zone = GetZoneAtClassification(ZoneString, Classification)); }
        }

        public int? Threshold
        {
            get { return _threshold ?? (_threshold = GetThresholdAtClassification(ThresholdString, Classification)); }
        }

        public Dictionary<string, int> ZoneClassifications { get; private set; }

        public Dictionary<string, int> ZoneThresholdClassifications { get; private set; }

        private int GetZoneAtClassification(string zoneString, string classification)
        {
            if (!string.IsNullOrEmpty(zoneString))
            {
                if (ZoneClassifications.Count == 0)
                {
                    //120(L) 110(PT) 80(H)
                    var indexofClassL = zoneString.IndexOf("(L)", StringComparison.InvariantCulture);
                    var indexofClassPt = zoneString.IndexOf("(PT)", StringComparison.InvariantCulture);
                    var indexofClassH = zoneString.IndexOf("(H)", StringComparison.InvariantCulture);

                    if (indexofClassL > 0 && indexofClassPt > 0 && indexofClassH > 0)
                    {
                        var zsL = zoneString.Substring(0, indexofClassL);
                        ZoneClassifications.Add("L", int.Parse(zsL));

                        var zsPt = zoneString.Substring(indexofClassL + 3, indexofClassPt - (indexofClassL + 3));
                        ZoneClassifications.Add("PT", int.Parse(zsPt));

                        var zsH = zoneString.Substring(indexofClassPt + 4, indexofClassH - (indexofClassPt + 4));
                        ZoneClassifications.Add("H", int.Parse(zsH));
                    }
                }

                if (!string.IsNullOrEmpty(classification))
                    foreach (var zoneClassification in ZoneClassifications)
                    {
                        if (zoneClassification.Key == classification)
                            return zoneClassification.Value;

                        return int.Parse(zoneString);
                    }
                else
                    return int.Parse(zoneString);
            }

            return 0;
        }

        private int GetThresholdAtClassification(string thresholdString, string classification)
        {
            if (!string.IsNullOrEmpty(thresholdString))
            {
                if (ZoneThresholdClassifications.Count == 0)
                {
                    //120(L) 110(PT) 80(H)
                    var indexofClassL = thresholdString.IndexOf("(L)", StringComparison.InvariantCulture);
                    var indexofClassPt = thresholdString.IndexOf("(PT)", StringComparison.InvariantCulture);
                    var indexofClassH = thresholdString.IndexOf("(H)", StringComparison.InvariantCulture);

                    if (indexofClassL > 0 && indexofClassPt > 0 && indexofClassH > 0)
                    {
                        var zsL = thresholdString.Substring(0, indexofClassL);
                        ZoneClassifications.Add("L", int.Parse(zsL));

                        var zsPt = thresholdString.Substring(indexofClassL + 3, indexofClassPt - (indexofClassL + 3));
                        ZoneClassifications.Add("PT", int.Parse(zsPt));

                        var zsH = thresholdString.Substring(indexofClassPt + 4, indexofClassH - (indexofClassPt + 4));
                        ZoneClassifications.Add("H", int.Parse(zsH));
                    }
                }

                if (!string.IsNullOrEmpty(classification))
                    foreach (var zoneClassification in ZoneThresholdClassifications)
                    {
                        if (zoneClassification.Key == classification)
                            return zoneClassification.Value;

                        return int.Parse(thresholdString);
                    }
                else
                    return int.Parse(thresholdString);
            }

            return 0;
        }

        private bool IsValidLength()
        {
            if (_lineSplit.Length == LineLength)
                return true;

            return false;
        }
    }
}