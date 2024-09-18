// Provides the IEntityBase interface
using eTickets.Data.Base;
using System;
using System.Collections.Generic;
// Provides attributes for validation and display
using System.ComponentModel.DataAnnotations;
// Provides attributes for database schema configuration
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Models
{
    // Represents a cinema in the system
    public class Cinema:IEntityBase
    {
        // Primary key for the Cinema entity
        [Key]
        public int Id { get; set; }
        // URL or path for the cinema's logo
        [Display(Name = "Cinema Logo")]
        public string Logo { get; set; }
        // Name of the cinema
        [Display(Name = "Cinema Name")]
        [Required(ErrorMessage = "Cinema name is required")]
        public string Name { get; set; }
        // Description of the cinema
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Cinema description is required")]
        public string Description { get; set; }
        // Foreign key for the Govern entity
        public int GovernId { get; set; }
        [ForeignKey("GovernId")]
        // Navigation property for the Govern entity
        public Govern Governorate { get; set; }

        //Relationships
        // One-to-many relationship with Movie
        public List<Movie> Movies { get; set; }
    }
}
