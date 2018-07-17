using Kapsch.Core.Data.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("CORRESPONDENCE_TEMPLATE", Schema = "CORRESPONDENCE")]
    public class CorrespondenceTemplate
    {
        public string Generate(Dictionary<string, string> parameterValues)
        {
            string message = Value;

            foreach (var pair in parameterValues)
            {
                message = message.Replace("<%" + pair.Key + "%>", pair.Value);
            }

            return message;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("KEY")]
        public string Key { get; set; }

        [Column("CORRESPONDENCE_TYPE_ID")]
        public CorrespondenceType CorrespondenceType { get; set; }

        [Column("TEMPLATE_VALUE")]
        public string Value { get; set; }
    }
}
