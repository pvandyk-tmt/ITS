using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.User
{
    public class SystemFunctionModel
    {
        //[Required]
        //[StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public long ID { get; set; }
    }
}
