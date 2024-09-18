using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Models
{
    // Represents an order placed by a user
    public class Order
    {
        // Primary key for the Order entity
        [Key]
        public int Id { get; set; }
        // Email of the user who placed the order
        public string Email { get; set; }
        // User ID of the user who placed the order
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        // Date and time when the order was placed
        public DateTime OrderDate { get; set; }
        // Date and time for the movie show
        // (when the movie is scheduled to be shown)
        public DateTime MovieDate { get; set; }
        // Navigation property for the ApplicationUser entity
        public ApplicationUser User { get; set; }
        // Collection of items included in the order
        public List<OrderItem> OrderItems { get; set; }
    }
}
