using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Models
{
    // Represents an item in a shopping cart
    public class ShoppingCartItem
    {
        // Primary key for the ShoppingCartItem entity
        [Key]
        public int Id { get; set; }
        // Navigation property for the Movie entity
        public Movie Movie { get; set; }
        // Quantity of the movie in the shopping cart
        public int Amount { get; set; }
        // Unique identifier for the shopping cart
        public string ShoppingCartId { get; set; }
    }
}
