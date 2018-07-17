using Kapsch.Core.Gateway.Models.Configuration;
using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.User
{
    public class UserModel
    {
        public long ID { get; set; }
        public long CredentialID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(128)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(128)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email Address")]
        
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedTimestamp { get; set; }

        [Display(Name = "Is Officer")]
        public bool IsOfficer { get; set; }

        [Display(Name = "External ID")]
        public string ExternalID { get; set; }

        public UserStatus Status { get; set; }

        public string FormattedStatus
        {
            get { return Status.ToString(); }
        }

        public string FormattedName
        {
            get { return string.Format("{0}, {1}", LastName, FirstName); }
        }

        public IList<DistrictModel> Districts { get; set; }
        public IList<SystemFunctionModel> SystemFunctions { get; set; }
        public long? SystemRoleID { get; set; }
    }
}
