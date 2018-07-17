using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.User
{
    public class SystemRoleModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long ID { get; set; }
        public IList<SystemFunctionModel> SystemFunctions { get; set; }
    }
}
