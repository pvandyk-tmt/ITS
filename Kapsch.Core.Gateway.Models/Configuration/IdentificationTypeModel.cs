using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Gateway.Models.Configuration
{
    public class IdentificationTypeModel
    {
        public long ID { get; set; }

        public string Description { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
