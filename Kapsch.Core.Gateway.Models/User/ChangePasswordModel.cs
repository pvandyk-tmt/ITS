using FoolproofWebApi;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.Core.Gateway.Models.User
{
    public class ChangePasswordModel
    {
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [DisplayName("Current Password")]
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "The new password must be at least 8 characters long.", MinimumLength = 8)]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DisplayName("Confirm New Password")]
        [EqualTo("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}