using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.Core.Gateway.Models.User
{
    public class ResetPasswordModel
    {
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        public string Token { get; set; }
    }
}