using System.ComponentModel.DataAnnotations;

namespace CrilieTechReviewsBlog.ViewModels
{
    public class SendPasswordResetTokenViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
