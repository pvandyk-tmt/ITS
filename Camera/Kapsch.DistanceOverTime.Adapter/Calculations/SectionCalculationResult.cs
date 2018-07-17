using System;
using System.IO;
using Kapsch.RTE.Gateway.Models;
using Kapsch.RTE.Gateway.Models.Camera;
using Kapsch.RTE.Gateway.Models.Camera.Enum;

namespace Kapsch.DistanceOverTime.Adapter.Calculations
{
    public class SectionCalculationResult
    {
        private double? _averageSpeed;
        private double? _travelDistance;
        private double? _tripDuration;
        private readonly int _levensteinMatchDistance;

        public SectionCalculationResult(int levensteinMatchDistance, double distanceInMeterInMeter, string sectionDescription, string sectionCode, AtPointModel start, AtPointModel end)
        {
            DateFormat = "dd/MM/yyyy HH:mm:ss";

            SectionDistanceInMeter = distanceInMeterInMeter;
            AtPointStart = (AtPointModel) start.Clone();
            AtPointEnd = (AtPointModel)end.Clone(); ;
            SectionDescription = sectionDescription;
            SectionCode = sectionCode;
            _levensteinMatchDistance = levensteinMatchDistance;
        }

        public AtPointModel AtPointStart { get; private set; }
        public AtPointModel AtPointEnd { get; private set; }

        public bool IsOffence
        {
            get
            {
                if (!VlnIsMatch)
                {
                    return false;
                }

                if (AverageSpeed != null && GraceSpeed != null && Zone != null)
                {
                    if (AverageSpeed > GraceSpeed + Zone)
                        return true;
                }

                return false;
            }
        }

        public double? AverageSpeed
        {
            get
            {
                if (_averageSpeed != null)
                {
                    return _averageSpeed;
                }

                if (!VlnIsMatch)
                {
                    _averageSpeed = null;
                    return _averageSpeed;
                }

                if (TravelDistance != null && TripDuration != null && TripDuration > 0)
                {
                    _averageSpeed = TravelDistance / TripDuration / 1000;
                }

                return _averageSpeed;
            }
        }

        public int? GraceSpeed
        {
            get
            {
                if (!VlnIsMatch)
                {
                    return null;
                }

                if (AtPointStart.Classification.Grace > AtPointEnd.Classification.Grace)
                {
                    return AtPointStart.Classification.Grace;
                }

                return AtPointEnd.Classification.Grace;
            }
        }

        /// <summary>
        ///     Returns the Duration in hours
        /// </summary>
        public double? TripDuration
        {
            get
            {
                if (_tripDuration != null)
                {
                    return _tripDuration;
                }

                if (!VlnIsMatch)
                {
                    _tripDuration = null;
                    return _tripDuration;
                }

                var duration = Math.Abs(AtPointStart.EventDateTime.Subtract(AtPointEnd.EventDateTime).TotalMilliseconds);
                duration /= 1000; //seconds
                duration /= 3600; //hour
                _tripDuration = duration;

                return _tripDuration;
            }
        }

        public double? TravelDistance
        {
            get
            {
                if (_travelDistance != null)
                {
                    return _travelDistance;
                }

                if (!VlnIsMatch)
                {
                    _travelDistance = null;
                    return _travelDistance;
                }

                _travelDistance = SectionDistanceInMeter;
                var sign = 1;

                if (AtPointStart.EventDateTime.Subtract(AtPointStart.EventDateTime).TotalMilliseconds > 0)
                    sign = -1;

                if (AtPointStart.ShotDirection == DirectionEnum.Towards)
                    _travelDistance += AtPointStart.ShotDistance * sign;
                else
                    _travelDistance -= AtPointStart.ShotDistance * sign;

                if (AtPointEnd.ShotDirection == DirectionEnum.Away)
                    _travelDistance += AtPointEnd.ShotDistance * sign;
                else
                    _travelDistance -= AtPointEnd.ShotDistance * sign;

                return _travelDistance;
            }
        }

        public double SectionDistanceInMeter { get; private set; }
        public string SectionDescription { get; set; }
        public string SectionCode { get; set; }

        public int? Zone
        {
            get
            {
                if (!VlnIsMatch)
                {
                    return null;
                }

                if (AtPointStart.Classification.Zone > AtPointEnd.Classification.Zone)
                {
                    return AtPointStart.Classification.Zone;
                }

                return AtPointEnd.Classification.Zone;
            }
        }

        public int FrameNumber { get; set; }

        public string Vln
        {
            get
            {
                if (!VlnIsMatch)
                {
                    return null;
                }

                return AtPointEnd.Vln;
            }
        }

        public bool VlnIsMatch
        {
            get
            {
                if (AtPointStart == null || AtPointEnd == null)
                    return false;

                if (AtPointStart.HashVln == AtPointEnd.HashVln)
                    return true;

                return VlnIsLevensteinMatch;
            }
        }

        public bool VlnIsLevensteinMatch
        {
            get
            {
                if (AtPointStart == null || AtPointEnd == null)
                    return false;

                if (LevenshteinDistance(AtPointStart.HashVln, AtPointEnd.HashVln) <= _levensteinMatchDistance)
                {
                    return true;
                }

                return false;
            }
        }

        public string FileName
        {
            get
            {
                if (AtPointStart == null || AtPointEnd == null)
                    return "";

                return string.Format("Img{0}{1}{2}.dot", Path.GetFileNameWithoutExtension(AtPointStart.ImageName), Path.GetFileNameWithoutExtension(AtPointEnd.ImageName), DateTime.Now.Ticks);
            }
        }

        public string DateFormat { get; set; }

        public double? AverageAnprAccuracy
        {
            get
            {
                if (!VlnIsMatch)
                {
                    return null;
                }

                if (AtPointStart.AnprAccuracy > AtPointEnd.AnprAccuracy)
                {
                    return AtPointEnd.AnprAccuracy;
                }

                return AtPointStart.AnprAccuracy;
            }
        }

        public string MachineId
        {
            get
            {
                if (!VlnIsMatch)
                {
                    return null;
                }

                if (AtPointStart != null && AtPointEnd == null)
                {
                    return AtPointStart.MachineId;
                }

                if (AtPointEnd != null && AtPointStart == null)
                {
                    return AtPointEnd.MachineId;
                }

                if (AtPointEnd != null && AtPointStart != null && AtPointStart.EventDateTime > AtPointEnd.EventDateTime)
                {
                    return AtPointStart.MachineId;
                }

                return AtPointEnd == null ? null : AtPointEnd.MachineId;
            }
        }

        public override string ToString()
        {
            if (!VlnIsMatch)
            {
                return "";
            }

            if (AtPointStart.EventDateTime > AtPointEnd.EventDateTime)
            {
                return string.Format("[Possible] Infringement Created for Vln {0} travelling at speed {1} km/h in {2} km/h zone over {3} m [{4} - {5}]",
                    Vln, AverageSpeed, Zone, TravelDistance, AtPointEnd.EventDateTime.ToString(DateFormat), AtPointStart.EventDateTime.ToString(DateFormat));
            }

            return string.Format("[Possible] Infringement Created for Vln {0} travelling at speed {1} km/h in {2} km/h zone over {3} m [{4} - {5}]",
                Vln, AverageSpeed, Zone, TravelDistance, AtPointStart.EventDateTime.ToString(DateFormat), AtPointEnd.EventDateTime.ToString(DateFormat));
        }


        /// <summary>
        ///     In information theory and computer science, the Levenshtein distance is a metric for measuring the amount of
        ///     difference between two sequences (i.e. an edit distance). The Levenshtein distance between two strings is defined
        ///     as the minimum number of edits needed to transform one string into the other, with the allowable edit operations
        ///     being insertion, deletion, or substitution of a single character.
        ///     For example, the Levenshtein distance between "kitten" and "sitting" is 3, since the following three edits change
        ///     one into the other, and there is no way to do it with fewer than three edits:
        ///     kitten sitten (substitution of 'k' with 's')
        ///     sitten sittin (substitution of 'e' with 'i')
        ///     sittin sitting (insert 'g' at the end).
        ///     The Levenshtein distance between "rosettacode", "raisethysword" is 8; The distance between two strings is same as
        ///     that when both strings is reversed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static int LevenshteinDistance(string source, string match)
        {
            var d = new int[source.Length + 1, match.Length + 1];
            for (int i = 0; i <= source.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= match.Length; j++)
                d[0, j] = j;
            for (int j = 1; j <= match.Length; j++)
            for (int i = 1; i <= source.Length; i++)
                if (source[i - 1] == match[j - 1])
                    d[i, j] = d[i - 1, j - 1]; //no operation
                else
                    d[i, j] = Math.Min(Math.Min(
                            d[i - 1, j] + 1, //a deletion
                            d[i, j - 1] + 1), //an insertion
                        d[i - 1, j - 1] + 1 //a substitution
                    );
            return d[source.Length, match.Length];
        }
    }
}