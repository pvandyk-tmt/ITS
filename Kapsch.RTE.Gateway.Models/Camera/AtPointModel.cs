using System;
using Kapsch.RTE.Gateway.Models.Camera.Enum;

namespace Kapsch.RTE.Gateway.Models.Camera
{
    public class AtPointModel : ICloneable
    {
        /// <summary>
        /// The Point Location Code for this section
        /// </summary>
        public string SectionPointCode { get; set; }

        /// <summary>
        /// The Point Location Description for this section
        /// </summary>
        public string SectionPointDescription { get; set; }

        public string SourceTextLine { get; set; }

        public DateTime EventDateTime { get; set; }

        public double AnprAccuracy { get; set; }

        public double ShotDistance { get; set; }

        public LocationModel PointLocation { get; set; }

        /// <summary>
        ///     "TOWARDS" or "AWAY"
        /// </summary>
        public DirectionEnum ShotDirection { get; set; }
        
        /// <summary>
        /// The zone, grace and vehicle classification
        /// </summary>
        public ClassificationZoneModel  Classification { get; set; }

        /// <summary>
        /// The speed travelled at the point
        /// </summary>
        public int? Speed { get; set; }

        /// <summary>
        /// Camera Serial Number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Camera Machine Id
        /// </summary>
        public string MachineId { get; set; }

        public string Vln { get; set; }

        public string HashVln { get; set; }

        public string VosiReason { get; set; }

        public byte[] Image { get; set; }

        public string ImagePhysicalFileAndPath { get; set; }

        public string ImageName { get; set; }

        public string PlateImagePhysicalFileAndPath { get; set; }

        public string PlateImageName { get; set; }

        /// <summary>
        /// The Source of the listener that created the Point Data.
        /// For Socket use 'IPAddress:Port'
        /// For Mock use 'Mock'
        /// For Disk use 'FilePath'
        /// </summary>
        public string ListenerSource { get; set; }

        public bool IsOffence { get; set; }

        public DateTime? CreateDate { get; set; }

        public object Clone()
        {
            return new AtPointModel
            {
                CreateDate = this.CreateDate,
                Vln = this.Vln,
                EventDateTime = this.EventDateTime,
                HashVln = this.HashVln,
                AnprAccuracy = this.AnprAccuracy,
                Classification = this.Classification,
                Image = this.Image,
                ImageName = this.ImageName,
                ImagePhysicalFileAndPath = this.ImagePhysicalFileAndPath,
                IsOffence = this.IsOffence,
                ListenerSource = this.ListenerSource,
                MachineId = this.MachineId,
                PlateImageName = this.PlateImageName,
                PlateImagePhysicalFileAndPath = this.PlateImagePhysicalFileAndPath,
                PointLocation = this.PointLocation,
                SectionPointCode = this.SectionPointCode,
                SectionPointDescription = this.SectionPointDescription,
                SerialNumber = this.SerialNumber,
                ShotDirection = this.ShotDirection,
                ShotDistance = this.ShotDistance,
                SourceTextLine = this.SourceTextLine,
                Speed = this.Speed,
                VosiReason = this.VosiReason
            };
        }
    }
}
