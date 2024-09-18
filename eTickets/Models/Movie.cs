// Provides access to base classes and data context
using eTickets.Data;
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
    // Represents a movie in the system
    public class Movie:IEntityBase
    {
        // Primary key for the Movie entity
        [Key]
        public int Id { get; set; }
        // The name of the movie
        public string Name { get; set; }
        // A brief description of the movie
        public string Description { get; set; }
        // The price of the movie ticket
        public double Price { get; set; }
        // URL for the movie's poster or image
        public string ImageURL { get; set; }
        // The date when the movie starts showing
        public DateTime StartDate { get; set; }
        // The date when the movie ends its run
        public DateTime EndDate { get; set; }
        // The time when the movie is shown
        public DateTime ShowTime { get; set; }
        // The number of available tickets for the movie
        public int AvailableTickets {  get; set; }
        // The category of the movie (e.g., Action, Comedy)
        public MovieCategory MovieCategory { get; set; }

        //Relationships
        // Many-to-many relationship between Movie and Actor through Actor_Movie
        public List<Actor_Movie> Actors_Movies { get; set; }

        public ICollection<Comment> Comments { get; set; } // New property
        public ICollection<Rating> Ratings { get; set; } // New property

        // Foreign key and navigation property for Cinema
        public int CinemaId { get; set; }
        [ForeignKey("CinemaId")]
        public Cinema Cinema { get; set; }

        // Foreign key and navigation property for Producer
        public int ProducerId { get; set; }
        [ForeignKey("ProducerId")]
        public Producer Producer { get; set; }
    }
    
}
