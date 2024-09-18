// Provides the IEntityBase interface
using eTickets.Data.Base;
using System;
using System.Collections.Generic;
// Provides attributes for validation and display
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Models
{
    // Represents a movie producer in the system
    public class Producer:IEntityBase
    {
        // Primary key for the Producer entity
        [Key]
        public int Id { get; set; }

        // URL for the producer's profile picture
        [Display(Name = "Profile Picture")]
        public string ProfilePictureURL { get; set; }

        // The producer's full name
        [Display(Name = "Full Name")]
        // Validation to ensure the full name is provided
        [Required(ErrorMessage = "Full Name is required")]
        // Validation for string length
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 50 chars")]
        public string FullName { get; set; }

        // A brief biography of the producer
        [Display(Name = "Biography")]
        // Validation to ensure a biography is provided
        [Required(ErrorMessage = "Biography is required")]
        public string Bio { get; set; }

        // Navigation property representing the relationship between Producer and Movie
        // A producer can be associated with multiple movies
        public List<Movie> Movies { get; set; }
    }
}
