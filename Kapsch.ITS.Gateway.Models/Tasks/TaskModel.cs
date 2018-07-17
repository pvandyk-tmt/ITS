using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Tasks
{
    public class TaskModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public long Low { get; set; }
        public long Medium { get; set; }
        public long Critical { get; set; }
    }
}
