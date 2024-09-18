using eTickets.Data.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.ViewModels
{
    public class ShoppingCartVM
    {
        // Represents the current shopping cart and its items
        public ShoppingCart ShoppingCart { get; set; }
        // The total price of all items in the shopping cart
        public double ShoppingCartTotal { get; set; }
    }
}
