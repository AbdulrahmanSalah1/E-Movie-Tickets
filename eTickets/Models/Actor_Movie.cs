using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Models
{
    // Represents the many-to-many relationship between Actor and Movie
    public class Actor_Movie
    {
        // Foreign key for the Movie entity
        public int MovieId { get; set; }
        // Navigation property for the Movie entity
        public Movie Movie { get; set; }
        // Foreign key for the Actor entity
        public int ActorId { get; set; }
        // Navigation property for the Actor entity
        public Actor Actor { get; set; }
    }
}
