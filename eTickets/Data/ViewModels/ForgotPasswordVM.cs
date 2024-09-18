using System.ComponentModel.DataAnnotations;

namespace eTickets.Data.ViewModels
{
    public class ForgotPasswordVM
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Enter The Proper Email Addresss !")]
        public string Email { get; set; }
    }
}
