using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleTestModel
    {
        public int ID { get; set; }
        public int BookingID { get; set; }

        [Display(Name="Booking Re.")]
        public string BookingReference { get; set; }
        public int SiteID { get; set; }

        [Display(Name = "Site")]
        public string SiteName { get; set; }

        [Display(Name = "Started")]
        public DateTime? StartedTimestamp { get; set; }

        [Display(Name = "Ended")]
        public DateTime? EndedTimestamp { get; set; }

        [Display(Name = "VIN")]
        public string VIN { get; set; }

        [Display(Name = "Engine Number")]
        public string EngineNumber { get; set; }

        public int VehicleCategoryID { get; set; }

        [Display(Name = "Category")]
        public string VehicleCategoryName { get; set; }
        public int VehicleTypeID { get; set; }

        [Display(Name = "Type")]
        public string VehicleTypeName { get; set; }
        public int VehicleMakeID { get; set; }

        [Display(Name = "Make")]
        public string VehicleMakeName { get; set; }
        public int VehicleModelID { get; set; }

        [Display(Name = "Model")]
        public string VehicleModelName { get; set; }
        public int VehicleModelNumberID { get; set; }

        [Display(Name = "Model Number")]
        public string VehicleModelNumberName { get; set; }

        [Display(Name = "Year")]
        public int YearOfMake { get; set; }
        public int ColourID { get; set; }

        [Display(Name = "Color")]
        public string ColourName { get; set; }

        [Display(Name = "VLN")]
        public string VLN { get; set; }

        [Display(Name = "Net Weight kg")]
        public int NetWeight { get; set; }

        public bool HasPassed { get; set; }

        public int TestCategoryID { get; set; }
        public string TestCategoryName { get; set; }
        public string UserFullName { get; set; }

        [Display(Name = "GVM kg")]
        public int GVM { get; set; }
        public int PropelledByID { get; set; }
        public int RegistrationStatusID { get; set; }
        public DateTime? LicenceExpiryDate { get; set; }
        public DateTime? RoadworthyExpiryDate { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public IList<VehicleTestAnswerModel> TestAnswers { get; set; }
    }

    public class VehicleTestAnswerModel
    {
        public long ID { get; set; }
        public string Question { get; set; }
        public string QuestionType { get; set; }
        public string Answer { get; set; }
        public string Comments { get; set; }
    }
}
