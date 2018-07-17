using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class TestCategoryModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ActiveStatusID { get; set; }
        public int IsTISCheckRequired { get; set; }
        public int CanContinueNoTIS { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
