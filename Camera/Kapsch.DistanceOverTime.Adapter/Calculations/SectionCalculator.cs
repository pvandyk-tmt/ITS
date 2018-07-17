using System;
using System.Collections.Generic;
using Kapsch.RTE.Gateway.Models;
using Kapsch.RTE.Gateway.Models.Camera;

namespace Kapsch.DistanceOverTime.Adapter.Calculations
{
    public class SectionCalculator
    {
        public SectionCalculator(int levensteinMatchDistance, double distanceInMeterInMeter, string sectionDescription, string sectionCode, List<AtPointModel> pointsA, List<AtPointModel> pointsB)
        {
            _pointsA = pointsA;
            _pointsB = pointsB;
            _sectionDistance = distanceInMeterInMeter;
            _sectionDescription = sectionDescription;
            _sectionCode = sectionCode;
            _levensteinMatchDistance = levensteinMatchDistance;
        }

        private readonly List<AtPointModel> _pointsA;
        private readonly List<AtPointModel> _pointsB;

        private readonly double _sectionDistance;
        private readonly string _sectionDescription;
        private readonly string _sectionCode;
        private readonly int _levensteinMatchDistance;

        public List<SectionCalculationResult> Calculate()
        {
            try
            {
                for (int i = 0; i < _pointsA.Count; i++)
                {
                    if (Expires(_pointsA[i]))
                    {
                        _pointsA.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                //string s = ex.Message;
            }

            try
            {
                for (int j = 0; j < _pointsB.Count; j++)
                {
                    if (Expires(_pointsB[j]))
                    {
                        _pointsB.RemoveAt(j);
                    }
                }

            }
            catch (Exception ex)
            {
                //string s = ex.Message;
            }
            

            return GetOffences(_pointsA, _pointsB);
        }

        private List<SectionCalculationResult> GetOffences(List<AtPointModel> pointsA, List<AtPointModel> pointsB)
        {
            List<SectionCalculationResult> offences = new List<SectionCalculationResult>();

            for (int i = 0; i < pointsA.Count; i++)
            {
                bool isOffence = false;
                for (int j = 0; j < pointsB.Count; j++)
                {
                   SectionCalculationResult model = new SectionCalculationResult(_levensteinMatchDistance, _sectionDistance, _sectionDescription, _sectionCode, pointsA[i], pointsB[j]);

                    if (model.VlnIsMatch)
                    {
                        if (model.IsOffence)
                        {
                            offences.Add(model);
                            isOffence = true;
                        }

                        try
                        {
                            pointsB.RemoveAt(j);
                        }
                        catch (Exception ex)
                        {
                            //string s = ex.Message;
                        }
                    }
                }

                if (isOffence)
                {
                    try
                    {
                        pointsA.RemoveAt(i);
                    }
                    catch (Exception ex)
                    {
                        //string s = ex.Message;
                    }
                }

            }

            return offences;
        }

        /// <summary>
        /// Check if the point in the bag of point expires. The idea is that if the point has remained in the bag for longer than the time
        /// it could take to travel the entire section, the point is expired and could no longer be an infringement irrespective of when the infringements occur.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool Expires(AtPointModel point)
        {
            if (point.CreateDate == null)
            {
                point.CreateDate = DateTime.Now;
            }

            double timeH = _sectionDistance / 1000 / point.Classification.Zone;

            DateTime maxTa = point.CreateDate.Value.AddHours(2 * timeH);
            DateTime minTa = point.CreateDate.Value.AddHours(-2 * timeH);

            if (DateTime.Now < minTa || DateTime.Now > maxTa)
            {
                return true;
            }

            return false;
        }
    }
}
