using System;
using System.Collections.Generic;
// Required for data annotations (validation attributes)
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.ViewModels
{
    
    public class LoginVM
    {
        // Property for the user's email address
        // Sets the display name for the field in the view
        [Display(Name = "Email address")]
        // Makes the field mandatory with a custom error message
        [Required(ErrorMessage = "Email address is required")]
        // Ensures the input is a valid email format with a custom error message
        [EmailAddress(ErrorMessage = "Enter The Proper Email Addresss !")]
        // The user's email address
        public string EmailAddress { get; set; }

        // Property for the user's password
        // Specifies that the field should be treated as a password (e.g., masked input)
        [DataType(DataType.Password)]
        // Makes the field mandatory with a custom error message
        [Required(ErrorMessage = "Password is required")]
        // Specifies the minimum and maximum length of the password
        [StringLength(100, ErrorMessage = "The password must be at least {0} characters long.", MinimumLength = 8)]
        // Enforces a complex password pattern with a custom error message
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character.")]
        // The user's password
        public string Password { get; set; }
    }
}
