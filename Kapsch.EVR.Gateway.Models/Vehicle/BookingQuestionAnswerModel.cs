using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class BookingQuestionAnswerModel
    {
        public int TestTypeID { get; set; }
        public string TestTypeDescription { get; set; }
        public int BookingID { get; set; }
        public string SiteName { get; set; }
        public long SiteID { get; set; }
        public string TestCategory { get; set; }
        public string BarcodeData { get; set; }
        public bool IsPhotoRequired { get; set; }
        //public int IsSuccessfull { get; set; }

        public List<QuestionModel> Questions;
        
    }
}
