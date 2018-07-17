using System;

namespace Kapsch.Core.Gateway.Models.Configuration
{
    public class AddressInfoModel
    {
        public long ID { get; set; }

        public long AddressTypeID { get; set; }

        public long SourceID { get; set; }

        public long PersonInfoID { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Suburb { get; set; }

        public string Town { get; set; }

        public string Country { get; set; }

        public string Code { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public long? CreatedUserDetailID { get; set; }

        public DateTime CreatedDate { get; set; }

        public long? IsPrefferedIndicator { get; set; }
    }
}
