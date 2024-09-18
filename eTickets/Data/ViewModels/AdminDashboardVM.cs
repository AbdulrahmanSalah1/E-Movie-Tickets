using eTickets.Models;
using System.Collections.Generic;

namespace eTickets.Data.ViewModels
{
    public class AdminDashboardVM
    {
        public int TotalMovies { get; set; }
        public int TotalCinemas { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalGoverns { get; set; }
        public int TotalProducers { get; set; }
        public int TotalActors { get; set; }

        public int TotalComments { get; set; }
        public int TotalRates { get; set; }
    }
}
