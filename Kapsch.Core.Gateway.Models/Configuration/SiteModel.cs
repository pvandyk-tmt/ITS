
namespace Kapsch.Core.Gateway.Models.Configuration
{
    
    public class SiteModel
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public long SiteTypeID { get; set; }

        public long? DistrictID { get; set; }

        public string DistrictName { get; set; }

    }
}
