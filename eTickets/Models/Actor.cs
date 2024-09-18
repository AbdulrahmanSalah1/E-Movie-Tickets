using eTickets.Data.Base;// Provides the IEntityBase interface
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;// Provides attributes for validation and display
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Models
{
    public class Actor:IEntityBase
    {
        // Primary key for the Actor entity
        [Key]
        public int Id { get; set; }
        // URL for the actor's profile picture
        [Display(Name = "Profile Picture")]
        public string ProfilePictureURL { get; set; }

        // The actor's full name
        [Display(Name = "Full Name")]
        // Validation to ensure the full name is provided
        [Required(ErrorMessage = "Full Name is required")]
        // Validation for string length
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 50 chars")]
        public string FullName { get; set; }

        // A brief biography of the actor
        [Display(Name = "Biography")]
        // Validation to ensure a biography is provided
        [Required(ErrorMessage = "Biography is required")]
        public string Bio { get; set; }

        // Navigation property representing the relationship between Actor and Actor_Movie
        // An actor can be associated with multiple movies
        public List<Actor_Movie> Actors_Movies { get; set; }
    }
}


