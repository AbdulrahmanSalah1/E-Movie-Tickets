using System.Collections.Generic;

namespace eTickets.Models
{
    // Represents a filter model used for searching and filtering movies,
    //  cinemas, and governance regions
    public class Filter
    {
        // Collection of movies to filter or search from
        public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();

        // Collection of cinemas to filter or search from
        public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();

        // Collection of governance regions to filter or search from
        public IEnumerable<Govern> Governs { get; set; } = new List<Govern>();
    }
}
