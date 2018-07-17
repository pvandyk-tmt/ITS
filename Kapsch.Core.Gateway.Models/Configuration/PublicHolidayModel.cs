using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    public class PublicHolidayModel
    {
        public long ID { get; set; }
        public string HolidayDescription { get; set; }
        public DateTime HolidayDate { get; set; }
        public bool IsActive { get; set; }
    }
}
