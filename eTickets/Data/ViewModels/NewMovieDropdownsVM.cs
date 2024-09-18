using eTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.ViewModels
{
    public class NewMovieDropdownsVM
    {
        // Constructor to initialize the lists
        public NewMovieDropdownsVM()
        {
            Producers = new List<Producer>();
            Cinemas = new List<Cinema>();
            Actors = new List<Actor>();
        }
        // List of producers to populate a dropdown
        public List<Producer> Producers { get; set; }
        // List of cinemas to populate a dropdown
        public List<Cinema> Cinemas { get; set; }
        // List of actors to populate a dropdown
        public List<Actor> Actors { get; set; }
    }
}
