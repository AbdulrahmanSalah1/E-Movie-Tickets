// Required for IdentityUser, which provides identity-related properties and methods
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
// Required for data annotations (e.g., Display attribute)
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Models
{
    // ApplicationUser class that extends the IdentityUser class
    public class ApplicationUser:IdentityUser
    {
        // Property for the user's full name
        // Sets the display name for the field in the view
        [Display(Name = "Full name")]
        // The user's full name
        public string FullName { get; set; }
        [Display(Name ="Picture Profile")]
        public string ProfileImageUrl { get; set; }

        [Display(Name ="Is Active")]
        public bool IsActive { get; set; }

        public ICollection<ContactUs> ContactUss { get; set; }
    }
}
