using System.ComponentModel.DataAnnotations;

namespace CrilieTechReviewsBlog.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Set new password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and password confirmation must match")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
