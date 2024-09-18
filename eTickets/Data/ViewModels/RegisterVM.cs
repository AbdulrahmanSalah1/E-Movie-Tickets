using System;
using System.Collections.Generic;
// Required for data annotations (validation attributes)
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 100 characters long.")]
        public string FullName { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string EmailAddress { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 100 characters long.")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be at least {1} characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character.")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
