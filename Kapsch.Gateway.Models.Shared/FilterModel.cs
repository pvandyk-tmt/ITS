
using Kapsch.Gateway.Models.Shared.Enums;

namespace Kapsch.Gateway.Models.Shared.Models
{
    public class FilterModel
    {
        public string PropertyName { get; set; }
        public Operation Operation { get; set; }
        public object Value { get; set; }
    }
}
