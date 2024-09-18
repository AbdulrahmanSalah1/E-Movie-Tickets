// Provides the IEntityBase interface
using eTickets.Data.Base;
// Provides generic collections like List
using System.Collections.Generic;
// Provides attributes for validation and display
using System.ComponentModel.DataAnnotations;

namespace eTickets.Models
{
    // Represents a governance or region entity
    public class Govern:IEntityBase
    {
        // Primary key for the Govern entity
        [Key]
        public int Id { get; set; }
        // Name of the governance or region
        [Display(Name = "Governorate")]
        public string Name { get; set; }
        // Relationships
        // One-to-many relationship with Cinema
        public List<Cinema> Cinemas { get; set; }
    }
}
