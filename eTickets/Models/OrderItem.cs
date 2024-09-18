using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Models
{
    // Represents an individual item in an order
    public class OrderItem
    {
        // Primary key for the OrderItem entity
        [Key]
        public int Id { get; set; }
        // Quantity of the movie being ordered
        public int Amount { get; set; }
        // Price of the movie being ordered
        public double Price { get; set; }
        // Foreign key for the Movie entity
        public int MovieId { get; set; }
        // Navigation property for the Movie entity
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
        // Foreign key for the Order entity
        public int OrderId { get; set; }
        // Navigation property for the Order entity
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
