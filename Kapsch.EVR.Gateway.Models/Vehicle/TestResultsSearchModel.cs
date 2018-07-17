using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class TestResultsSearchModel
    {
        public List<TestCategoryModel> TestCategories = new List<TestCategoryModel>();
        public BookingSearchTypeModel BookingSearchType { get; set; }
    }
}
