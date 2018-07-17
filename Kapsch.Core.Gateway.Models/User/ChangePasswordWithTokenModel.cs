using FoolproofWebApi;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.Core.Gateway.Models.User
{
    public class ChangePasswordWithTokenModel
    {
        [Required]
        public string Token { get; set; }
        
        [Required]
        [StringLength(128, ErrorMessage = "The new password must be at least 8 characters long.", MinimumLength = 8)]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DisplayName("Confirm New Password")]
        [EqualTo("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string RepeatNewPassword { get; set; }
    }
}